using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using System.IO;
using System.Net;
using Heimiko;


namespace NzbSearcher
{
    public class NzbMatrixConfig : ProviderConfig<SearchSettings_NzbMatrix>
    {
        public string UserName { get; set; }
        public string ApiKey { get; set; }
    }

    public class NzbItem_NzbMatrix : NzbItem
    {
        bool _HasNFO = false;
        public override bool HasNFO { get { return _HasNFO; } }
        public void SetHasNFO(bool b) { _HasNFO = b; }

        public string WebLink { get; set; }
        public string NFO { get; set; }
    }

    [Serializable]
    public class SearchSettings_NzbMatrix : SearchSettings
    {
        public SearchSettings_NzbMatrix()
        {

        }

        public bool EnglishOnly { get; set; }
        public bool SearchInWebLink { get; set; }
    }

    class NzbMatrix : SearchProviderBase
    {
        public static NzbMatrixConfig NzbMatrixConfig { get { return Config<NzbMatrixConfig>.Value; } }
        public override IProviderConfig Config { get { return NzbMatrixConfig; } }

        SearchSettings_NzbMatrix _SearchParams = new SearchSettings_NzbMatrix();
        public override SearchSettings SearchParams { get { return _SearchParams; } set { _SearchParams = (SearchSettings_NzbMatrix)value; } }

        //Additional controls
        CheckBox chkEnglishOnly = new CheckBox() { Text = "English-Only", AutoSize = true };
        CheckBox chkSearchInWebLink = new CheckBox() { Text = "Search in WebLink", AutoSize = true};

        public override string DisplayName { get { return "Matrix"; } }
        public override string Name { get { return "NzbMatrix.com"; } }
        public override string ToolTip { get { return "NzbMatrix.com Search Provider"; } }
        public override string HistoryName { get { return "NzbMatrix"; } }

        public override Image Icon { get { return Properties.Resources.NzbMatrix; } }
        public override bool IsItemFromProvider(NzbItem item) { return item is NzbItem_NzbMatrix; }


