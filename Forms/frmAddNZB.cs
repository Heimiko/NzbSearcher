using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Xml;

namespace NzbSearcher
{
    public partial class frmAddNZB : Form
    {
        ISearchProvider _prov;
        
        List<NzbItem> _NzbItems;
        List<NzbItem> _DownloadedNzbItems;

        string _OriginalTitle;
        static string _CombinedTempFileName = Path.Combine(Path.GetTempPath(), "NzbSearcher_CombinedDownload.nzb");

        string _LocalNzbFile = null;

        public frmAddNZB(string LocalNzbFile)
        {
            InitializeComponent();
            _LocalNzbFile = LocalNzbFile;
            _OriginalTitle = Path.GetFileNameWithoutExtension(LocalNzbFile);
        }

        public frmAddNZB(ISearchProvider prov, List<NzbItem> Items)
        {
            InitializeComponent();

            if (Items.Count < 1)
                throw new Exception("Unable to download NZB list with item count < 1");

            _prov = prov;
            _NzbItems = Items;
            _DownloadedNzbItems = new List<NzbItem>();
            _OriginalTitle = _NzbItems[0].Title;

            if (Items.Count > 1)
                this.Text = "Add " + Items.Count + " NZBs to SABnzbd's Queue (as a single combined download)";
        }

        private void frmAddNZB_Load(object sender, EventArgs e)
        {
            if (_prov != null)
            {
                _prov.DownloadCompleted += new DownloadCompletedEvent(prov_DownloadCompleted);
                this.FormClosing += new FormClosingEventHandler(frmAddNZB_FormClosing);
            }

            cboFriendly.SelectedIndex = Global.Config.FriendlyNameProcessing;

            txtNZB.Text = _OriginalTitle; // Path.GetFileName(_NzbItem.DownloadLink);
            txtFriendlyName.Text = ProcessFriendlyName();

            //add all configured categories from SABnzbd
            foreach (string cat in Global.SABnzbd.Status.Categories)
                cboCategories.Items.Add(cat);

            //Select last used SABcat
            if (Global.SABnzbd.Status.Categories.Contains(Global.Config.LastSABcat))
                cboCategories.Text = Global.Config.LastSABcat;
            else if (cboCategories.Items.Count > 0)
                cboCategories.SelectedIndex = 0;

            //If our temp file already exists, delete it!
            if (File.Exists(_CombinedTempFileName))
            {
                File.Delete(_CombinedTempFileName);
                if (File.Exists(_CombinedTempFileName))
                    throw new Exception("Couldn't delete temp NZB file");
            }
        }

