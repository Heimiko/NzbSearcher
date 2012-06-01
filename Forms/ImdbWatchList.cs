using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NzbSearcher
{
    public delegate void WatchListItem_DoubleClick_Event(WatchListItem item);

    public partial class ImdbWatchList : UserControl
    {
        public event WatchListItem_DoubleClick_Event WatchListItem_DoubleClick;

        int _AddWidth;
        bool _bInitialized = false;

        public ImdbWatchList()
        {
            InitializeComponent();

            _AddWidth = toolStrip1.Width - cboMyLists.Width;
        }

        private void ImdbWatchList_VisibleChanged(object sender, EventArgs e)
        {
            //load config on visibility changed, and only when not initialized yet or NOT actually running!
            if (this.Visible && !_bInitialized) 
            {
                _bInitialized = true;
                using (frmMessage.Show("Refreshing stuff..."))
                    RebuildListsCombo();
            }
        }

        void RebuildListsCombo()
        {
            WatchListDefenition selectedList = cboMyLists.SelectedItem as WatchListDefenition;
            cboMyLists.Items.Clear();

            if (Global.ImdbWatchList.MyWatchLists != null)
                foreach (WatchListDefenition list in Global.ImdbWatchList.MyWatchLists)
                    cboMyLists.Items.Add(list);

            if (Global.ImdbWatchList.AdditionalWatchLists != null)
                foreach (WatchListDefenition list in Global.ImdbWatchList.AdditionalWatchLists)
                    cboMyLists.Items.Add(list);

            if (selectedList != null)
                foreach(WatchListDefenition list in cboMyLists.Items)
                    if (list.ListID == selectedList.ListID)
                    {
                        cboMyLists.SelectedItem = list; //will do a rebuild of the actual list
                        break;
                    }
            if (cboMyLists.SelectedIndex < 0 && cboMyLists.Items.Count > 0)
                cboMyLists.SelectedIndex = 0; //will do a rebuild of the actual list
        }

        private void ImdbWatchList_Resize(object sender, EventArgs e)
        {
            lstWatchList.Height = this.Height - 51; //make same height as list in SearchControl
            cboMyLists.Width = this.Width - _AddWidth;
        }

        void RebuildList()
        {
            lstWatchList.Items.Clear();

            WatchListDefenition list = cboMyLists.SelectedItem as WatchListDefenition;
            if (list != null)
            {
                WitchListItemsCollection Items = Global.ImdbWatchList[list.ListID];
                if (Items != null)
                {
                    foreach (WatchListItem item in Items)
                    {
                        ListViewItem NewItem = new ListViewItem(item.title);
                        NewItem.SubItems.Add(item.year);
                        NewItem.SubItems.Add(item.imdb_rating);
                        NewItem.Tag = item;
                        lstWatchList.Items.Add(NewItem);
                    }
                }
            }
        }

        public WatchListItem GetSelectedItem()
        {
            if (lstWatchList.SelectedItems.Count > 0)
                return lstWatchList.SelectedItems[0].Tag as WatchListItem;

            return null;
        }

        private void lstWatchList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WatchListItem_DoubleClick != null)
            {
                WatchListItem item = GetSelectedItem();
                if (item != null)
                    WatchListItem_DoubleClick(item);
            }
        }

        private void cmdManageList_Click(object sender, EventArgs e)
        {
            WatchListDefenition list = cboMyLists.SelectedItem as WatchListDefenition;
            if (list != null)
                System.Diagnostics.Process.Start("http://www.imdb.com/list/" + list.ListID + "?view=compact&sort=listorian:asc");
        }

        private void lstWatchList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && GetSelectedItem() != null)
                mnuContext.Show(lstWatchList, e.Location);
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            lstWatchList_MouseDoubleClick(sender, null);
        }

        private void cmdOpenOnIMDB_Click(object sender, EventArgs e)
        {
            WatchListItem item = GetSelectedItem();
            if (item != null)
                System.Diagnostics.Process.Start("http://www.imdb.com/title/" + item.const_);
        }

        private void cmdDeleteItem_Click(object sender, EventArgs e)
        {
            WatchListDefenition list = cboMyLists.SelectedItem as WatchListDefenition;
            if (list != null)
            {
                WatchListItem item = GetSelectedItem();
                if (item != null)
                {
                    using (frmMessage.Show("Deleting movie from list, please wait..."))
                    {
                        if (Global.ImdbWatchList.DeleteMovie(list.ListID, item))
                        {
                            //remove selected item from list
                            lstWatchList.Items.Remove(lstWatchList.SelectedItems[0]);
                        }
                        else
                            MessageBox.Show("Couldn't delete movie from list");
                    }
                }
            }
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            using (frmMessage.Show("Refreshing stuff..."))
            {
                Global.ImdbWatchList.RefreshMyWatchListDefenitions();

                WatchListDefenition list = cboMyLists.SelectedItem as WatchListDefenition;
                if (list != null)
                    Global.ImdbWatchList.RefreshWatchList(list.ListID);

                RebuildListsCombo(); //will automatically do a rebuild of the lst itself
            }
        }

        private void cmdAddList_Click(object sender, EventArgs e)
        {
            string ListID = string.Empty;
            if (InputBox.Show("NzbSearcher", "Please enter the List ID - check URL of the list you wish to add", ref ListID) == DialogResult.Cancel)
                return;

            try
            {
                string Name = Global.ImdbWatchList.GetListTitle(ListID);
                if (InputBox.Show("NzbSearcher", "Please enter the name for the list", ref Name) == DialogResult.Cancel)
                    return;

                WatchListDefenition list = new WatchListDefenition();
                list.Name = Name;
                list.ListID = ListID; // "nORduzyVDnM";

                Global.ImdbWatchList.AdditionalWatchLists.Add(list);
                cboMyLists.Items.Add(list);
                cboMyLists.SelectedItem = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void cmdDeleteList_Click(object sender, EventArgs e)
        {
            if (Global.ImdbWatchList.MyWatchLists != null && cboMyLists.SelectedIndex >= Global.ImdbWatchList.MyWatchLists.Count) //can't delete item 0
            {
                WatchListDefenition list = cboMyLists.SelectedItem as WatchListDefenition;
                if (list != null)
                {
                    cboMyLists.SelectedIndex = 0; //first select different item, before deleting stuff
                    Global.ImdbWatchList.AdditionalWatchLists.Remove(list);
                    cboMyLists.Items.Remove(list);
                }
            }
        }

        private void cboMyLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdDeleteList.Enabled = Global.ImdbWatchList.MyWatchLists != null && cboMyLists.SelectedIndex >= Global.ImdbWatchList.MyWatchLists.Count;
            RebuildList();
        }

        private void lstWatchList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            WatchListDefenition list = cboMyLists.SelectedItem as WatchListDefenition;
            if (list != null)
            {
                WitchListItemsCollection Items = Global.ImdbWatchList[list.ListID];
                if (Items != null)
                {
                    switch (e.Column)
                    {
                        case 0: Items.Sort("title"); break;
                        case 1: Items.Sort("year"); break;
                        case 2: Items.Sort("imdb_rating"); break;
                    }
                    RebuildList();
                }
            }
        }

        private void lstWatchList_KeyUp(object sender, KeyEventArgs e)
        {
            bool Handled = true;

            switch (e.KeyCode)
            {
                case Keys.Delete:
                    cmdDeleteItem_Click(sender, e);
                    break;
                case Keys.F10:
                    cmdOpenOnIMDB_Click(sender, e);
                    break;
                case Keys.Enter:
                    lstWatchList_MouseDoubleClick(sender, null);
                    break;
                default:
                    Handled = false;
                    break;
            }

            e.Handled = Handled;
        }
    }
}
