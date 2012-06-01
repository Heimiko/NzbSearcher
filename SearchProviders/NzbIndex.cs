using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Drawing;
using System.IO;

using System.Collections;
using System.Windows.Forms;
using System.Threading;

using Heimiko;

namespace NzbSearcher
{
    public class NzbIndexConfig : ProviderConfig<SearchSettings_NzbIndex>
    {

    }

    public class NzbItem_NzbIndex : NzbItem
    {
        public override bool HasNFO { get { return Description != null && Description.Contains("nfo"); } }
        public string NFO { get; set; }
    }

    public class SearchGroupsCollection : List<int> { }

    [Serializable]
    public class SearchSettings_NzbIndex : SearchSettings
    {
        public SearchSettings_NzbIndex()
        {
            SortColumn = NzbSearcher.SortColumn.Age;
            SearchGroups = new SearchGroupsCollection();
        }

        public SearchGroupsCollection SearchGroups { get; set; }
        public SortColumn SortColumn { get; set; }
    }

    public class NzbIndex : SearchProviderBase
    {
        //Label lblMaxItems = new Label { Text = "Max.Items:", AutoSize = true, Margin = new Padding(0,7,0,0) };
        //ComboBox cboMaxItems = new ComboBox() { Width = 50, DropDownStyle = ComboBoxStyle.DropDownList };
        Button cmdGroups = new Button() { Text = "Specify Groups", AutoSize = false, Width = 88, Height = 23, Margin = new Padding(0) };
        SearchSettings_NzbIndex _SearchParams = new SearchSettings_NzbIndex();

        //Properties
        public override Image Icon { get { return Properties.Resources.nzb.ToBitmap(); } }
        public override string Name { get { return "NZBIndex.nl"; } }
        public override string DisplayName { get { return "Index"; } }
        public override string ToolTip { get { return "NZBIndex.nl Search Provider"; } }
        public override string HistoryName { get { return "NZBIndex"; } }

        public override SearchSettings SearchParams { get { return _SearchParams; } set { _SearchParams = (SearchSettings_NzbIndex)value; }  }
        public override bool IsItemFromProvider(NzbItem item) { return item is NzbItem_NzbIndex; }
        public override IProviderConfig Config { get { return Config<NzbIndexConfig>.Value; } }

        protected override void Search(object bNextPage)
        {
            string ErrorMessage = null;
            
            _bMoreAvailable = false;

            try
            {
                if (SearchParams == null || SearchParams.Text == null)
                    return;

                HttpWebRequest SearchRequest = (HttpWebRequest)HttpWebRequest.Create(GetSearchURL(bNextPage != null));
                SearchRequest.Timeout = WebTextBrowser.Timeout;

                //1st, add language cookie (otherwise we get freakish dutch!)
                SearchRequest.CookieContainer = new CookieContainer();
                SearchRequest.CookieContainer.Add(new Cookie("lang", "2", "/", "nzbindex.nl"));

                if (bNextPage == null) //Start with new search? -> new result list
                    _SearchResult.Clear();

                XmlDocument X = new XmlDocument();

                using (WebResponse resp = SearchRequest.GetResponse())
                using (Stream respStream = resp.GetResponseStream())
                using (StreamReader reader = new StreamReader(respStream))
                    X.LoadXml(reader.ReadToEnd());

                XmlNodeList Items = X.GetElementsByTagName("item");
                foreach (XmlNode Item in Items)
                {
                    try
                    {
                        NzbItem_NzbIndex NewItem = new NzbItem_NzbIndex();
                        string description = Item.SelectSingleNode("description").InnerText;

                        NewItem.Title = Item.SelectSingleNode("title").InnerText;
                        NewItem.Description = GetDescriptionPart(description, "<font", 3).ToLower();

                        string Parts = GetDescriptionPart(description, "<font", 1).ToLower();
                        Parts = Parts.Substring(Parts.IndexOf('('));
                        NewItem.Description += " " + Parts;

                        NewItem.DownloadLink = Item.SelectSingleNode("enclosure").Attributes["url"].InnerText;
                        NewItem.Size = GetDescriptionPart(description, "<b>", 0);
                        NewItem.Age = GetDescriptionPart(description, "<br />", 1);
                        NewItem.UniqueID = GetUniqueID(NewItem.DownloadLink);
                        NewItem.PostedDate = DateTime.Parse(Item.SelectSingleNode("pubDate").InnerText);

                        NewItem.HasBeenDownloaded = _History.ContainsHistory(NewItem.UniqueID);
                        NewItem.HasMissingParts = Parts.Contains("miss");

                        if (NewItem.HasMissingParts)
                        {
                            string[] MissingParts = Parts.Replace("parts", string.Empty).Replace("missing", string.Empty).Trim(' ', '(', ')').Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (MissingParts.Length == 2)
                                NewItem.PercentAvailable = 99 - ((int.Parse(MissingParts[1]) * 100) / int.Parse(MissingParts[0]));
                        }
                        else
                            NewItem.PercentAvailable = 100;

                        _SearchResult.Add(NewItem);
                    }
                    catch (Exception)
                    {

                    }
                }

                _bMoreAvailable = (Items.Count == Global.Config.ItemsPerPage);
            }
            catch (Exception exc)
            {
                ErrorMessage = exc.Message;
            }
            finally
            {
                UpdateReason Reason = bNextPage == null ? UpdateReason.NewSearch : UpdateReason.MoreSearch;
                FireResultsUpdated(Reason, ErrorMessage);
            }
        }

