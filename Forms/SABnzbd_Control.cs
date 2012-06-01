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
    public partial class SABnzbd_Control : UserControl, ICustomTabControl
    {
        Image _Icon = Properties.Resources.SABnzbd;
        
        public Image Icon { get { return _Icon; } }
        public string DisplayName { get { return "nzbd"; } }
        public string ToolTip { get { return "SABnzbd Control (double click for website)"; } }

        public IGUIelementConfig Config { get { return Config<SABnzbdConfig>.Value; } }

        List<ListViewItem> _CachedItems = new List<ListViewItem>();

        //Controls
        ToolStripButton cmdDelete = new ToolStripButton(Properties.Resources.delete) { Text = "Delete", ToolTipText = "Delete item(s) from SABnzb's download queue" };
        ToolStripButton cmdMoveTop = new ToolStripButton(Properties.Resources.MoveToTop) { Text = "Top", ToolTipText = "Move item(s) to top in SABnzb's download queue" };
        ToolStripButton cmdMoveUp = new ToolStripButton(Properties.Resources.Round_Arrow_Up) { Text = "Up", ToolTipText = "Move item(s) up in SABnzb's download queue" };
        ToolStripButton cmdMoveDown = new ToolStripButton(Properties.Resources.Round_Arrow_Down) { Text = "Down", ToolTipText = "Move item(s) down in SABnzb's download queue" };
        ToolStripButton cmdMoveBottom = new ToolStripButton(Properties.Resources.MoveToBottom) { Text = "Bottom", ToolTipText = "Move item(s) to bottom in SABnzb's download queue" };
        ToolStripButton cmdDeleteHistory = new ToolStripButton(Properties.Resources.delete) { Text = "History", ToolTipText = "Delete item(s) from SABnzb's History" };

        ToolStripButton _Button = null;
        public ToolStripButton Button 
        {
            get { return _Button; }
            set
            {
                if (_Button != null)
                    _Button.DoubleClick -= Button_DoubleClick;

                _Button = value;
                _Button.DoubleClickEnabled = true;
                _Button.DoubleClick += new EventHandler(Button_DoubleClick);
            }
        }

        void Button_DoubleClick(object sender, EventArgs e)
        {
            string URL = ((SABnzbdConfig)Config).HostURL;
            if (!URL.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                URL = "http://" + URL;
            System.Diagnostics.Process.Start(URL);
        }

        bool _BlinkState = false;

        public SABnzbd_Control()
        {
            InitializeComponent();

            cmdDelete.Click += new EventHandler(cmdDelete_Click);
            cmdMoveTop.Click += new EventHandler(cmdMoveTop_Click);
            cmdMoveUp.Click += new EventHandler(cmdMoveUp_Click);
            cmdMoveDown.Click += new EventHandler(cmdMoveDown_Click);
            cmdMoveBottom.Click += new EventHandler(cmdMoveBottom_Click);
            cmdDeleteHistory.Click+= new EventHandler(cmdDeleteHistory_Click);
        
            Global.SABnzbd.StatusUpdated += new StatusUpdatedEvent(SABnzbd_StatusUpdated);
        }

        private void SABnzbd_Control_Load(object sender, EventArgs e)
        {
            NzbSearcher.Config.Saving += new ConfigEvent(Config_Saving);
            

            UpdateControls();

            DownloadList.Columns.Add("Name", "Downloading File", 400);
            DownloadList.Columns.Add("Status", "Status", 100);
            DownloadList.Columns.Add("Progress", "Progress", 120);
            DownloadList.Columns.Add("TimeLeft", "Time Left", 70);
            DownloadList.Columns.Add("Category", "Category", 70);
            DownloadList.Columns.Add("Priority", "Priority", 70);

            HistoryList.Columns.Add("Name", "History File", 400);
            HistoryList.Columns.Add("Status", "Status", 100);
            HistoryList.Columns.Add("fail_msg", "", 300);

            Global.SABnzbd.Config.LoadColumns(DownloadList);
            Global.SABnzbd.Config.LoadColumns(HistoryList);

            SABnzbd_StatusUpdated(Global.SABnzbd.Status);
            SABnzbd_Control_Resize(sender, e);
        }

        public void AddToolbarItems(ToolStripItemCollection col)
        {
            col.Add(cmdDelete);
            col.Add(cmdMoveTop);
            col.Add(cmdMoveUp);
            col.Add(cmdMoveDown);
            col.Add(cmdMoveBottom);
            col.Add(new ToolStripSeparator());
            col.Add(cmdDeleteHistory);
        }

        void Config_Saving()
        {
            Global.SABnzbd.Config.SaveColumns(DownloadList);
            Global.SABnzbd.Config.SaveColumns(HistoryList);
        }

        void SABnzbd_StatusUpdated(SABnzbdStatus status)
        {
            _Icon = status.HistoryItems.HasErrors ? Properties.Resources.SABnzbd_red : Properties.Resources.SABnzbd;

            //do blinking if needed
            if (status.QueuedItems.Count > 0)
            {
                _BlinkState = !_BlinkState;
                if (_BlinkState)
                    Button.Image = Properties.Resources.SABnzbd_green;
                else
                    Button.Image = this.Icon;
            }
            else
            {
                Button.Image = this.Icon;
                _BlinkState = false;
            }


            //using (new LockWindowUpdate(DownloadList.Handle))
            {
                DownloadList.VirtualListSize = status.QueuedItems.Count;
                DownloadList.Refresh();
            }

            if (status.HistoryItems.HasChanged || HistoryList.VirtualListSize != status.HistoryItems.Count)
            {
                //using (new LockWindowUpdate(HistoryList.Handle))
                {
                    HistoryList.VirtualListSize = status.HistoryItems.Count;
                    HistoryList.Refresh();
                }
            }

            UpdateControls();
        }

        public void OnTabFocus()
        {

        }

        public void OnKeyPreview(object sender, KeyEventArgs e)
        {
        
        }

        private void ListSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            SABnzbd_Control_Resize(sender, e);
        }

        private void SABnzbd_Control_Resize(object sender, EventArgs e)
        {
            HistoryList.Top = ListSplitter.Bottom;
            HistoryList.Left = 0;
            HistoryList.Width = ListSplitter.Width;
            HistoryList.Height = this.ClientSize.Height - HistoryList.Top;
        }

        private void DownloadList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            SABnzbdItem item = Global.SABnzbd.Status.QueuedItems[e.ItemIndex];

            e.Item = new ListViewItem(item.FileName);
            e.Item.Tag = item;
            
            if (item.MB > 0)
            {
                float Downloaded = item.MB - item.MBleft;
                string Progress = (int)((Downloaded / item.MB) * 100) + "% (" + item.SizeLeft + " left)";

                e.Item.SubItems.Add(item.Status);
                e.Item.SubItems.Add(Progress);
                e.Item.SubItems.Add(item.TimeLeft);
                e.Item.SubItems.Add(item.Category);
                e.Item.SubItems.Add(item.Priority);
            }
            else
            {
                //Item is not actually within queue for downloading yet
                // so don't display progress and time
                e.Item.SubItems.Add(string.Empty);
                e.Item.SubItems.Add(string.Empty);
                e.Item.SubItems.Add(string.Empty);
                e.Item.SubItems.Add(string.Empty);
                e.Item.SubItems.Add(string.Empty);
            }
        }

        private void HistoryList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            SABnzbdItem item = Global.SABnzbd.Status.HistoryItems[e.ItemIndex];

            e.Item = new ListViewItem(item.FileName)
            {
                BackColor = item.BackColor
            };
            
            e.Item.Tag = item;
            e.Item.SubItems.Add(item.Status);
            e.Item.SubItems.Add(item.FailMessage);
        }

        void WaitBitAndThenUpdateControls()
        {
            Thread NewThread = new Thread(new ThreadStart(UpdateControlsThread));
            NewThread.Name = "UpdateControls";
            NewThread.Priority = ThreadPriority.BelowNormal;
            NewThread.Start();
        }

        void UpdateControlsThread()
        {
            Thread.Sleep(10);
            this.Invoke((UpdateControlsDelegate)UpdateControls);
        }

        delegate void UpdateControlsDelegate();
        void UpdateControls()
        {
            cmdDelete.Enabled = DownloadList.SelectedIndices.Count > 0;
            cmdMoveTop.Enabled = cmdDelete.Enabled;
            cmdMoveUp.Enabled = cmdDelete.Enabled;
            cmdMoveDown.Enabled = cmdDelete.Enabled;
            cmdMoveBottom.Enabled = cmdDelete.Enabled;
            cmdDeleteHistory.Enabled = HistoryList.SelectedIndices.Count > 0;

            foreach (int SelIdx in DownloadList.SelectedIndices)
            {
                if (SelIdx <= 0)
                {
                    cmdMoveUp.Enabled = false;
                    cmdMoveTop.Enabled = false;
                }
                if (SelIdx >= (Global.SABnzbd.Status.QueuedItems.Count - 1))
                {
                    cmdMoveDown.Enabled = false;
                    cmdMoveBottom.Enabled = false;
                }
            }
        }

        private void DownloadList_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaitBitAndThenUpdateControls();
        }

        private void HistoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaitBitAndThenUpdateControls();
        }


        private void cmdDelete_Click(object sender, EventArgs e)
        {
            SABnzbdItemsCollection DeleteCol = new SABnzbdItemsCollection();
            foreach (int SelIndex in DownloadList.SelectedIndices)
                DeleteCol.Add(Global.SABnzbd.Status.QueuedItems[SelIndex]);

            if (MessageBox.Show("Remove all selected items from the queue?", "NzbSearcher", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = DeleteCol.Count - 1; i >= 0; i--)
                {
                    SABnzbdItem qi = DeleteCol[i];
                    Global.SABnzbd.RemoveFromQueue(qi);
                    DownloadList.SelectedIndices.Remove(qi.Index);
                }
            }
        }

        private void cmdDeleteHistory_Click(object sender, EventArgs e)
        {
            SABnzbdItemsCollection DeleteCol = new SABnzbdItemsCollection();
            foreach (int SelIndex in HistoryList.SelectedIndices)
                DeleteCol.Add(Global.SABnzbd.Status.HistoryItems[SelIndex]);

            if (MessageBox.Show("Remove all selected items from history?", "NzbSearcher", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = DeleteCol.Count - 1; i >= 0; i--)
                {
                    SABnzbdItem qi = DeleteCol[i];
                    Global.SABnzbd.RemoveFromHistory(qi);
                    HistoryList.SelectedIndices.Remove(qi.Index);
                }
            }
        }

        private void cmdMoveUp_Click(object sender, EventArgs e)
        {
            SABnzbdItemsCollection MoveCol = new SABnzbdItemsCollection();
            foreach (int SelIndex in DownloadList.SelectedIndices)
                MoveCol.Add(Global.SABnzbd.Status.QueuedItems[SelIndex]);

            foreach (SABnzbdItem qi in MoveCol)
            {
                Global.SABnzbd.MoveItem(qi, qi.Index - 1);
                DownloadList.SelectedIndices.Remove(qi.Index);
                DownloadList.SelectedIndices.Add(qi.Index - 1);
            }
        }

        private void cmdMoveDown_Click(object sender, EventArgs e)
        {
            SABnzbdItemsCollection MoveCol = new SABnzbdItemsCollection();
            foreach (int SelIndex in DownloadList.SelectedIndices)
                MoveCol.Add(Global.SABnzbd.Status.QueuedItems[SelIndex]);

            for(int i = MoveCol.Count - 1; i >= 0; i--)
            {
                SABnzbdItem qi = MoveCol[i];
                Global.SABnzbd.MoveItem(qi, qi.Index + 1);
                DownloadList.SelectedIndices.Remove(qi.Index);
                DownloadList.SelectedIndices.Add(qi.Index + 1);
            }
        }

        void cmdMoveTop_Click(object sender, EventArgs e)
        {
            SABnzbdItemsCollection MoveCol = new SABnzbdItemsCollection();
            foreach (int SelIndex in DownloadList.SelectedIndices)
                MoveCol.Add(Global.SABnzbd.Status.QueuedItems[SelIndex]);

            for (int i = 0; i < MoveCol.Count; i++)
            {
                SABnzbdItem qi = MoveCol[i];
                Global.SABnzbd.MoveItem(qi, i);
                DownloadList.SelectedIndices.Remove(qi.Index);
                DownloadList.SelectedIndices.Add(i);
            }

            UpdateControls();
        }

        void cmdMoveBottom_Click(object sender, EventArgs e)
        {
            SABnzbdItemsCollection MoveCol = new SABnzbdItemsCollection();
            foreach (int SelIndex in DownloadList.SelectedIndices)
                MoveCol.Add(Global.SABnzbd.Status.QueuedItems[SelIndex]);

            for (int i = MoveCol.Count - 1; i >= 0; i--)
            {
                SABnzbdItem qi = MoveCol[i];
                int NewIndex = DownloadList.VirtualListSize - (MoveCol.Count - i);
                Global.SABnzbd.MoveItem(qi, NewIndex);
                DownloadList.SelectedIndices.Remove(qi.Index);
                DownloadList.SelectedIndices.Add(NewIndex);
            }

            UpdateControls();
        }

        private void DownloadList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && DownloadList.SelectedIndices.Count > 0)
            {
                cmdDelete_Click(sender, e);
                e.Handled = true;
            }
        }

        private void HistoryList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && HistoryList.SelectedIndices.Count > 0)
            {
                cmdDeleteHistory_Click(sender, e);
                e.Handled = true;
            }
        }


        bool bChangingColumnWidth = false;
        private void DownloadList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (!bChangingColumnWidth && e.ColumnIndex <= 1 && e.ColumnIndex < HistoryList.Columns.Count)
            {
                bChangingColumnWidth = true;
                HistoryList.Columns[e.ColumnIndex].Width = DownloadList.Columns[e.ColumnIndex].Width;
                bChangingColumnWidth = false;
            }
        }

        private void HistoryList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (!bChangingColumnWidth && e.ColumnIndex <= 1 && e.ColumnIndex < DownloadList.Columns.Count)
            {
                bChangingColumnWidth = true;
                DownloadList.Columns[e.ColumnIndex].Width = HistoryList.Columns[e.ColumnIndex].Width;
                bChangingColumnWidth = false;
            }
        }

        private void HistoryList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (HistoryList.SelectedIndices.Count > 0)
            {
                SABnzbdItem item = Global.SABnzbd.Status.HistoryItems[HistoryList.SelectedIndices[0]];

                string dir = item.Directory;
                while (!Directory.Exists(dir) && dir.Length > 3)
                    dir = Path.GetDirectoryName(dir);

                try
                {
                    System.Diagnostics.Process.Start(dir);
                }
                catch (Exception)
                {
                    MessageBox.Show("Path not found: " + item.Directory);
                }
            }
        }
    }
}
