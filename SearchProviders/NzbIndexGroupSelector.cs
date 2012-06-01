using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using System.Net;
using System.IO;

namespace NzbSearcher
{
    public partial class NzbIndexGroupSelector : Form
    {
        static List<NzbIndexSearchGroup> _SearchGroups = null;
        SearchSettings_NzbIndex _SearchParams;

        List<NzbIndexSearchGroup> _FilteredList;


        public NzbIndexGroupSelector(Point Location, SearchSettings_NzbIndex SearchParams)
        {
            _SearchParams = SearchParams;
            this.Location = Location;

            InitializeComponent();
        }

        private void NzbIndexGroupSelector_Load(object sender, EventArgs e)
        {
            this.Width = lstGroups.Right;
            this.Height = lstGroups.Bottom;
            
            if (LoadGroups())
            {
                BuildFilteredList();
                lstGroups.Columns.Add("group", lstGroups.Width - 30);
            }
        }

        
        void BuildFilteredList()
        {
            string search = txtFilter.Text.ToLower();

            _FilteredList = new List<NzbIndexSearchGroup>();
            foreach (NzbIndexSearchGroup grp in _SearchGroups)
                if (_SearchParams.SearchGroups.Contains(grp.Value) && grp.Name.Contains(search))
                    _FilteredList.Add(grp);
            foreach (NzbIndexSearchGroup grp in _SearchGroups)
                if (!_SearchParams.SearchGroups.Contains(grp.Value) && grp.Name.Contains(search))
                    _FilteredList.Add(grp);
            lstGroups.VirtualListSize = _FilteredList.Count;
        }

        private void NzbIndexGroupSelector_Activated(object sender, EventArgs e)
        {
            txtFilter.Focus();
        }

        private void NzbIndexGroupSelector_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        bool LoadGroups()
        {
            if (_SearchGroups == null)
            {
                try
                {
                    _SearchGroups = new List<NzbIndexSearchGroup>();
                    XmlDocument X = new XmlDocument();

                    HttpWebRequest SearchRequest = (HttpWebRequest)HttpWebRequest.Create("http://nzbindex.nl/?go=grouplist");
                    using (WebResponse resp = SearchRequest.GetResponse())
                        using (Stream respStream = resp.GetResponseStream())
                            using (StreamReader reader = new StreamReader(respStream))
                            {
                                //fix input not being closed! (otherwise invalid XML)
                                string xml = "<?xml version=\"1.0\" ?><groups>" +
                                    reader.ReadToEnd().Replace("><input", "") + "</groups>";
                                X.LoadXml(xml);
                            }

                    XmlNodeList labels = X.GetElementsByTagName("label");
                    foreach (XmlNode lbl in labels)
                    {
                        try
                        {
                            string name = lbl.InnerText;
                            int value = int.Parse(lbl.Attributes["value"].InnerText);
                            _SearchGroups.Add(new NzbIndexSearchGroup() { Name = name, Value = value });
                        }
                        catch { }
                    }
                }
                catch (Exception Exc)
                {
                    _SearchGroups = null;
                    _SearchParams.SearchGroups.Clear(); //we had an error retrieving the groups list, be sure to clear out items
                    MessageBox.Show("Error: " + Exc.Message);
                    this.Close();
                }
            }
            
            return _SearchGroups != null;
        }

        private void lstGroups_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            NzbIndexSearchGroup group = _FilteredList[e.ItemIndex];
            e.Item = new ListViewItem(group.Name);
            e.Item.Tag = group.Value;
            e.Item.Checked = true; //kludge! always set true first, so it will always display a checkbox 
            e.Item.Checked = _SearchParams.SearchGroups.Contains(group.Value);
        }

        //Kludge: because virtual list with checkboxes don't work right, we'll have to use the following code
        void lstGroups_MouseClick(object sender, MouseEventArgs e)
        {
            ListView lv = (ListView)sender;
            ListViewItem lvi = lv.GetItemAt(e.X, e.Y);
            if (lvi != null)
            {
                //if (e.X < (lvi.Bounds.Left + 16))
                {
                    SetItemChecked((int)lvi.Tag, !lvi.Checked);
                    //lstGroups_ItemChecked(sender, new ItemCheckedEventArgs(lvi));
                    lv.Invalidate(lvi.Bounds);
                }
            }
        }

        private void SetItemChecked(int Value, bool Checked)
        {
            if (Checked && !_SearchParams.SearchGroups.Contains(Value))
                _SearchParams.SearchGroups.Add(Value);
            else if (!Checked && _SearchParams.SearchGroups.Contains(Value))
                _SearchParams.SearchGroups.Remove(Value);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            BuildFilteredList();
            //lstGroups.RedrawItems(0, _FilteredList.Count - 1, false);
            lstGroups.Refresh();
        }

    }

    public class NzbIndexSearchGroup
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
