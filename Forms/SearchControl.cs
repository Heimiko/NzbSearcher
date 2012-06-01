using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.IO;

namespace NzbSearcher
{
    public partial class SearchControl : UserControl, ICustomTabControl
    {
        ISearchProvider _provider = null;
        string _ProvNameNoSpace;
        SearchSettings _LastSearch = null;

        public string DisplayName { get { return _provider.DisplayName; } }
        public string ToolTip { get { return _provider.ToolTip; } }
        public Image Icon { get { return _provider.Icon; } }
        public IGUIelementConfig Config { get { return _provider.Config; } }
        public ISearchProvider SearchProvider { get { return _provider; } }

        List<ListViewItem> _ListCache = new List<ListViewItem>();
        List<NzbItem> _FilteredList = null;

        Timer _FetchInfoTimer = new Timer();
        int _LastSplitterPos = -1;
        frmMessage _BusyMessage = null;

        //Controls
        ToolStripButton cmdFavorites = new ToolStripButton(Properties.Resources.favorites) { Text = "Favs", ToolTipText = "Show/Add Favorites" };
        ToolStripButton cmdIMDBmovies = new ToolStripButton(Properties.Resources.imdb2) { Text = "Movs", ToolTipText = "Show IMDB's My Movies" };
        ToolStripButton cmdInfo = new ToolStripButton(Properties.Resources.notepad) { Text = "NFO", ToolTipText = "Show/Hide NFO file for selected NZB (if available)" };
        ToolStripButton cmdNewSearch = new ToolStripButton(Properties.Resources._new) { Text = "Clear", ToolTipText = "Clear all search parameters and data, to start clean" };

        public ToolStripButton Button { get; set; }

        public SearchControl(ISearchProvider Provider)
        {
            InitializeComponent();
            _provider = Provider;
            _ProvNameNoSpace = _provider.HistoryName.Replace(' ', '_');

            NzbSearcher.Config.Saving += new ConfigEvent(Config_Saving);
            //Global.SABnzbd.StatusUpdated += new StatusUpdatedEvent(SABnzbd_StatusUpdated);
            Provider.SearchResultsUpdated += new ResultsUpdatedEvent(Provider_SearchResultsUpdated);

            cmdFavorites.Click += new EventHandler(cmdFavorites_Click);
            cmdIMDBmovies.Click += new EventHandler(cmdIMDBmovies_Click);
            cmdInfo.Click += new EventHandler(cmdInfo_Click);
            cmdNewSearch.Click +=new EventHandler(cmdNewSearch_Click);

            
        }

        void Config_Saving()
        {
            _provider.Config.SaveColumns(NzbResultList);

            _provider.Config.SearchHistory.Clear();
            foreach(string HistoryItem in cboSearch.Items)
                _provider.Config.SearchHistory.Add(HistoryItem);

            _provider.Config.Favorites.Clear();
            foreach (ToolStripItem favItem in mnuFavorites.Items)
                if (favItem.Tag is SearchSettings)
                    _provider.Config.Favorites.Add((SearchSettings)favItem.Tag);
        }

        object ListToArray(IList lst)
        {
            return lst.GetType().GetMethod("ToArray").Invoke(lst, null);
        }

