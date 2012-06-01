using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.IO;

namespace NzbSearcher
{
    public partial class AutoDownloader_Control : UserControl, ICustomTabControl
    {
        public string DisplayName { get { return "Auto"; } }
        public string ToolTip { get { return "Automatic Episode Downloader"; } }
        public Image Icon { get { return Properties.Resources.auto; } }

        public IGUIelementConfig Config { get { return Config<AutoDownloaderConfig>.Value; } }

        List<ListViewItem> _CachedItems = new List<ListViewItem>();
        System.Windows.Forms.Timer _RefreshTimer = new System.Windows.Forms.Timer();

        //Controls
        ToolStripButton cmdAdd = new ToolStripButton(Properties.Resources.add) { Text = "Add", ToolTipText = "Add a new episode to watch/download" };
        ToolStripButton cmdDelete = new ToolStripButton(Properties.Resources.delete) { Text = "Delete", ToolTipText = "Delete selected episode(s) from watch/download list" };

        public ToolStripButton Button { get; set; }

        public AutoDownloader_Control()
        {
            InitializeComponent();
            Global.AutoEpisodeDownloader.AutoEpisodeChanged += new AutoEpisodeChangedEvent(AutoDownloader_AutoEpisodeChanged);
            NzbSearcher.Config.Saving += new ConfigEvent(Config_Saving);

            cmdAdd.Click += new EventHandler(cmdAdd_Click);
            cmdDelete.Click += new EventHandler(cmdDelete_Click);
        }

        private void AutoDownloader_Control_Load(object sender, EventArgs e)
        {
            lstAutoDownloads.Columns.Add("Name", "Episode Name", 200);
            lstAutoDownloads.Columns.Add("Season", "Season", 60);
            lstAutoDownloads.Columns.Add("Episode", "Episode", 60);
            lstAutoDownloads.Columns.Add("LastCheck", "Last Checked", 100);
            lstAutoDownloads.Columns.Add("LastDownload", "Age of last NZB", 100);

            Global.AutoEpisodeDownloader.Config.LoadColumns(lstAutoDownloads);

            UpdateControls();

            _RefreshTimer.Interval = 60 * 1000; //once per minute
            _RefreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
            
            RefreshList(); //will also start the refresh timer
        }

        public void AddToolbarItems(ToolStripItemCollection col)
        {
            col.Add(cmdAdd);
            col.Add(cmdDelete);
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshList();
        }

        void Config_Saving()
        {
            Global.AutoEpisodeDownloader.Config.SaveColumns(lstAutoDownloads);
        }

        private void lstAutoDownloads_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            AutoEpisode ep = Global.AutoEpisodeDownloader.Episodes[e.ItemIndex];
            
            e.Item = new ListViewItem(ep.DisplayName);
            e.Item.SubItems.Add(ep.SeasonNr.ToString());
            e.Item.SubItems.Add(ep.EpisodeNr.ToString());
            e.Item.SubItems.Add(ep.LastCheck > 0 ? Global.GetAgeFrom(new DateTime(ep.LastCheck)) : string.Empty);
            e.Item.SubItems.Add(ep.LastDownload > 0 ? Global.GetAgeFrom(new DateTime(ep.LastDownload)) : string.Empty);
        
            //set color
            TimeSpan Age = DateTime.Now - new DateTime(ep.LastDownload);
            if (Age.TotalDays < 1)
                e.Item.ForeColor = SystemColors.Highlight;
            else if (Age.TotalDays < 6)
                e.Item.BackColor = NzbItem.DownloadedBackgroundColor;
            else if (Age.TotalDays < 8)
                e.Item.BackColor = NzbItem.NewBackgroundColor;
            else if (Age.TotalDays < 15)
                e.Item.BackColor = NzbItem.ErrorBackgroundColor;
            else
                e.Item.BackColor = NzbItem.DownloadedBackgroundColor;
        }

        void AutoDownloader_AutoEpisodeChanged(AutoEpisode ep, int ListIndex)
        {
            RefreshList();
        }

        public void OnTabFocus()
        {

        }

        public void OnKeyPreview(object sender, KeyEventArgs e)
        {

        }

        void UpdateControlsThread()
        {
            Thread.Sleep(10);
            this.Invoke((UpdateControlsDelegate)UpdateControls);
        }