        public override string GetNzbInfo(NzbItem item)
        {
            NzbItem_NzbIndex Nzb = item as NzbItem_NzbIndex;
            if (Nzb == null)
                return null;
            if (Nzb.NFO != null)
                return Nzb.NFO;
            if (!Nzb.HasNFO)
                return null;

            string NFOlink = Nzb.DownloadLink.Replace("/download/", "/nfo/");

            try
            {
                string HTML = null;

                HttpWebRequest MyRequest = (HttpWebRequest)HttpWebRequest.Create(NFOlink);
                using (WebResponse resp = MyRequest.GetResponse())
                    using (Stream respStream = resp.GetResponseStream())
                        using (StreamReader reader = new StreamReader(respStream))
                            HTML = reader.ReadToEnd();

                Nzb.NFO = string.Empty; // already fill this in at this point, if we have an error decoding, don't try again

                if (HTML != null)
                {
                    int Pos1 = HTML.IndexOf("<pre id=");
                    if (Pos1 < 0)
                        return null;
                    Pos1 = HTML.IndexOf('>', Pos1) + 1;
                    int Pos2 = HTML.IndexOf("</pre>", Pos1) - 1;
                    if (Pos2 < Pos1)
                        return null;

                    Nzb.NFO = HTML.Substring(Pos1, Pos2 - Pos1);

                    Pos1 = Nzb.NFO.IndexOf('\r');
                    if (Pos1 == -1)
                        Nzb.NFO = Nzb.NFO.Replace("\n", "\r\n");
                }
            }
            catch (Exception) { /* blaah */ }

            return Nzb.NFO;
        }

        string GetDescriptionPart(string Descr, string SearchString, int Number)
        {
            int idx = Descr.IndexOf(SearchString);
            while (Number > 0)
            {
                idx = Descr.IndexOf(SearchString, idx + SearchString.Length);
                if (idx < 0)
                    return string.Empty;
                Number--;
            }

            idx = Descr.IndexOf('>', idx) + 1;
            int idx2 = Descr.IndexOf('<', idx);
            if (idx2 > idx)
                Descr = Descr.Substring(idx, idx2 - idx);
            else
                Descr = Descr.Substring(idx);
                    
            return Descr.Replace("\n", "").Replace("\r", "").Trim();
        }