        private void SearchControl_Load(object sender, EventArgs e)
        {
            _provider.Initialize();

            toolTips.SetToolTip(cmdSearch, "Search using current selected parameters");

            cboMaxDays.Enabled = _provider.SearchParams.SupportsMaxDays;
            lblMaxDays.Enabled = cboMaxDays.Enabled;
            txtMinSize.Enabled = _provider.SearchParams.SupportsMinSize;
            lblMinSize.Enabled = txtMinSize.Enabled;

            ProviderToScreen();

            //lblProvSpecific.Text = _provider.DisplayName + " Specific Controls:";
            //AdditionalControlsPanel.Left = lblProvSpecific.Right;

            NzbResultList.Columns.Add("Name", "NZB File", 400);
            NzbResultList.Columns.Add("Size", "Size", 70);
            NzbResultList.Columns.Add("Age", "Age", 70);
            NzbResultList.Columns.Add("Avail", "Availability", 70);
            NzbResultList.Columns.Add("Descr", "Description", 200);

            _provider.Config.LoadColumns(NzbResultList);

            foreach (string sh in _provider.Config.SearchHistory)
                cboSearch.Items.Add(sh);

            foreach (SearchSettings FavItem in _provider.Config.Favorites)
                AddFavorite(FavItem);
            
            SearchControl_Resize(sender, e);
            _provider.CreateAdditionalControls(AdditionalControlsPanel);

            _FetchInfoTimer.Interval = 500;
            _FetchInfoTimer.Tick += new EventHandler(FetchInfoTimer_Tick);
        }

        public void OnTabFocus()
        {
            
        }

        private void SearchControl_Resize(object sender, EventArgs e)
        {
            Size NewSize = this.ClientSize;

            if (ListSplitter.Visible)
            {
                txtInfo.Height = NewSize.Height - ListSplitter.Bottom;
                txtInfo.Top = ListSplitter.Bottom;
                txtInfo.Width = ListSplitter.Width;
            }
            else
            {
                NzbResultList.Height = NewSize.Height - NzbResultList.Top;
            }
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            ScreenToProvider();
            DoSearch();
        }

        /*
        void SABnzbd_StatusUpdated(SABnzbdStatus status)
        {
            if (cboSABcat.SelectedIndex == -1 && status.Categories.Count > 0)
            {
                RefillSABcategories();
                cboSABcat.Text = _provider.SearchParams.SABcat;
                if (cboSABcat.SelectedIndex == -1)
                    cboSABcat.SelectedIndex = 0;
            }
        }
        */

        void ScreenToProvider()
        {
            _provider.SearchParams.Text = cboSearch.Text;
            try { _provider.SearchParams.MaxDays = int.Parse(cboMaxDays.Text); }
            catch { _provider.SearchParams.MaxDays = 0; }
            try { _provider.SearchParams.MinSize = int.Parse(txtMinSize.Text); }
            catch { _provider.SearchParams.MinSize = 0; }

            _provider.SearchParams.MaxSize = 0; // set default 
            _provider.SearchParams.MinDays = 0; // set default

            //_provider.SearchParams.SABcat = cboSABcat.Text;
            _provider.ScreenToProvider();
        }

        void ProviderToScreen()
        {
            txtMinSize.Text = _provider.SearchParams.SupportsMinSize && _provider.SearchParams.MinSize > 0 ? _provider.SearchParams.MinSize.ToString() : string.Empty;
            cboMaxDays.Text = _provider.SearchParams.SupportsMaxDays && _provider.SearchParams.MaxDays > 0 ? _provider.SearchParams.MaxDays.ToString() : string.Empty;
            cboSearch.Text = _provider.SearchParams.Text;
            //cboSABcat.Text = _provider.SearchParams.SABcat;

            _provider.ProviderToScreen();
        }

        public void DoSearch(string SearchText)
        {
            cboSearch.Text = SearchText;
            ScreenToProvider();
            DoSearch();
        }

        void DoSearch()
        {
            _BusyMessage = frmMessage.Show("Searching, please wait...");
            
            _provider.ClearSearchResults();
            txtFilter.Text = string.Empty;   // will trigger a filter build, which is why we clear search results BEFORE doing this
            NzbResultList.VirtualListSize = 0; //at this point, should already be at 0, but just to be sure
            _FilteredList = null;           //should already be NULL at this point
            _ListCache.Clear();
            StartRefresh();

            if (_provider.SearchParams.Text != null && _provider.SearchParams.Text.Length > 0)
            {
                //add item to top of list in combobox
                if (cboSearch.Items.Contains(_provider.SearchParams.Text))
                    cboSearch.Items.Remove(_provider.SearchParams.Text);

                cboSearch.Items.Insert(0, _provider.SearchParams.Text);
                cboSearch.SelectedIndex = 0;

                while (cboSearch.Items.Count > 20) //keep total history items below 20
                    cboSearch.Items.RemoveAt(cboSearch.Items.Count - 1);
            }

            _provider.Search();
        }

