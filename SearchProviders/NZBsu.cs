using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using System.IO;
using System.Net;
using System.Xml;

using Heimiko;

namespace NzbSearcher
{
    public class NZBsuConfig : ProviderConfig<SearchSettings_NZBsu>
    {
        public string ApiKey { get; set; }
    }

    public class NzbItem_NZBsu : NzbItem
    {

    }

    [Serializable]
    public class SearchSettings_NZBsu : SearchSettings
    {
        public override bool SupportsMaxDays { get { return false; } }
        public override bool SupportsMinSize { get { return false; } }

        public SearchSettings_NZBsu()
        {

        }
    }

    class NZBsu : SearchProviderBase
    {
        public static NZBsuConfig NZBsuConfig { get { return Config<NZBsuConfig>.Value; } }
        public override IProviderConfig Config { get { return NZBsuConfig; } }

        SearchSettings_NZBsu _SearchParams = new SearchSettings_NZBsu();
        public override SearchSettings SearchParams { get { return _SearchParams; } set { _SearchParams = (SearchSettings_NZBsu)value; } }

        public override string Name { get { return "NZB.su"; } }
        public override string DisplayName { get { return "NZBsu"; } }
        public override string ToolTip { get { return "NZB.su Search Provider"; } }
        public override string HistoryName { get { return "NZBsu"; } }

        public override Image Icon { get { return Properties.Resources.NZBsu; } }
        public override bool IsItemFromProvider(NzbItem item) { return item is NzbItem_NZBsu; }

        protected override void Search(object bNextPage)
        {
            // API info: http://nzb.su/apihelp

            string ErrorText = null;
            string SearchResults = null;

            if (bNextPage == null)
                _SearchResult.Clear();

            try
            {
                HttpWebRequest SearchRequest = (HttpWebRequest)HttpWebRequest.Create(GetSearchURL(bNextPage != null));
                SearchRequest.Timeout = WebTextBrowser.Timeout;
                using (WebResponse resp = SearchRequest.GetResponse())
                using (Stream respStream = resp.GetResponseStream())
                using (StreamReader reader = new StreamReader(respStream))
                    SearchResults = reader.ReadToEnd();


                XmlDocument XML = new XmlDocument();
                XML.LoadXml(SearchResults);

                XmlNode ErrorNode = XML.SelectSingleNode("/error");
                if (ErrorNode != null)
                    throw new Exception(ErrorNode.Attributes["description"].InnerText);

                XmlNodeList items = XML.SelectNodes("/rss/channel/item");
                foreach (XmlNode item in items)
                {
                    NzbItem_NZBsu NewItem = new NzbItem_NZBsu();
                    NewItem.Title = item.SelectSingleNode("title").InnerText;
                    NewItem.UniqueID = Path.GetFileName(item.SelectSingleNode("guid").InnerText);
                    NewItem.PostedDate = DateTime.Parse(item.SelectSingleNode("pubDate").InnerText);
                    NewItem.Age = Global.GetAgeFrom(NewItem.PostedDate);
                    NewItem.Size = Global.GetSizeFromBytes(double.Parse(item.SelectSingleNode("enclosure").Attributes["length"].InnerText, System.Globalization.CultureInfo.InvariantCulture));
                    NewItem.PercentAvailable = 100; //TODO
                    NewItem.HasBeenDownloaded = _History.ContainsHistory(NewItem.UniqueID);

                    _SearchResult.Add(NewItem);
                }
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
            }

            UpdateReason Reason = bNextPage == null ? UpdateReason.NewSearch : UpdateReason.MoreSearch;
            FireResultsUpdated(UpdateReason.NewSearch, ErrorText);

        }

        protected override string GetNzbItemDownloadURL(NzbItem item)
        {
            return "http://nzb.su/api?t=get&id=" + item.UniqueID + "&apikey=" + NZBsuConfig.ApiKey;
        }

        string GetSearchURL(bool bNextPage)
        {
            string URL = "http://nzb.su/api?";

            URL += "t=search";

            if (NZBsuConfig.ApiKey != null && NZBsuConfig.ApiKey.Length > 0)
                URL += "&apikey=" + NZBsuConfig.ApiKey;

            /*
            if (_SearchParams.MaxDays > 0)
                URL += "&age=" + _SearchParams.MaxDays;
            if (_SearchParams.MinSize > 0)
                URL += "&larger=" + _SearchParams.MinSize;
            if (_SearchParams.MaxSize > 0)
                URL += "&smaller=" + _SearchParams.MaxSize;
            */

            URL+= "&q=" + _SearchParams.Text;
            
            return URL;        
        }

        public override void NewSearchParams()
        {
            _SearchParams = new SearchSettings_NZBsu();
            _SearchResult = new NzbItemsCollection();
        }
    }
}