        protected override void Search(object bNextPage)
        {
            // API info: http://nzbmatrix.com/api-info.php

            string ErrorText = null;
            string SearchResults = null;

            if (bNextPage == null)
                _SearchResult.Clear();

            _bMoreAvailable = false;

            try
            {
                HttpWebRequest SearchRequest = (HttpWebRequest)HttpWebRequest.Create(GetSearchURL(bNextPage != null));
                SearchRequest.Timeout = WebTextBrowser.Timeout;

                using (WebResponse resp = SearchRequest.GetResponse())
                using (Stream respStream = resp.GetResponseStream())
                using (StreamReader reader = new StreamReader(respStream))
                    SearchResults = reader.ReadToEnd();

                if (SearchResults.IndexOf('|') > 0)
                {
                    string[] ResultItems = SearchResults.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ResultItem in ResultItems)
                    {
                        NzbItem_NzbMatrix NewItem = new NzbItem_NzbMatrix();
                        string[] ItemParts = ResultItem.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Part in ItemParts)
                        {
                            string[] KeyValue = Part.Split(new char[] { ':' } , 2);
                            if (KeyValue.Length == 2)
                            {
                                switch (KeyValue[0].Trim())
                                {
                                    case "NZBNAME": NewItem.Title = KeyValue[1]; break;
                                    case "NZBID": NewItem.UniqueID = long.Parse(KeyValue[1]); break;
                                    case "LINK": NewItem.DownloadLink = KeyValue[1]; break;
                                    case "SIZE": NewItem.Size = Global.GetSizeFromBytes(double.Parse(KeyValue[1].Split('.')[0], System.Globalization.CultureInfo.InvariantCulture)); break;
                                    case "USENET_DATE": NewItem.Age += " / " + Global.GetAgeFrom(DateTime.Parse(KeyValue[1])); break;
                                    case "INDEX_DATE": 
                                        NewItem.Age = Global.GetAgeFrom(DateTime.Parse(KeyValue[1]));
                                        NewItem.PostedDate = DateTime.Parse(KeyValue[1]); 
                                        break;
                                    case "NFO": NewItem.SetHasNFO(KeyValue[1] == "yes"); break;
                                    case "WEBLINK": NewItem.WebLink = KeyValue[1]; break;
                                }
                            }
                        }

                        if (NewItem.Title != null && (long)NewItem.UniqueID > 0)
                        {
                            NewItem.HasBeenDownloaded = _History.ContainsHistory(NewItem.UniqueID);
                            NewItem.PercentAvailable = 100;
                            _SearchResult.Add(NewItem);
                        }
                    }

                    //_bMoreAvailable = ResultItems.Length >= 15;
                }
                else
                {
                    ErrorText = SearchResults;
                    if (ErrorText.StartsWith("error:"))
                    {
                        ErrorText = SearchResults.Substring(6); //strip off "error" string
                        switch (ErrorText)
                        {
                            case "nothing_found": //no need to display this error
                                ErrorText = null;
                                break;
                            default:
                                break; //do nothing
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
            }

            UpdateReason Reason = bNextPage == null ? UpdateReason.NewSearch : UpdateReason.MoreSearch;
            FireResultsUpdated(UpdateReason.NewSearch, ErrorText);

        }

        string GetSearchURL(bool bNextPage)
        {
            string URL = "http://api.nzbmatrix.com/v1.1/search.php?";

            URL += "search=" + _SearchParams.Text + "&num=20";

            if (NzbMatrixConfig.UserName != null && NzbMatrixConfig.UserName.Length > 0)
                URL += "&username=" + NzbMatrixConfig.UserName;
            if (NzbMatrixConfig.ApiKey != null && NzbMatrixConfig.ApiKey.Length > 0)
                URL += "&apikey=" + NzbMatrixConfig.ApiKey;

            if (_SearchParams.MaxDays > 0)
                URL += "&age=" + _SearchParams.MaxDays;
            if (_SearchParams.MinSize > 0)
                URL += "&larger=" + _SearchParams.MinSize;
            if (_SearchParams.MaxSize > 0)
                URL += "&smaller=" + _SearchParams.MaxSize;
            if(_SearchParams.EnglishOnly)
                URL += "&englishonly=1";
            if (_SearchParams.SearchInWebLink)
                URL += "&searchin=weblink";

            return URL;        
        }

        protected override string GetNzbItemDownloadURL(NzbItem item)
        {
            string URL = "https://nzbmatrix.com/api-nzb-download.php?id=" + item.UniqueID;

            if (NzbMatrixConfig.UserName != null && NzbMatrixConfig.UserName.Length > 0)
                URL += "&username=" + NzbMatrixConfig.UserName;
            if (NzbMatrixConfig.ApiKey != null && NzbMatrixConfig.ApiKey.Length > 0)
                URL += "&apikey=" + NzbMatrixConfig.ApiKey;

            return URL;
        }

        public override string GetIMDBurl(NzbItem item)
        {
            NzbItem_NzbMatrix MatrixItem = item as NzbItem_NzbMatrix ;
            
            if (MatrixItem != null && MatrixItem.WebLink != null && MatrixItem.WebLink.Length > 0)
                return MatrixItem.WebLink;
            
            return base.GetIMDBurl(item);
        }

        public override string GetNzbInfo(NzbItem item)
        {
            NzbItem_NzbMatrix MatrixItem = item as NzbItem_NzbMatrix ;
            if (item == null)
                return null;
            if (!item.HasNFO || MatrixItem.NFO != null)
                return MatrixItem.NFO;

            string URL = "http://nzbmatrix.com/viewnfo.php?id=" + item.UniqueID + "&liteview=1";

            if (NzbMatrixConfig.UserName != null && NzbMatrixConfig.UserName.Length > 0)
                URL += "&username=" + NzbMatrixConfig.UserName;
            if (NzbMatrixConfig.ApiKey != null && NzbMatrixConfig.ApiKey.Length > 0)
                URL += "&apikey=" + NzbMatrixConfig.ApiKey;

            HttpWebRequest SearchRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            SearchRequest.Timeout = 10000;
            using (WebResponse resp = SearchRequest.GetResponse())
            using (Stream respStream = resp.GetResponseStream())
            using (StreamReader reader = new StreamReader(respStream))
                MatrixItem.NFO = reader.ReadToEnd();

            return MatrixItem.NFO;
        }

        public override void CreateAdditionalControls(Panel parent)
        {
            parent.Controls.Add(chkEnglishOnly);
            parent.Controls.Add(chkSearchInWebLink);
        }

        public override void ProviderToScreen()
        {
            chkEnglishOnly.Checked = _SearchParams.EnglishOnly;
            chkSearchInWebLink.Checked = _SearchParams.SearchInWebLink;
        }

        public override void ScreenToProvider()
        {
            _SearchParams.EnglishOnly = chkEnglishOnly.Checked;
            _SearchParams.SearchInWebLink = chkSearchInWebLink.Checked;
        }

        public override void NewSearchParams()
        {
            _SearchParams = new SearchSettings_NzbMatrix();
            _SearchResult = new NzbItemsCollection();
        }
    }
}
