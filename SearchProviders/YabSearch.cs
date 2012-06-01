using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Collections;
using System.Windows.Forms;

//using System.Threading;
using System.Xml;
using System.Net;
using Heimiko;

namespace NzbSearcher
{
    public class YabSearchConfig : ProviderConfig<SearchSettings_YabSearch>
    {

    }

    public class NzbItem_YabSearch : NzbItem
    {
        bool _HasNFO = false;
        public override bool HasNFO { get { return _HasNFO; } }
        public void SetHasNFO(bool b) { _HasNFO = b; }

        public string WebLink { get; set; }
        public string NFO { get; set; }
    }

    [Serializable]
    public class SearchSettings_YabSearch : SearchSettings
    {
        public override bool SupportsMaxDays { get { return false; } }
        public SortColumn Sort { get; set; }
    }

    class YabSearch : SearchProviderBase
    {
        public override string Name { get { return "YabSearch.com"; } }
        public override string DisplayName { get { return "Yab"; } }
        public override string ToolTip { get { return "YabSearch.com Search Provider"; } }
        public override string HistoryName { get { return "NZBIndex"; } } // uses SAME history file as NzbIndex !!

        SearchSettings_YabSearch _SearchParams = new SearchSettings_YabSearch();
        public override SearchSettings SearchParams { get { return _SearchParams; } set { _SearchParams = (SearchSettings_YabSearch)value; } }

        public override IProviderConfig Config { get { return Config<YabSearchConfig>.Value; } }
        protected override CookieContainer Cookies { get { return _Browser.Cookies; } }

        public override Image Icon { get { return Properties.Resources.yabsearch; } }
        public override bool IsItemFromProvider(NzbItem item) { return item is NzbItem_YabSearch; }

        WebTextBrowser _Browser = null;

        public override void Search()
        {
            DoSearch(UpdateReason.NewSearch);
        }

        public override void SetSorting(SortColumn col)
        {
            if (_SearchParams.Sort == col)
                _SearchParams.Sort = col | SortColumn.Descending;
            else
                _SearchParams.Sort = col;

            DoSearch(UpdateReason.Sorting);
        }

        void DoSearch(UpdateReason Reason)
        {
            try
            {
                _SearchResult.Clear();

                if (_Browser == null)
                {
                    _Browser = new WebTextBrowser();
                    _Browser.Navigate("http://www.yabsearch.com");
                }

                WebForm[] forms = _Browser.GetWebForms();
                if (forms.Length == 0)
                {
                    _Browser.Navigate("http://www.yabsearch.com");
                    forms = _Browser.GetWebForms();
                }

                if (forms.Length > 0)
                {
                    forms[0].Values["q"] = _SearchParams.Text;
                    forms[0].Values["sizemin"] = _SearchParams.MinSize.ToString();
                    forms[0].Values["perpage"] = Global.Config.ItemsPerPage.ToString();

                    switch (_SearchParams.Sort)
                    {
                        case SortColumn.Age:
                            forms[0].Values["sort"] = "agedesc";
                            break;
                        case SortColumn.Size:
                            forms[0].Values["sort"] = "size";
                            break;
                        case SortColumn.Age | SortColumn.Descending:
                            forms[0].Values["sort"] = "age";
                            break;
                        case SortColumn.Size | SortColumn.Descending:
                            forms[0].Values["sort"] = "sizedesc";
                            break;
                        default:
                            forms[0].Values["sort"] = "agedesc";
                            break;
                    }
                    
                    forms[0].Submit(_Browser);
                    ProcessResults();
                }
            }
            finally
            {
                FireResultsUpdated(Reason, null);
            }
        }

        void ProcessResults()
        {
            int Index = 0;

            while (true)
            {
                WebElement checkbox = _Browser.GetElement("input", "type", "checkbox", ref Index); //each checkbox marks the beginning of a new item

                if (checkbox == null)
                    break;

                try
                {
                    NzbItem_YabSearch NewItem = new NzbItem_YabSearch();
                    NewItem.UniqueID = GetUniqueID(checkbox.GetAttributeValue("name"));
                    WebElement Subject = _Browser.GetElement("td", "class", "subject", ref Index);
                    NewItem.Title = Subject.Content.StartsWith("<a ") ? Subject.GetElement("a", null, null).Content : Subject.Content;
                    NewItem.DownloadLink = _Browser.GetElement("a", null, null, ref Index).GetAttributeValue("href");
                    NewItem.Size = _Browser.GetElement("td", null, null, ref Index).Content;
                    NewItem.PercentAvailable = int.Parse(_Browser.GetElement("td", null, null, ref Index).Content.Trim(' ', '%'));
                    NewItem.HasMissingParts = NewItem.PercentAvailable != 100;
                    NewItem.HasBeenDownloaded = _History.ContainsHistory(NewItem.UniqueID);

                    string[] DateTimeParts = _Browser.GetElement("td", null, null, ref Index).Content.Split('-');
                    if (DateTimeParts.Length == 3)
                    {
                        NewItem.PostedDate = new DateTime(int.Parse(DateTimeParts[2]), int.Parse(DateTimeParts[1]), int.Parse(DateTimeParts[0]));
                        NewItem.Age = Global.GetAgeFrom(NewItem.PostedDate);
                    }

                    _SearchResult.Add(NewItem);
                }
                catch
                {
                    //unexpected item format, skip it 
                }
            }
        }

        public override void NewSearchParams()
        {
            _SearchParams = new SearchSettings_YabSearch();
            _SearchResult = new NzbItemsCollection();
        }

    }
}