        delegate void UpdateControlsDelegate();
        void UpdateControls()
        {
            cmdDelete.Enabled = lstAutoDownloads.SelectedIndices.Count > 0;
        }


        private void AutoDownloader_Control_Resize(object sender, EventArgs e)
        {
            Size NewSize = this.ClientSize;
            //NewSize.Height -= NzbResultList.Top;
            //NewSize.Width = SearchPanel.Width;
            lstAutoDownloads.Size = NewSize;
        }

        void RefreshList()
        {
            //stop timer, and restart it afterwards. 
            //  this should results in: not updating the grid too oten
            _RefreshTimer.Stop(); 
            //using (new LockWindowUpdate(lstAutoDownloads.Handle))
            {
                lstAutoDownloads.VirtualListSize = Global.AutoEpisodeDownloader.Episodes.Count;
                lstAutoDownloads.Refresh();
            }
            _RefreshTimer.Start();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            frmAutoDownload NewAuto = new frmAutoDownload();
            NewAuto.EpisodeSaved += new frmAutoDownload.EpisodeSavedEvent(NewAuto_EpisodeSaved);
            NewAuto.Show();
        }

        void NewAuto_EpisodeSaved(frmAutoDownload frm, AutoEpisode ep)
        {
            frm.EpisodeSaved -= NewAuto_EpisodeSaved;

            int NewIndex = Global.AutoEpisodeDownloader.Episodes.Add(ep);
            NzbSearcher.Config.Save(); //save our new episode
            RefreshList();
            lstAutoDownloads.SelectedIndices.Clear();
            lstAutoDownloads.SelectedIndices.Add(NewIndex);

            //start new thread to check episode (don't lockup gui)
            Thread NewThread = new Thread(new ParameterizedThreadStart(CheckEpisodeThread));
            NewThread.Name = "CheckEpisode";
            NewThread.Start(ep);
        }

        void CheckEpisodeThread(object ep)
        {
            Global.AutoEpisodeDownloader.CheckEpisode((AutoEpisode)ep);
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected episodes from the Auto Downloader?", "NzbSearcher", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return; //no deleting

            List<int> selectedIndexes = new List<int>();
            foreach (int SelIdx in lstAutoDownloads.SelectedIndices)
                selectedIndexes.Add(SelIdx);
            lstAutoDownloads.VirtualListSize = 0; // prevent problems
            for (int i = selectedIndexes.Count - 1; i >= 0; i--)
                Global.AutoEpisodeDownloader.Episodes.RemoveAt(selectedIndexes[i]);

            lstAutoDownloads.SelectedIndices.Clear();
            NzbSearcher.Config.Save();
            RefreshList();
        }

        private void lstAutoDownloads_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstAutoDownloads.SelectedIndices.Count > 0)
            {
                int SelectedIndex = lstAutoDownloads.SelectedIndices[0];
                AutoEpisode ep = Global.AutoEpisodeDownloader.Episodes[SelectedIndex];
                frmAutoDownload EditAuto = new frmAutoDownload(ep);
                DialogResult res = EditAuto.ShowDialog();
                if (res == DialogResult.OK)
                {
                    Global.AutoEpisodeDownloader.Episodes.RemoveAt(SelectedIndex);
                    SelectedIndex = Global.AutoEpisodeDownloader.Episodes.Add(EditAuto.NewEpisode);
                    NzbSearcher.Config.Save(); //save our new episode
                    RefreshList();
                    lstAutoDownloads.SelectedIndices.Clear();
                    lstAutoDownloads.SelectedIndices.Add(SelectedIndex);

                    //start new thread to check episode (don't lockup gui)
                    Thread NewThread = new Thread(new ParameterizedThreadStart(CheckEpisodeThread));
                    NewThread.Name = "CheckEpisode";
                    NewThread.Start(EditAuto.NewEpisode);
                }
            }
        }

        private void lstAutoDownloads_SelectedIndexChanged(object sender, EventArgs e)
        {
            //kludge!
            new Thread(new ThreadStart(UpdateControlsThread)).Start();
        }

        private void lstAutoDownloads_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstAutoDownloads.SelectedIndices.Count > 0)
            {
                cmdDelete_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