        void Provider_SearchResultsUpdated(ISearchProvider provider, UpdateReason Reason, string ErrorMessage)
        {
            this.Cursor = Cursors.Default;

            if (_BusyMessage != null)
            {
                _BusyMessage.Dispose();
                _BusyMessage = null;
            }

            //using (new LockWindowUpdate(NzbResultList.Handle))
            {
                BuildFilteredListIfNeeded();
                NzbResultList.Refresh();
            }
            
            switch (Reason)
            {
                case UpdateReason.NewSearch:
                    _ListCache.Clear();
                    NzbResultList.Refresh();
                    if (ErrorMessage == null && provider.SearchResultItemCount == 0)
                        ErrorMessage = "No items found with current search criteria";
                    if (ErrorMessage == null)
                        NzbResultList.Focus(); //move focus to list
                    break;
                
                case UpdateReason.MoreSearch:
                    //no refreshing of any kind
                    break;

                case UpdateReason.Sorting:
                    _ListCache.Clear();
                    NzbResultList.Refresh();
                    break;
            }

            if (ErrorMessage != null && ErrorMessage.Length > 0)
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void StartRefresh()
        {
            this.Cursor = Cursors.WaitCursor;
            //this.Enabled = false;
            //this.Refresh();
        }

        bool IsListViewItemVisible(ListView lstView, ListViewItem lstItem)
        {
            Rectangle MainRect = NzbResultList.ClientRectangle; 
            if (MainRect.Contains(lstItem.Position))
                return true;
            return MainRect.IntersectsWith(lstItem.Bounds);
        }

        private void NzbResultList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            NzbItem ResultItem;
            
            if (_FilteredList == null)
            {
                if (e.ItemIndex >= _provider.SearchResultItemCount)
                    throw new Exception("Search result index out of bounds");

                if (e.ItemIndex < _ListCache.Count && _ListCache[e.ItemIndex] != null)
                {
                    e.Item = _ListCache[e.ItemIndex];
                    return;
                }

                bool FetchMore = e.ItemIndex > 2 && e.ItemIndex >= _provider.SearchResultItemCount - 2 &&
                                    (e.ItemIndex - 1) < _ListCache.Count && _ListCache[e.ItemIndex - 1] != null &&
                                        IsListViewItemVisible(NzbResultList, _ListCache[e.ItemIndex - 1]);
                if (FetchMore)
                    _provider.SearchMore();

                ResultItem = _provider[e.ItemIndex];
            }
            else
            {
                if (e.ItemIndex >= _FilteredList.Count)
                    throw new Exception("Filtered result index out of bounds");

                ResultItem = _FilteredList[e.ItemIndex];
            }

            if (ResultItem.HasBeenDownloaded)
                UpdateLatestDownload(ResultItem);
            
            ListViewItem lvItem = new ListViewItem(ResultItem.Title)
            {
                Tag = ResultItem,
                BackColor = ResultItem.BackgroundColor
            };

            lvItem.SubItems.Add(ResultItem.Size);
            lvItem.SubItems.Add(ResultItem.Age);
            lvItem.SubItems.Add(ResultItem.PercentAvailable + "%");
            lvItem.SubItems.Add(ResultItem.Description);

            e.Item = lvItem;

            if (_FilteredList == null)
            {
                //Grow the list if too small
                while (_ListCache.Count < e.ItemIndex)
                    _ListCache.Add(null);

                if (_ListCache.Count == e.ItemIndex)
                    _ListCache.Add(lvItem);
                else
                    _ListCache[e.ItemIndex] = lvItem;
            }
        }