        string GetSearchURL(bool bNextPage)
        {
            string strURL = "http://nzbindex.nl/rss/?";

            //Query text
            if (SearchParams.Text != null &&  SearchParams.Text.Length > 0)
                strURL += "q=" + string.Join("+", SearchParams.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            
            //Max age
            strURL += "&age=" + (SearchParams.MaxDays > 0 ? SearchParams.MaxDays.ToString() : "");

            foreach (int grp in _SearchParams.SearchGroups)
                strURL += "&g[]=" + grp;

            if (_SearchParams.SortColumn == SortColumn.Age)
                strURL += "&sort=agedesc";
            else if (_SearchParams.SortColumn == (SortColumn.Age | SortColumn.Descending))
                strURL += "&sort=age";
            if (_SearchParams.SortColumn == SortColumn.Size)
                strURL += "&sort=size";
            else if (_SearchParams.SortColumn == (SortColumn.Size | SortColumn.Descending))
                strURL += "&sort=sizedesc";

            if (SearchParams.MinSize > 0)
                strURL += "&minsize=" + SearchParams.MinSize.ToString();
            if (SearchParams.MaxSize > 0)
                strURL += "&maxsize=" + SearchParams.MaxSize.ToString();

            strURL += "&max=" + Global.Config.ItemsPerPage;
            //strURL += "&more=1";

            if (bNextPage)
            {
                int page = _SearchResult.Count / Global.Config.ItemsPerPage;
                strURL += "&page=" + page.ToString();
            }

            return strURL;
        }

        public override SortColumn GetSorting()
        {
            return _SearchParams.SortColumn;
        }

        int SortResultItems(NzbItem Item1, NzbItem Item2)
        {
            return Item1.Title.CompareTo(Item2.Title);
        }

        int SortResultItemsDesc(NzbItem Item1, NzbItem Item2)
        {
            return Item2.Title.CompareTo(Item1.Title);
        }

        public override void SetSorting(SortColumn col)
        {
            try
            {
                SortColumn c = (SortColumn)((int)col & 0xF);
                if (c == SortColumn.FileName) //FileName we sort locally
                {
                    NzbItem[] Items = _SearchResult.ToArray();
                    if ((col & SortColumn.Descending) == SortColumn.Descending)
                        Array.Sort(Items, SortResultItemsDesc);
                    else
                        Array.Sort(Items, SortResultItems);

                    _SearchResult = new NzbItemsCollection(Items);
                    FireResultsUpdated(UpdateReason.Sorting,  null);
                }
                else
                {
                    if (c != SortColumn.Age && c != SortColumn.Size)
                        throw new Exception("Unable to sort on column: " + col.ToString()); //can't sort by anything else then age and size

                    _SearchParams.SortColumn = col;
                    Search(); //this provider simply leaves the sorting to be done on server side
                }
            }
            catch (Exception exc)
            {
                FireResultsUpdated(UpdateReason.Sorting, exc.Message);
            }
        }

        public override void CreateAdditionalControls(Panel parent)
        {
            parent.Controls.Add(cmdGroups);
            /*
            parent.Controls.Add(lblMaxItems);
            parent.Controls.Add(cboMaxItems);
            

            cboMaxItems.Items.Add(25);
            cboMaxItems.Items.Add(50);
            cboMaxItems.Items.Add(100);
            cboMaxItems.Items.Add(250);
            cboMaxItems.SelectedIndex = 3;
            */
            cmdGroups.Click += new EventHandler(cmdGroups_Click);
        }

        void cmdGroups_Click(object sender, EventArgs e)
        {
            NzbIndexGroupSelector select = new NzbIndexGroupSelector(cmdGroups.PointToScreen(new Point()), _SearchParams);
            select.Show();
        }

        public override void NewSearchParams()
        {
            _SearchParams = new SearchSettings_NzbIndex();
            _SearchResult = new NzbItemsCollection();
        }
    }
}
