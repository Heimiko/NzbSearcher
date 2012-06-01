using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Net;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NzbSearcher
{
    public partial class frmMain : Form
    {
        System.Windows.Forms.Timer _VersionCheckTimer = new System.Windows.Forms.Timer();

        new public ICustomTabControl ActiveControl { get; private set; }

        public frmMain()
        {
            // don't do initialization here, use frmMain_Load for that!
        }

        /// <summary>
        /// MUST call this func before activating it!
        /// </summary>
        public void Init()
        {
            InitializeComponent();
            this.ImdbWatchList.WatchListItem_DoubleClick += new WatchListItem_DoubleClick_Event(WatchListItem_DoubleClick);
        }

        void _VersionCheckTimer_Tick(object sender, EventArgs e)
        {
            DoCheckForNewVersion();
        }

        void AddCustomTabControl(ICustomTabControl TabCtrl)
        {
            TabCtrl.Button = new ToolStripButton()
            {
                Tag = TabCtrl,
                Image = TabCtrl.Icon,
                Text = TabCtrl.DisplayName,
                ToolTipText = TabCtrl.ToolTip,
                TextImageRelation = TextImageRelation.ImageAboveText,
            };

            MainTabControl.Items.Add(TabCtrl.Button);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;

            _VersionCheckTimer.Interval = 1000 * 60 * 60; //once every hour
            _VersionCheckTimer.Tick += new EventHandler(_VersionCheckTimer_Tick);

            if (Global.Config.FormBounds.Width > 0)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Bounds = Global.Config.FormBounds;
                this.WindowState = Global.Config.WindowState;

                //only set toolbar positions when locations are different (ie. we have actually stored the info before)
                if (Global.Config.ToolbarPos_Tabs != Global.Config.ToolbarPos_Client)
                {
                    this.MainTabControl.Location = Global.Config.ToolbarPos_Tabs;
                    this.MainToolStrip.Location = Global.Config.ToolbarPos_Main;
                    this.ClientToolStip.Location = Global.Config.ToolbarPos_Client;
                }
            }
            else
                this.StartPosition = FormStartPosition.CenterScreen;

            Global.SABnzbd.StatusUpdated += new StatusUpdatedEvent(SABnzbd_StatusUpdated);
            NzbSearcher.Config.Saving += new ConfigEvent(Config_Saving);

            foreach (ISearchProvider prov in SearchProvidersCollection.Providers)
                if (!prov.Config.Disabled)
                    AddCustomTabControl(new SearchControl(prov));

            foreach (ICustomTabControl TabCtrl in CustomTabControlsCollection.CustomTabControls)
                if (!TabCtrl.Config.Disabled)
                    AddCustomTabControl(TabCtrl);

            ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;

            //show 1st tab
            MainTabControl_ItemClicked(MainTabControl, new ToolStripItemClickedEventArgs(MainTabControl.Items[0]));
            UpdateControls();

            Global.SABnzbd.Start();
            Global.AutoEpisodeDownloader.Start();
            Global.FolderWatcher.Start();

            txtSpeedLimit.KeyPress += new KeyPressEventHandler(txtSpeedLimit_KeyPress);

            if (this.WindowState == FormWindowState.Minimized && Global.Config.ShowInTray)
            {
                ShowHideForm(false); //we should be hidden
                frmMain_Activated(this, new EventArgs()); //activate form (as we're hidden, this should be done by code)
            }
        }

        void UpdateToolStripButtons(ToolStrip toolStrip, bool ShowButtonText)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item is ToolStripButton)
                {
                    item.TextImageRelation = TextImageRelation.ImageAboveText;
                    item.DisplayStyle = ShowButtonText ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image;
                    item.AutoSize = !ShowButtonText;
                    if (ShowButtonText)
                    {
                        item.Width = 51;
                        item.Height = 51;
                    }
                }
            }
        }

        void UpdateControls()
        {
            bool ShowButtonText = Global.Config.ShowButtonText;
            UpdateToolStripButtons(MainTabControl, ShowButtonText);
            UpdateToolStripButtons(MainToolStrip, ShowButtonText);
            UpdateToolStripButtons(ClientToolStip, ShowButtonText);
            notifyIcon1.Visible = Global.Config.ShowInTray;

            SABnzbd_StatusUpdated(Global.SABnzbd.Status);
        }
        
        void Config_Saving()
        {
            Global.Config.WindowState = this.WindowState;
            Global.Config.ToolbarPos_Tabs = MainTabControl.Location;
            Global.Config.ToolbarPos_Main = MainToolStrip.Location;
            Global.Config.ToolbarPos_Client = ClientToolStip.Location;
        }

        bool bFirstTimeActivate = true;
        private void frmMain_Activated(object sender, EventArgs e)
        {
            if (!bFirstTimeActivate)
                return;
            bFirstTimeActivate = false;
#if !DEBUG
            DoCheckForNewVersion(); //will start timer when done
#endif
            frmMain_Resize(sender, e); 

            if (!Config.ConfigFileExists)
                new frmConfig().ShowDialog();
        }

        void DoCheckForNewVersion()
        {
            Thread NewThread = new Thread(new ThreadStart(CheckForNewVersion));
            NewThread.Name = "CheckForNewVersion";
            NewThread.Priority = ThreadPriority.BelowNormal;
            NewThread.Start();
        }

        void CheckForNewVersion()
        {
            _VersionCheckTimer.Stop();
            
            try
            {
                HttpWebRequest VersionRequest = (HttpWebRequest)HttpWebRequest.Create("http://sourceforge.net/projects/nzbsearcher/files/README.txt/download");

                using (WebResponse resp = VersionRequest.GetResponse())
                using (Stream respStream = resp.GetResponseStream())
                using (StreamReader reader = new StreamReader(respStream))
                {
                    string NewestVersion = null;
                    string strLine = reader.ReadLine();
                    while (strLine != null) {
                        Match Error = Regex.Match(strLine, "^Latest Version: (.*)");
                        if (Error.Success)
                        {
                            NewestVersion = Error.Groups[1].Value;
                            break;
                        }
                        strLine = reader.ReadLine();
                    }

                    if (NewestVersion != null)
                    {
                        string CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                        if (CurrentVersion != NewestVersion)
                        {
                            if (MessageBox.Show("A new version of NzbSearcher (v" + NewestVersion + ") is available for download.\r\nWould you like to open a webbrowser so you can download it?", "NzbSearcher", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("https://sourceforge.net/projects/nzbsearcher/files/");
                                Invoke((VoidDelegate)Close); //close program
                            }

                            return; // don't start timer again, only show message once.
                        }
                    }
                }
            }
            catch (Exception)
            {
                // do nothing, no need to bother the user
            }

            _VersionCheckTimer.Start();
        }

        static string FormText = null;
        void SABnzbd_StatusUpdated(SABnzbdStatus status)
        {
            if (FormText == null)
                FormText = this.Text;

            string DisplayFormText = FormText + " - " + ActiveControl.ToolTip;

            lblSpeed.Text = "Cur: " + status.Speed;
            lblSpace.Text = "Space: " + status.DiskSpace;
            lblLimit.Text = "Speed Limit: " + (status.SpeedLimit > 0 ? (status.SpeedLimit + " KB/sec") : "None");

            if (status.IsPaused)
                this.Text = "Paused - " + DisplayFormText;
            else if (status.QueuedItems.Count > 0)
                this.Text = status.TimeLeft + " / " + status.SizeLeft + " left - " + DisplayFormText;
            else
                this.Text = DisplayFormText;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                ShowHideForm(false);
            else
                ShowHideForm(true);

            if (this.WindowState == FormWindowState.Normal)
                Global.Config.FormBounds = this.Bounds;
        }

        private void frmMain_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                Global.Config.FormBounds = this.Bounds;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && Global.Config.ShowInTray)
            {
                if (Global.AutoEpisodeDownloader.Episodes.Count > 0 && !Global.AutoEpisodeDownloader.Config.Disabled)
                {
                    DialogResult res = MessageBox.Show(
                        "You have configured one or more Auto Download items.\r\n" +
                        "NzbSearcher can't automatically download episodes when it's not running.\r\n" +
                        "\r\n" +
                        "Are you sure you want to close NzbSearcher?",
                        "NzbSearcher", MessageBoxButtons.YesNo);
                    if (res != System.Windows.Forms.DialogResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            Global.ApplicationIsTerminating = true;
            NzbSearcher.Config.Save();
        }

        delegate void SetStatusDelegate(string Status);
        public void SetStatus(string Status)
        {
            if (Status == null)
                Status = string.Empty;
            if (this.InvokeRequired)
            {
                try { this.Invoke((SetStatusDelegate)SetStatus, Status); }
                catch { }
            }
            else if (lblStatus.Text != Status) //don't trigger update if not needed
                lblStatus.Text = Status;
        }

        private void cmdConfig_Click(object sender, EventArgs e)
        {
            DialogResult res = new frmConfig().ShowDialog();
            if (res == DialogResult.OK)
            {
                Global.SABnzbd.RefreshQueueStatus();
                UpdateControls();

                if (!Global.Config.ShowInTray) //make sure we're visible t the crowd
                {
                    ActiveControl.Visible = true;
                    this.Visible = true;
                }
            }
        }

        private void cmdHelp_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (ActiveControl != null)
                ActiveControl.OnKeyPreview(sender, e);
        }


        public void ShowHideForm(bool bShow)
        {
            if (Program.MainForm.InvokeRequired)
            {
                Global.InvokeOnGUI((VoidDelegateBool)ShowHideForm, bShow);
                return;
            }

            if (bShow)
            {
                //re-add active control to content
                if (!panel1.Controls.Contains(ActiveControl as Control))
                    panel1.Controls.Add(ActiveControl as Control);

                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;

                this.Focus();
                Global.SABnzbd.RefreshQueueStatus(); //trigger refresh
                
            }
            else if (!bShow && this.Visible)
            {
                if (Global.Config.ShowInTray)
                    this.Visible = false;

                //remove all content, no flickering
                panel1.Controls.Clear();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowHideForm(true);
        }

        private void MainTabControl_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is ICustomTabControl)
            {
                foreach (ToolStripButton item in MainTabControl.Items)
                    item.Checked = (item == e.ClickedItem);
                    //((UserControl)item.Tag).Visible = item.Checked;

                ICustomTabControl NewActiveControl = e.ClickedItem.Tag as ICustomTabControl;
                if (NewActiveControl != null)
                {
                    using (new LockWindowUpdate(this.Handle))
                    {
                        ActiveControl = NewActiveControl;

                        //remove all ICustomTabControl's
                        panel1.Controls.Clear();
                        panel1.Controls.Add(ActiveControl as Control);
                        ActiveControl.Dock = DockStyle.Fill;

                        //ActiveControl.BringToFront();
                        SABnzbd_StatusUpdated(Global.SABnzbd.Status);

                        //remove any tool buttons, re-add them
                        ClientToolStip.Items.Clear();
                        ActiveControl.AddToolbarItems(ClientToolStip.Items);
                        UpdateToolStripButtons(ClientToolStip, Global.Config.ShowButtonText);
                    }
                }
                
            }
        }

        private void lblLimit_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem)
                Global.SABnzbd.SetSpeedLimit(int.Parse((string)e.ClickedItem.Tag));
        }

        private void lblLimit_DropDownOpening(object sender, EventArgs e)
        {
            txtSpeedLimit.Text = Global.SABnzbd.Status.SpeedLimit.ToString();
        }

        private void lblLimit_DropDownOpened(object sender, EventArgs e)
        {
            txtSpeedLimit.Focus();
            txtSpeedLimit.SelectAll();
        }

        private void lblLimit_DropDownClosed(object sender, EventArgs e)
        {
            int NewSpeedLimit = Global.SABnzbd.Status.SpeedLimit;
            if (int.TryParse(txtSpeedLimit.Text, out NewSpeedLimit))
                if (NewSpeedLimit != Global.SABnzbd.Status.SpeedLimit)
                    Global.SABnzbd.SetSpeedLimit(NewSpeedLimit);
        }

        void txtSpeedLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                SendKeys.Send("{ESC}"); //closes the context menu
        }

        private void mnuTrayExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuTrayShow_Click(object sender, EventArgs e)
        {
            ShowHideForm(true);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                mnuTray.Show(Cursor.Position);
        }

        private void cmdWatchList_Click(object sender, EventArgs e)
        {
            cmdWatchList.Checked = !cmdWatchList.Checked;
            splitter1.Visible = cmdWatchList.Checked;
            ImdbWatchList.Visible = cmdWatchList.Checked;
        }

        void WatchListItem_DoubleClick(WatchListItem item)
        {
            if (ActiveControl is SearchControl)
            {
                SearchControl control = (SearchControl)ActiveControl;

                if (control.SearchProvider is NzbMatrix) //special handling for NzbMatrix
                {
                    ((SearchSettings_NzbMatrix)control.SearchProvider.SearchParams).SearchInWebLink = true;
                    control.SearchProvider.ProviderToScreen(); //update SearchInWebLink to screen
                    control.DoSearch(item.const_);
                }
                else
                {
                    string SearchText = item.title;
                    string FilteredSearch = string.Empty;

                    if (Global.IMDB.Config.IncludeYearOnSearch)
                        SearchText += " " + item.year;

                    //filter out characters
                    foreach (char c in SearchText)
                    {
                        if (char.IsLetterOrDigit(c))
                            FilteredSearch += c;
                        else if (FilteredSearch.Length > 0 && !FilteredSearch.EndsWith(" "))
                            FilteredSearch += " "; //convert unknown character to space if last char wasnt one
                    }

                    control.DoSearch(FilteredSearch);
                }
            }
        }
    }
}