        NzbItem GetSelectedItem()
        {
            if (NzbResultList.SelectedIndices.Count > 0)
                return _FilteredList != null ? _FilteredList[NzbResultList.SelectedIndices[0]] : _provider[NzbResultList.SelectedIndices[0]];

            return null;
        }

        private void NzbResultList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelectedItemsToQueue();
        }

        private void NzbResultList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                AddSelectedItemsToQueue();
            }
        }

        void AddSelectedItemsToQueue()
        {
            if (NzbResultList.SelectedIndices.Count > 0)
            {
                List<NzbItem> SelectedItems = new List<NzbItem>();
                if (_FilteredList != null)
                {
                    foreach (int itemIndex in NzbResultList.SelectedIndices)
                        SelectedItems.Add(_FilteredList[itemIndex]);
                }
                else
                {
                    foreach (int itemIndex in NzbResultList.SelectedIndices)
                        SelectedItems.Add(_provider[itemIndex]);
                }

                frmAddNZB frm = new frmAddNZB(_provider, SelectedItems);
                DialogResult res = frm.ShowDialog();
                if (res == DialogResult.OK)
                {
                    if (_FilteredList == null)
                    {
                        foreach (int itemIndex in NzbResultList.SelectedIndices)
                            _ListCache[itemIndex].BackColor = _provider[itemIndex].BackgroundColor;
                    }
                    else
                        NzbResultList.Refresh();

                    foreach (NzbItem item in SelectedItems)
                        UpdateLatestDownload(item);

                    //cboSABcat.Text = frm.SABcat;
                }
            }
        }

        void UpdateLatestDownload(NzbItem item)
        {
            long PostedTicks = item.PostedDate.Ticks;
            if (_provider.SearchParams.LatestDownload < PostedTicks)
            {
                _provider.SearchParams.LatestDownload = PostedTicks;
                bool HasBeenUpdated = false;
                SearchSettings parms;
                
                foreach (ToolStripItem mnuItem in mnuFavorites.Items)
                {
                    if ((parms = mnuItem.Tag as SearchSettings) != null)
                    {
                        if (parms.LatestDownload < _provider.SearchParams.LatestDownload && parms.Text == _provider.SearchParams.Text)
                        {
                            parms.LatestDownload = _provider.SearchParams.LatestDownload;
                            HasBeenUpdated = true;
                        }
                    }
                }

                if (HasBeenUpdated)
                    NzbSearcher.Config.Save();
            }
        }

        private void txtMinSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 20 && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
            else
                cboSearch_KeyPress(sender, e);
        }

        private void cboMaxDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 20 && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
            else
                cboSearch_KeyPress(sender, e);
        }

        private void NzbResultList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortColumn CurrentCol = _provider.GetSorting();
            SortColumn NewCol = SortColumn.Age;

            switch (NzbResultList.Columns[e.Column].Name)
            {
                case "Name": NewCol = SortColumn.FileName; break; // file name
                case "Descr": NewCol = SortColumn.Description; break; // description
                case "Age": NewCol = SortColumn.Age; break; //age
                case "Size": NewCol = SortColumn.Size; break; //size
            }

            if (((int)CurrentCol & 0xF) == (int)NewCol)
                NewCol = CurrentCol ^ SortColumn.Descending;

            NzbResultList.VirtualListSize = 0;
            StartRefresh();
            _provider.SetSorting(NewCol);
        }

        private void cboSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // on ENTER -> search
            {
                cmdSearch_Click(sender, e);
                e.Handled = true;
            }
        }

        private void cmdFavorites_Click(object sender, EventArgs e)
        {
            cmdFavorites.Checked = true;

            //Update all favorite images to reflect the latest download
            SearchSettings parms;
            foreach (ToolStripItem item in mnuFavorites.Items)
            {
                if ((parms = item.Tag as SearchSettings) != null)
                {
                    long NowTicks = DateTime.Now.Ticks;

                    if (parms.LatestDownload + (new TimeSpan(6, 0, 0, 0).Ticks) > NowTicks)
                        item.Image = null; // no image if too young 
                    else if (parms.LatestDownload + (new TimeSpan(8, 0, 0, 0).Ticks) > NowTicks)
                        item.Image = Properties.Resources.favorites_green;
                    else if (parms.LatestDownload + (new TimeSpan(15, 0, 0, 0).Ticks) > NowTicks)
                        item.Image = Properties.Resources.favorites_red;
                    else
                        item.Image = null; //bloody old, no image
                }
            }

            mnuFavorites.Show(cmdFavorites.Owner, cmdFavorites.Bounds.Left, cmdFavorites.Bounds.Bottom);
        }

        private void mnuFavorites_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            cmdFavorites.Checked = false;
        }

        void cmdIMDBmovies_Click(object sender, EventArgs e)
        {
            cmdIMDBmovies.Checked = true;
            
            if (txtIMDBfilter.Text != "<Filter>")
                txtIMDBfilter.Text = "<Filter>";  //reset item text to "filter" -> should rebuild menu 
            else
                Global.IMDB.ReBuildMenu(mnuIMDBmovies, mnuEditMov, txtIMDBfilter.Text);
            
            mnuIMDBmovies.Show(cmdIMDBmovies.Owner, cmdIMDBmovies.Bounds.Left, cmdIMDBmovies.Bounds.Bottom);

            //txtIMDBfilter.Focus();
        }

        private void mnuIMDBmovies_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange)
            {
                e.Cancel = true;

            }

            cmdIMDBmovies.Checked = false;
        }

        private void cmdRefreshMyMovies_Click(object sender, EventArgs e)
        {
            Global.IMDB.RefreshMyMovies();
            cmdIMDBmovies_Click(sender, e);
        }

        private void cmdAddFavorite_Click(object sender, EventArgs e)
        {
            ScreenToProvider();

            SearchSettings SearchParams = _provider.SearchParams;
            string DisplayName = SearchParams.Text;
            if (InputBox.Show("Add Favorite", "Favorite Name:", ref DisplayName) == DialogResult.OK)
            {
                SearchParams.DisplayName = DisplayName;
                
                //See if we already have a favorite with the same name
                foreach (ToolStripItem item in mnuFavorites.Items)
                {
                    if (item.Tag is SearchSettings && ((SearchSettings)item.Tag).DisplayName.ToLower() == DisplayName.ToLower())
                    {
                        if (MessageBox.Show("Favorite '" + DisplayName + "' already exists, do you want to replace it?", "NzbSeachter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            item.Tag = SearchParams.Clone();

                        NzbSearcher.Config.Save();
                        return; // done
                    }
                }

                AddFavorite(SearchParams.Clone());
                NzbSearcher.Config.Save();
            }
        }

        void AddFavorite(SearchSettings SearchParams)
        {
            if (SearchParams.DisplayName == null || SearchParams.DisplayName.Length == 0)
                SearchParams.DisplayName = SearchParams.Text;

            //Add the favorite at the right place (alphabetical)
            for (int i = mnuFavorites.Items.Count - 1; i > 0; i--)
            {
                if (!(mnuFavorites.Items[i].Tag is SearchSettings) ||
                    mnuFavorites.Items[i].Text.CompareTo(SearchParams.DisplayName) < 0)
                {
                    ToolStripMenuItem FavItem = new ToolStripMenuItem(SearchParams.DisplayName);
                    FavItem.Tag = SearchParams;
                    FavItem.DropDown = mnuEditFav;

                    mnuFavorites.Items.Insert(i + 1, FavItem);
                    break; //done
                }
            }
        }

        void RemoveFavorite(SearchSettings SearchParams)
        {
            for (int i = mnuFavorites.Items.Count - 1; i > 0; i--)
            {
                if (mnuFavorites.Items[i].Tag == SearchParams)
                    mnuFavorites.Items.RemoveAt(i);
            }
        }

        private void mnuFavorites_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is SearchSettings)
            {
                mnuFavorites.Close();
                _provider.SearchParams = ((SearchSettings)e.ClickedItem.Tag).Clone();
                _LastSearch = _provider.SearchParams;
                ProviderToScreen();
                DoSearch();
            }
        }

        private void mnuEditFav_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SearchSettings SearchParams = (SearchSettings)e.ClickedItem.OwnerItem.Tag;

            switch (e.ClickedItem.Text)
            {
                case "Search":
                    mnuFavorites_ItemClicked(sender, new ToolStripItemClickedEventArgs(e.ClickedItem.OwnerItem));
                    break;
                case "Rename":
                    string DisplayName = SearchParams.DisplayName;
                    if (InputBox.Show("Rename Favorite", "Favorite Name:", ref DisplayName) == DialogResult.OK)
                    {
                        RemoveFavorite(SearchParams);
                        SearchParams.DisplayName = DisplayName;
                        AddFavorite(SearchParams);
                        NzbSearcher.Config.Save();
                    }
                    break;
                case "Change":
                    string SearchText = SearchParams.Text;
                    if (InputBox.Show("Rename Favorite", "Favorite Name:", ref SearchText) == DialogResult.OK)
                    {
                        SearchParams.Text = SearchText;
                        NzbSearcher.Config.Save();
                    }
                    break;

                case "Update":
                    RemoveFavorite(SearchParams);
                    ScreenToProvider();
                    _provider.SearchParams.DisplayName = SearchParams.DisplayName;
                    AddFavorite(_provider.SearchParams.Clone());
                    NzbSearcher.Config.Save();
                    break;

                case "Delete":
                    mnuFavorites.Items.Remove(e.ClickedItem.OwnerItem);
                    NzbSearcher.Config.Save();
                    break;


            }

            mnuFavorites.Close();
        }
        
        /*
        private void cboSABcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            _provider.SearchParams.SABcat = cboSABcat.Text;
        }

        void RefillSABcategories()
        {
            cboSABcat.BeginUpdate();
            string SelItem = (string)cboSABcat.SelectedItem;
            cboSABcat.Items.Clear();
            foreach (string cat in Global.SABnzbd.Status.Categories)
                cboSABcat.Items.Add(cat);
            if (SelItem != null && SelItem.Length > 0)
                cboSABcat.SelectedItem = SelItem;
            cboSABcat.EndUpdate();
        }

        private void cboSABcat_DropDown(object sender, EventArgs e)
        {
            RefillSABcategories();
        }
        */

        private void ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem)sender).Text)
            {
                case "Search":
                    Program.MainForm.SetStatus("Search using currently selected favorite");
                    break;
                case "Rename":
                    Program.MainForm.SetStatus("Rename current selected favorite");
                    break;
                case "Update":
                    Program.MainForm.SetStatus("Update selected favorite from currently active search parameters");
                    break;
                case "Delete":
                    Program.MainForm.SetStatus("Delete current selected favorite");
                    break;
            }
        }

        private void mnuEditFav_Opening(object sender, CancelEventArgs e)
        {
            SearchSettings FavSearch = ((ContextMenuStrip)sender).OwnerItem.Tag as SearchSettings;
            updateToolStripMenuItem.Enabled = FavSearch == null || _LastSearch == null ? false : 
                                                FavSearch.DisplayName == _LastSearch.DisplayName;
        }

        private void cmdNewSearch_Click(object sender, EventArgs e)
        {
            _provider.NewSearchParams();
            txtFilter.Text = string.Empty;
            NzbResultList.VirtualListSize = 0;
            _ListCache.Clear();
            ProviderToScreen();
            cboSearch.Focus();
        }

        private void Form_MouseLeave(object sender, EventArgs e)
        {
            Program.MainForm.SetStatus(string.Empty);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            BuildFilteredListIfNeeded();
            NzbResultList.Refresh();
        }

        bool BuildFilteredListIfNeeded()
        {
            string[] filters = txtFilter.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (filters.Length == 0)
            {
                _FilteredList = null;
                NzbResultList.VirtualListSize = _provider.SearchResultItemCount;
                return false;
            }

            _FilteredList = new List<NzbItem>();

            for (int i = 0; i < _provider.SearchResultItemCount; i++)
            {
                NzbItem item = _provider[i];
                int i2 = 0;
                for (; i2 < filters.Length; i2++)
                {
                    string filter = filters[i2];

                    if (filter[0] == '-')
                    {
                        if (item.Title.IndexOf(filter.Substring(1), StringComparison.OrdinalIgnoreCase) >= 0)
                            break;
                    }
                    else if (item.Title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                        break;
                }

                if (i2 == filters.Length)
                    _FilteredList.Add(item);
            }

            NzbResultList.VirtualListSize = _FilteredList.Count;
            return true;
        }

        private void ListSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (e.Y >= this.ClientSize.Height - SearchPanel.Height)
            {
                ListSplitter.Visible = false;
                cmdInfo.Checked = false;
            }
            else
            {
                if (_LastSplitterPos < NzbResultList.Height)
                    NzbResultList.Focus();
                else
                {
                    txtInfo.Focus();
                    txtInfo.SelectionLength = 0;
                }

                _LastSplitterPos = NzbResultList.Height;
            }

            SearchControl_Resize(sender, e);

            if (!ListSplitter.Visible)
                NzbResultList.Focus();
        }

        void cmdInfo_Click(object sender, EventArgs e)
        {
            cmdInfo.Checked = !cmdInfo.Checked;

            NzbResultList.Focus();
            FetchInfoTimer_Tick(sender, e);
            if (ListSplitter.Visible)
            {
                txtInfo.Focus();
                txtInfo.SelectionLength = 0;
            }
        }

        private void NzbResultList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _FetchInfoTimer.Stop();
            txtInfo.BackColor = this.BackColor;
            txtInfo.Clear();

            if (cmdInfo.Checked)
                _FetchInfoTimer.Start();
        }

        void FetchInfoTimer_Tick(object sender, EventArgs e)
        {
            _FetchInfoTimer.Stop();
            string strInfo = null;

            if (cmdInfo.Checked)
            {
                NzbItem itm = GetSelectedItem();
                if (itm != null)
                    strInfo = _provider.GetNzbInfo(itm);
            }

            if (strInfo != null && strInfo.Length > 0)
            {
                txtInfo.BackColor = NzbResultList.BackColor;
                txtInfo.Text = strInfo;
                if (!ListSplitter.Visible)
                {
                    ListSplitter.Visible = true;
                    if (_LastSplitterPos < 0 || _LastSplitterPos > this.Height)
                        _LastSplitterPos = this.Height / 2;
                    NzbResultList.Height = _LastSplitterPos;
                    SearchControl_Resize(sender, e);
                }
            }
            else
            {
                //no info file -> put away info screen
                ListSplitter.Visible = false;
                SearchControl_Resize(sender, e);
                NzbResultList.Focus();
            }
        }

        private void cmdAddToQueue_Click(object sender, EventArgs e)
        {
            AddSelectedItemsToQueue();
        }

        private void cmdIMDB_Click(object sender, EventArgs e)
        {
            NzbItem itm = GetSelectedItem();
            string URL = null;
            string SearchIMDB = null;
            if (itm != null)
                URL = _provider.GetIMDBurl(itm);

            if (URL == null)
            {
                if (itm != null)
                    SearchIMDB = Global.RemoveNonAlfaNumeric(itm.Title);

                DialogResult res = InputBox.Show("NzbSearcher", "Search IMDB:", ref SearchIMDB);
                if (res != DialogResult.OK)
                    return; //no searching
            }

            if (URL != null)
                System.Diagnostics.Process.Start(URL);
            else if (SearchIMDB != null && SearchIMDB.Length > 0)
                System.Diagnostics.Process.Start("http://www.imdb.com/find?q=" + SearchIMDB);
            else
                System.Diagnostics.Process.Start("http://www.imdb.com/");
        }

        private void cmdShowNFO_Click(object sender, EventArgs e)
        {
            NzbItem itm = GetSelectedItem();
            if (itm != null && itm.HasNFO)
            {
                string NFO = _provider.GetNzbInfo(itm);
                if (NFO != null && NFO.Length > 0)
                {
                    string FileName = Path.GetFileName(itm.DownloadLink) + ".txt";
                    string TempFile = Path.Combine(Path.GetTempPath(), FileName);
                    if (!File.Exists(TempFile))
                        using (FileStream fs = File.Open(TempFile, FileMode.CreateNew))
                            using (StreamWriter writer = new StreamWriter(fs))
                                writer.Write(NFO);

                    System.Diagnostics.Process.Start("notepad.exe", TempFile);
                }
            }
        }

        private void NzbResultList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && GetSelectedItem() != null)
                mnuNzbContext.Show(NzbResultList, e.Location);
        }

        public void OnKeyPreview(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.F10:
                        cmdIMDB_Click(sender, e);
                        e.Handled = true;
                        break;
                    case Keys.F11:
                        cmdShowNFO_Click(sender, e);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void mnuNzbContext_Opening(object sender, CancelEventArgs e)
        {
            NzbItem itm = GetSelectedItem();
            cmdShowNFO.Enabled = itm != null && itm.HasNFO;
        }

        public void AddToolbarItems(ToolStripItemCollection col)
        {
            col.Add(cmdFavorites);
            
            if (Global.IMDB.Config.DisplayMyMovs)
                col.Add(cmdIMDBmovies);

            col.Add(cmdInfo);
            col.Add(cmdNewSearch);

            if (Global.Config.SearchInToolbar)
            {
                SearchPanel.Dock = DockStyle.None;
                if (this.Controls.Contains(SearchPanel))
                    this.Controls.Remove(SearchPanel);

                col.Add(new ToolStripControlHost(SearchPanel));
            }
            else
            {
                SearchPanel.Dock = DockStyle.Top;
                if (!this.Controls.Contains(SearchPanel))
                    this.Controls.Add(SearchPanel);
            }
        }

        private void mnuIMDBmovies_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            IMDBmovie mov = e.ClickedItem.Tag as IMDBmovie;
            if (mov != null)
            {
                cboSearch.Text = Global.RemoveNonAlfaNumeric(mov.Title);
                if (Global.IMDB.Config.IncludeYearOnSearch)
                    cboSearch.Text += " " + mov.Year; 
                
                cmdSearch_Click(sender, e);
                mnuIMDBmovies.Close();
            }
        }

        private void cmdManageMyMovies_Click(object sender, EventArgs e)
        {
            Global.IMDB.ManageMyMovies();
        }

        private void mnuEditMov_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            IMDBmovie mov = e.ClickedItem.OwnerItem.Tag as IMDBmovie;
            string ItemAction = e.ClickedItem.Text;
            ItemAction = ItemAction.IndexOf(' ') >= 0 ? ItemAction.Substring(0, ItemAction.IndexOf(' ')) : ItemAction;

            switch (ItemAction)
            {
                case "Search":
                    mnuIMDBmovies_ItemClicked(sender, new ToolStripItemClickedEventArgs(e.ClickedItem.OwnerItem));
                    break;
                case "Open":
                    System.Diagnostics.Process.Start("http://www.imdb.com/title/" + mov.ID);
                    break;

                case "Delete":
                    Global.IMDB.DeleteMovie(mov);
                    break;

                default:
                    //undefined item -> no action
                    return;
            }

            //After defined action, always close menu
            mnuIMDBmovies.Close();
        }

        private void txtIMDBfilter_TextChanged(object sender, EventArgs e)
        {
            mnuIMDBmovies.Tag = null; //need re-do
            Global.IMDB.ReBuildMenu(mnuIMDBmovies, mnuEditMov, txtIMDBfilter.Text);
        }
    }




}
