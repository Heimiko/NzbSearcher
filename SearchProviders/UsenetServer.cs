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
using System.Text.RegularExpressions;
using System.Web;
using Heimiko;

namespace NzbSearcher
{
    public class UsenetServerConfig : ProviderConfig<SearchSettings_UsenetServer>
    {
        public UsenetServerConfig()
        {
            UseForCollectiveSearch = false; //don't use in collective search by default
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class NzbItem_UsenetServer : NzbItem
    {
        
    }

    [Serializable]
    public class SearchSettings_UsenetServer : SearchSettings
    {
        public SortColumn Sort { get; set; }
        public override bool SupportsMinSize { get { return false; } }

        public bool Partial { get; set; }
        public bool AnyKeyWord { get; set; }
    }

    public class UsenetServer : SearchProviderBase
    {
        SearchSettings_UsenetServer _SearchParams = new SearchSettings_UsenetServer();

        //Properties
        public override Image Icon { get { return Properties.Resources.UNS; } }
        public override string Name { get { return "UseNetServer"; } }
        public override string DisplayName { get { return "UseNet"; } }
        public override string ToolTip { get { return "UseNetServer Global Search"; } }
        public override string HistoryName { get { return "UseNetServer"; } }

        public override SearchSettings SearchParams { get { return _SearchParams; } set { _SearchParams = (SearchSettings_UsenetServer)value; } }
        public override bool IsItemFromProvider(NzbItem item) { return item is NzbItem_UsenetServer; }
        public override IProviderConfig Config { get { return Config<UsenetServerConfig>.Value; } }

        protected override CookieContainer Cookies { get { return _Browser.Cookies; } }

        WebTextBrowser _Browser = null;

        //Additional controls
        RadioButton radWhole = new RadioButton() { Text = "Whole", AutoSize = false, Height = 23, Width = 60, Margin = new Padding(0) };
        RadioButton radPartial = new RadioButton() { Text = "Partial", AutoSize = false, Height = 23, Margin = new Padding(0) };
        CheckBox chkAnyKeyword = new CheckBox() { Text = "Any Keyword", AutoSize = false, Height = 23, Margin = new Padding(0) };

        public override void Initialize()
        {
            base.Initialize();
        }

        bool StringAtPosStartsWith(string Source, int StartIndex, string value)
        {
            if (Source.Length < (value.Length + StartIndex))
                return false;

            foreach (char chr in value)
                if (Source[StartIndex++] != chr)
                    return false;

            return true;
        }

        string GetStringValueBetween(string Source, int Pos, string Start, string End)
        {
            Pos = Source.IndexOf(Start, Pos);
            if (Pos < 0)
                return null;

            string value = string.Empty;

            while (Pos < Source.Length)
            {
                if (StringAtPosStartsWith(Source, Pos, End))
                    break; //we're done

                value += Source[Pos++];
            }

            return value;
        }

        string GetSearchURL(UpdateReason Reason)
        {
            string SearchURL = null;

            SearchURL = "https://globalsearch.usenetserver.com/index.php?";
            SearchURL += "search=" + HttpUtility.UrlEncode(_SearchParams.Text);
            SearchURL += "&group=&poster=&endrange=1&";
            SearchURL += "startrange=" + (_SearchParams.MaxDays > 0 ? _SearchParams.MaxDays : 1000);
            SearchURL += "&pagelimit=" + Global.Config.ItemsPerPage + "&searchbutton=SEARCH&";
            SearchURL += "searchmode=" + (_SearchParams.Partial ? "partial" : "whole");
            SearchURL += "&pagemode=search";

            if (_SearchParams.AnyKeyWord)
                SearchURL += "&anykeyword=true";

            if (Reason == UpdateReason.Sorting)
            {
                string SortBy = null;

                switch (_SearchParams.Sort)
                {
                    case SortColumn.FileName: SortBy = "subject"; break;
                    case SortColumn.Age: SortBy = "date"; break;
                    case SortColumn.Size: SortBy = "size"; break;
                    case SortColumn.Availability: SortBy = "completion"; break;
                }

                if (SortBy != null)
                    SearchURL += "&sortby=" + SortBy;
            }

            return SearchURL;
        }

        protected override void Search(object bNextPage)
        {
            Search(bNextPage == null ? UpdateReason.NewSearch : UpdateReason.MoreSearch);
        }

        protected void Search(UpdateReason Reason)
        {
            string SearchURL = GetSearchURL(Reason);

            try
            {
                if (Reason == UpdateReason.NewSearch || Reason == UpdateReason.Sorting)
                {
                    _SearchResult.Clear();
                }
                else
                {
                    //https://globalsearch.usenetserver.com/index.php?anykeyword=&offset=25
                    SearchURL = null;
                    //todo
                }

                if (SearchURL != null)
                {
                    if (_Browser == null)
                    {
                        WebTextBrowser NewBrowser = new WebTextBrowser();

                        NewBrowser.Navigate("https://globalsearch.usenetserver.com");
                        WebForm LoginForm = NewBrowser.GetWebFormByName("login");
                        LoginForm.Values["username"] = ((UsenetServerConfig)Config).UserName; // "heimiko";
                        LoginForm.Values["password"] = ((UsenetServerConfig)Config).Password; // "SVWZ71RNEnnur";
                        LoginForm.Submit(NewBrowser);
                        if (NewBrowser.GetWebFormByName("login") != null)
                            throw new Exception("Either your username or password were incorrect");

                        _Browser = NewBrowser; // success -> we continue doing our search
                    }

                    _Browser.Navigate(SearchURL);
                    ParseSearchResults(Reason);
                }
            }
            catch (Exception e)
            {
                FireResultsUpdated(Reason, e.Message);
            }
        }

        void ParseSearchResults(UpdateReason Reason)
        {
            string ErrorMessage = null;

            try
            {
                WebElement ErrorText = _Browser.GetElement("div", "class", "errorstatus");
                if (ErrorText != null)
                    ErrorMessage = ErrorText.Content;

                List<WebElement> WebElmResults = new List<WebElement>();
                int Pos1 = 0, Pos2 = 0;
                while (true)
                {
                    WebElement Elm1 = _Browser.GetElement("tr", "id", "resultsdark", ref Pos1);
                    WebElement Elm2 = _Browser.GetElement("tr", "id", "resultslight", ref Pos2);
                    if (Elm1 != null)
                        WebElmResults.Add(Elm1);
                    if (Elm2 != null)
                        WebElmResults.Add(Elm2);
                    if (Elm1 == null && Elm2 == null)
                        break;
                }

                //now we have the list of results -> process them

                foreach (WebElement ResultElm in WebElmResults)
                {
                    try
                    {
                        NzbItem_UsenetServer NewItem = new NzbItem_UsenetServer();

                        NewItem.Title = HttpUtility.HtmlDecode(ResultElm.GetElement("td", "id", "resultssubject").Content);
                        NewItem.DownloadLink = _Browser.GetFullURL(ResultElm.GetElement("td", "id", "resultsaction").GetElement("a", null, null).GetAttributeValue("href"));
                        string PercentAvailable = ResultElm.GetElement("td", "id", "resultscompletion").Content.Trim('%', ' ');
                        NewItem.PercentAvailable = PercentAvailable.Length <= 3 ? int.Parse(PercentAvailable) : 100;
                        NewItem.UniqueID = ResultElm.GetElement("td", "id", "resultscheckbox").GetElement("input", "type", "checkbox").GetAttributeValue("value");
                        NewItem.Size = ResultElm.GetElement("td", "id", "resultssize").Content;
                        NewItem.Age = ResultElm.GetElement("td", "id", "resultsdate").Content; // format: 04-31
                        
                        //translate date to age
                        Match AgeMatch = Regex.Match(NewItem.Age, "^[0-9]{2}-[0-9]{2}");
                        if (AgeMatch.Success)
                        {
                            try
                            {
                                NewItem.PostedDate = new DateTime(DateTime.Now.Year, int.Parse(NewItem.Age.Substring(0, 2)), int.Parse(NewItem.Age.Substring(3, 2)));
                                if (NewItem.PostedDate > DateTime.Now)
                                    NewItem.PostedDate = NewItem.PostedDate.AddYears(-1);

                                NewItem.Age = Global.GetAgeFrom(NewItem.PostedDate);
                            }
                            catch { /* do nothing */ }
                        }


                        //DateTime PostedDate = DateTime.Parse(NewItem.Age);
                        //DateTime PostedDate

                        NewItem.HasMissingParts = NewItem.PercentAvailable < 100;

                        if (NewItem.PercentAvailable > 0) //only add result if its actually available
                            _SearchResult.Add(NewItem);
                    }
                    catch
                    {
                        //error while parsing element, skip to next
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                FireResultsUpdated(Reason, ErrorMessage);
            }
        }

        public override void CreateAdditionalControls(Panel parent)
        {
            parent.Controls.Add(radWhole);
            parent.Controls.Add(radPartial);
            parent.Controls.Add(chkAnyKeyword);
        }

        public override void ProviderToScreen()
        {
            radWhole.Checked = !_SearchParams.Partial;
            radPartial.Checked = _SearchParams.Partial;
            chkAnyKeyword.Checked = _SearchParams.AnyKeyWord;
        }

        public override void ScreenToProvider()
        {
            _SearchParams.Partial = radPartial.Checked;
            _SearchParams.AnyKeyWord = chkAnyKeyword.Checked;
        }

        public override void SetSorting(SortColumn col)
        {
            if ((col & SortColumn.Descending) != 0)
                col = col ^ SortColumn.Descending;

            _SearchParams.Sort = col;
            Search(UpdateReason.Sorting);
        }

        public override void NewSearchParams()
        {
            _SearchParams = new SearchSettings_UsenetServer();
            _SearchResult = new NzbItemsCollection();
        }
    }
}