        void frmAddNZB_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_prov != null)
                _prov.DownloadCompleted -= prov_DownloadCompleted;
        }

        int CompareLength(string x, string y)
        {
            if (x.Length < y.Length)
                return 1;
            if (x.Length > y.Length)
                return -1;
            return 0;
        }

        int GetCategoryIndexForNZB(string NZB)
        {
            NZB = NZB.ToLower(); //make lower case

            //Sort cat's by length, largest first!
            string[] SortedCats = Global.SABnzbd.Status.Categories.ToArray();
            Array.Sort<string>(SortedCats, CompareLength);

            //Now search our NZB for a matching category
            foreach (string cat in SortedCats)
                if (cat != "None") //no searching for default category
                {

                    bool bMatch = true;
                    string[] findStrings = cat.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string findStr in findStrings)
                        if (!NZB.Contains(findStr.ToLower()))
                        {
                            bMatch = false;
                            break;
                        }

                    if (bMatch) //matching category found? then return it
                        return Global.SABnzbd.Status.Categories.IndexOf(cat);
                }

            return 0;
        }

        string ProcessFriendlyName()
        {
            if (cboFriendly.SelectedIndex <= 0)
                return _OriginalTitle;

            string FriendlyName = Global.RemoveNonAlfaNumeric(_OriginalTitle);

            if (_prov == null)
                return FriendlyName;

            switch (cboFriendly.SelectedIndex)
            {
                case 1:
                    return FriendlyName;

                case 2:
                    {
                        //try cutting out garbage, search for the last occurances of our search params,
                        //  everything before, cut it out
                        int FirstIndex = FriendlyName.Length;
                        foreach (string SearchTerm in _prov.SearchParams.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            int LastIndex = FriendlyName.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase);
                            if (LastIndex >= 0 && LastIndex < FirstIndex)
                                FirstIndex = LastIndex;
                        }
                        //now we have our last occurances, cut out some stuff
                        return FriendlyName.Substring(FirstIndex);
                    }

                case 3:
                    {
                        //try cutting out garbage, search for the last occurances of our search params,
                        //  everything before, cut it out
                        int FirstIndex = FriendlyName.Length;
                        foreach (string SearchTerm in _prov.SearchParams.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            int LastIndex = FriendlyName.LastIndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase);
                            if (LastIndex >= 0 && LastIndex < FirstIndex)
                                FirstIndex = LastIndex;
                        }
                        //now we have our last occurances, cut out some stuff
                        return FriendlyName.Substring(FirstIndex);
                    }
            }


            return string.Empty;
        }

        void EnableControls(bool bDownloading)
        {
            this.UseWaitCursor = bDownloading;
            this.txtFriendlyName.Enabled = !bDownloading;
            this.cboCategories.Enabled = !bDownloading;
            this.cboFriendly.Enabled = !bDownloading;
            this.cmdOK.Enabled = !bDownloading;
            //this.cmdCancel.Enabled = !bDownloading;

            bDownloading &= Global.Config.NzbHandling != NzbHandling.SendURL || _NzbItems.Count > 1;
            this.lblDownloading.Visible = bDownloading;

            if (_NzbItems != null && _NzbItems.Count > 0)
                this.lblDownloading.Text = "Downloading " + _NzbItems.Count + " NZB(s), then adding it to the queue...";
            else 
                this.lblDownloading.Text = "Adding NZB to SAB's queue...";

            this.Refresh();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (_LocalNzbFile == null && _NzbItems.Count < 1)
            {
                MessageBox.Show("Unable to download NZB list with item count < 1  (this shouldn't happen)");
                return; // can't
            }
            
            EnableControls(true);

            Global.Config.LastSABcat = cboCategories.Text;

            if (_LocalNzbFile != null)
            {
                Global.SABnzbd.AddNzbByUpload(_LocalNzbFile, txtFriendlyName.Text, cboCategories.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (_NzbItems.Count == 1 && Global.Config.NzbHandling == NzbHandling.SendURL)
            {
                NzbItem item = _NzbItems[0];
                Global.SABnzbd.AddNzbFromURL(item.DownloadLink, txtFriendlyName.Text, cboCategories.Text);
                _prov.MarkAsDownloaded(item);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                _prov.DownloadNZB_Async(_NzbItems[0]);
        }

        void prov_DownloadCompleted(ISearchProvider provider, NzbItem item, string TempFileName, string ErrorMessage)
        {
            try
            {
                if (TempFileName != null && ErrorMessage == null)
                {
                    if (!_NzbItems.Contains(item)) //nothing to do?
                        throw new Exception("NzbItem not contained within list");

                    //combine existing NZB's with eachother
                    CombineNZBs(_CombinedTempFileName, TempFileName);

                    _NzbItems.Remove(item);
                    _DownloadedNzbItems.Add(item);
                    EnableControls(true);

                    if (_NzbItems.Count > 0) //still items left to download?
                    {
                        _prov.DownloadNZB_Async(_NzbItems[0]);
                        return; //wait until next is downloaded
                    }

                    Global.HandleDownloadedNZB(_CombinedTempFileName, txtFriendlyName.Text, cboCategories.Text);

                    foreach(NzbItem DownloadedItem in _DownloadedNzbItems)
                        provider.MarkAsDownloaded(DownloadedItem);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return; //we're done
                }
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;
            }

            if (ErrorMessage != null)
            {
                //we need to start over, so do cleanup, reset to start
                _NzbItems.AddRange(_DownloadedNzbItems);
                _DownloadedNzbItems.Clear();
                if (File.Exists(_CombinedTempFileName))
                    File.Delete(_CombinedTempFileName);

                MessageBox.Show(ErrorMessage);
            }

            EnableControls(false);
        }

        void CombineNZBs(string MasterFile, string AddNZBfile)
        {
            if (!File.Exists(AddNZBfile))
                throw new Exception("Can't combine NZBs, NZB file not found");

            if (!File.Exists(MasterFile))
            {
                File.Move(AddNZBfile, MasterFile);
                return; //done
            }

            using (FileStream NewNzbFileStream = File.Open(AddNZBfile, FileMode.Open))
            {
                //first, validate if propper XML file
                string FileStart = string.Empty;
                while (!FileStart.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                {
                    char NewChar = (char)NewNzbFileStream.ReadByte();
                    if (NewChar >= 32)
                        FileStart += NewChar;

                    if (FileStart.Length == 6)
                        throw new Exception("not a valid XML file");
                }
                //now seek to <nzb> node, keep reading until string ends with node name
                while (!FileStart.EndsWith("<nzb", StringComparison.OrdinalIgnoreCase))
                    FileStart += (char)NewNzbFileStream.ReadByte();
                while (NewNzbFileStream.ReadByte() != '>'); //keep reading until node closes
                //at this point, we're located just after the NZB node opened

                using (FileStream MasterStream = File.Open(MasterFile, FileMode.Open))
                {
                    MasterStream.Seek(-1, SeekOrigin.End);
                    while (MasterStream.ReadByte() != '<')
                        MasterStream.Seek(-2, SeekOrigin.Current);
                    MasterStream.Seek(-1, SeekOrigin.Current);
                    //at this point, current position is right before the closing tag </nzb>

                    //so right now, we can do a simple stream copy from source to master
                    Global.CopyStream(NewNzbFileStream, MasterStream);
                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboFriendly_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFriendlyName.Text = ProcessFriendlyName();
            Global.Config.FriendlyNameProcessing = cboFriendly.SelectedIndex;
        }

        private void frmAddNZB_Activated(object sender, EventArgs e)
        {
            txtFriendlyName.Focus();
        }
    }
}
