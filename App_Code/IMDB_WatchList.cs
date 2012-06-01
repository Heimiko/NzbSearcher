using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;

using Heimiko;

namespace NzbSearcher
{
    public class WatchListItem
    {
         string[] _line = new string[16];
         public WatchListItem(string[] cells)
         {
             for (int i = 0; i < cells.Length; i++)
             {
                 _line[i] = cells[i].Trim(',', '\"');
             }
         }
        public string position { get { return _line[0]; } }
        public string const_ { get { return _line[1]; } }
        public string created { get { return _line[2]; } }
        public string modified { get { return _line[3]; } }
        public string description { get { return _line[4]; } }
        public string title { get { return _line[5]; } }
        public string title_type { get { return _line[6]; } }
        public string directors { get { return _line[7]; } }
        public string your_rating { get { return _line[8]; } }
        public string imdb_rating { get { return _line[9]; } }
        public string runtime { get { return _line[10]; } }
        public string year { get { return _line[11]; } }
        public string genres { get { return _line[12]; } }
        public string num_votes { get { return _line[13]; } }
        public string release_date { get { return _line[14]; } }
        public string url { get { return _line[15]; } }
    }

    public class WatchListDefenition
    {
        public string Name { get; set; }
        public string ListID { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class WitchListItemsCollection : List<WatchListItem>
    {
        PropertyInfo _SortBy = null;
        bool _bReverse = false;

        public void Sort(string SortBy)
        {
            if (_SortBy != null && _SortBy.Name == SortBy)
            {
                _bReverse = !_bReverse;
                base.Sort(Compare);
            }
            else
            {
                _bReverse = false;
                _SortBy = typeof(WatchListItem).GetProperty(SortBy);
                if (_SortBy != null)
                    base.Sort(Compare);
            }
        }

        int Compare(WatchListItem x, WatchListItem y)
        {
            float FloatX, FloatY;
            string ValueX = _SortBy.GetValue(x, null) as string;
            string ValueY = _SortBy.GetValue(y, null) as string;
            if (ValueX == null && ValueY == null)
                return 0;
            if (ValueX == null )
                return _bReverse ? 1 : -1;
            if (ValueY == null)
                return _bReverse ? -1 : 1;
            if (float.TryParse(ValueX, out FloatX) && float.TryParse(ValueY, out FloatY))
                return _bReverse ? FloatY.CompareTo(FloatX) : FloatX.CompareTo(FloatY);
            return _bReverse ? ValueY.CompareTo(ValueX) : ValueX.CompareTo(ValueY);
        }
    }

    public class IMDB_WatchList
    {
        const string _LoginURL = "https://secure.imdb.com/register-imdb/login";
        const string _LogoutURL = "http://www.imdb.com/register/logout";
        const string _SettingsURL = "https://secure.imdb.com/register-imdb/";
        const string _UserURL = "http://www.imdb.com/user/";

        WebTextBrowser _Browser = new WebTextBrowser();

        public IMDBconfig Config { get { Init(); return Config<IMDBconfig>.Value; } }

        WatchListDefenition _MainWatchList = new WatchListDefenition() { ListID = "watchlist", Name = "Your Watchlist" };
        WatchListDefenition MainWatchList { get { return _MainWatchList; } }

        List<WatchListDefenition> _MyWatchLists = null;
        public List<WatchListDefenition> MyWatchLists 
        { 
            get 
            {
                if (_MyWatchLists == null)
                    RefreshMyWatchListDefenitions();
                return _MyWatchLists; 
            } 
        }

        public List<WatchListDefenition> AdditionalWatchLists { get { return Config.AdditionalWatchLists; } }

        public WitchListItemsCollection this[string ListID]
        { 
            get 
            {
                if (!_MyLists.ContainsKey(ListID))
                    RefreshWatchList(ListID);
                if (_MyLists.ContainsKey(ListID))
                    return _MyLists[ListID];
                return null;
            } 
        }

        Dictionary<string, WitchListItemsCollection> _MyLists = new Dictionary<string, WitchListItemsCollection>();
        bool _HasInitialized = false;
        bool _HasLoggedIn = false;
        string _UserId;

        void Init()
        {
            if (_HasInitialized)
                return;

            Config<IMDBconfig>.Value.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Config_PropertyChanged);
            _HasInitialized = true;
        }

        void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UserName":
                case "Password":
                    _HasLoggedIn = false; //need to re-login
                    _UserId = null;
                    //_MenuTagObject = new object(); //need rebuild
                    break;
                default:
                    //_MenuTagObject = new object(); //need rebuild
                    break;

            }
        }

        bool DoLogin()
        {
            using (frmMessage.Show("Logging into IMDB..."))
            {
                _Browser = new WebTextBrowser(); //lets clear all cookies (if any)
                _Browser.Navigate(_LoginURL); //always begin by going to login site

                foreach (WebForm form in _Browser.GetWebForms())
                {
                    if (form.Action.Contains("login"))
                    {
                        form.Values["login"] = Config.UserName;
                        form.Values["password"] = Config.Password;
                        form.Submit(_Browser);

                        //Check if the link to log out is present, if yes, the we should be logged in
                        if (_Browser.GetElement("a", "href", _LogoutURL) != null)
                        {
                            _HasLoggedIn = true;

                            //Need to get the user id in order to be able to export the lists as csv
                            _Browser.Navigate(_SettingsURL);
                            int StartIndex = 0;
                            WebElement elm = _Browser.GetElement("a", "href", null, ref StartIndex);

                            while (elm != null)
                            {
                                Match Error2 = Regex.Match(elm.GetAttributeValue("href"), _UserURL+"([^/]+).*");
                                if (Error2.Success)
                                {
                                    _UserId = Error2.Groups[1].Value;
                                    return true;
                                }
                                elm = _Browser.GetElement("a", "href", null, ref StartIndex);
                            }

                        }
                        
                        Match Error = Regex.Match(_Browser.DocumentHTML, "<font color=\"#ff0000\"><b>\n(.*?)<");
                        if (Error.Success)
                        {
                            MessageBox.Show(Error.Groups[1].Value, "IMDB Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            frmConfig cfg = new frmConfig();
                            cfg.InitialActiveTab = "IMDB";
                            cfg.TopMost = true;
                            cfg.ShowDialog();
                        }

                        break;
                    }
                }
            }

            return false;
        }

        public void RefreshMyWatchListDefenitions()
        {
            using (frmMessage.Show("Fetching your IMDB's watchlists..."))
            {
                if (!_HasLoggedIn)
                    DoLogin();

                if (_HasLoggedIn)
                {
                    _MyWatchLists = new List<WatchListDefenition>();
                    //add the default one
                    _MyWatchLists.Add(_MainWatchList);

                    //find out about other lists you may have
                    _Browser.Navigate("http://www.imdb.com/list/_ajax/lists?list_type=Titles");

                    try
                    {
                        //Deserialize JSON and store values in MyWatchLists
                        JsonObject JSON = JsonObject.Deserialize(_Browser.DocumentHTML);
                        foreach (JsonObject item in JSON["lists"].Childs)
                            _MyWatchLists.Add(new WatchListDefenition
                            {
                                Name = item["name"].Value,
                                ListID = item["list_id"].Value
                            });
                    }
                    catch { /* do nothing */ }
                }
            }
        }

        public void RefreshWatchList(string ListID)
        {
            using (frmMessage.Show("Refreshing your IMDB's watchlist..."))
            {
                RefreshWatchList_NoDialog(ListID);
            }
        }

        public void RefreshWatchList_NoDialog(string ListID)
        {
            if (!_HasLoggedIn)
                DoLogin();

            if (_HasLoggedIn)
            {
                //TODO: obtain lists: http://www.imdb.com/list/_ajax/lists?list_type=Titles

                string URL = "http://www.imdb.com/list/export?list_id=" + ListID + "&author_id=" + _UserId; //ur26936582

                _Browser.Navigate(URL);
                string CSV = _Browser.DocumentHTML;
                string[] lines = CSV.Split((char)10);
                string[] columns = null;
                WitchListItemsCollection Items = new WitchListItemsCollection();

                foreach (string line in lines)
                {
                    if (columns == null || columns.Length <= 1)
                    {
                        columns = line.Split(',');
                        for (int i = 0; i < columns.Length; i++)
                            columns[i] = columns[i].Trim(',', '\"');  
                    }
                    else
                    {
                        string[] cells = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                        if (cells.Length == columns.Length)
                        {
                            WatchListItem item = new WatchListItem(cells);
                            Items.Add(item);
                        }
                    }
                }

                //update item in cache
                _MyLists[ListID] = Items;
            }
        }

        public string GetListTitle(string ListID)
        {
            using (frmMessage.Show("Looking up and verifying list ID..."))
            {
                _Browser.Navigate("http://www.imdb.com/list/" + ListID);

                if (!_Browser.CurrentUri.AbsolutePath.Contains(ListID))
                    throw new Exception("Invalid List ID");

                return _Browser.Title.Substring(5).Trim();
            }
        }

        public bool IsMyList(string ListID)
        {
            if (MyWatchLists == null)
                return false;
            
            foreach (WatchListDefenition list in MyWatchLists)
                if (list.ListID == ListID)
                    return true;
            
            return false;
        }

        public bool DeleteMovie(string ListID, WatchListItem item)
        {
            //check if the list we're deleting this item on is actually owned by us
            if (!IsMyList(ListID))
                return false;

            string url = null;

            if (ListID == _MainWatchList.ListID)
                url = "http://www.imdb.com/user/" + _UserId + "/watchlist?start=1&view=compact";
            else
                url = "http://www.imdb.com/list/" + ListID + "/?start=1&view=compact";

            _Browser.Navigate(url);
            int StartIndex = 0;
            string RealListID = ListID;
            WebElement elm = _Browser.GetElement("div", "data-list-id", null, ref StartIndex);
            if (elm != null)
                RealListID = elm.GetAttributeValue("data-list-id"); //update list ID

            WebElement DataItem = _Browser.GetElement("tr", "data-item-id", null, ref StartIndex);
            while (DataItem != null)
            {
                string DataID = DataItem.GetAttributeValue("data-item-id");
                WebElement link = DataItem.GetElement("a", "href", "/title/" + item.const_ + "/");
                if (link != null)
                {
                    //we have the correct item -> delete it!
                    _Browser.Navigate("http://www.imdb.com/list/_ajax/edit?list_item_id=" + DataID + "&action=delete&list_id=" + RealListID);

                    try
                    {
                        //verify that the delete action was successful
                        JsonObject content = JsonObject.Deserialize(_Browser.DocumentHTML);
                        if (content["status"].Value == "200")
                        {
                            //remove item from cache
                            _MyLists[ListID].Remove(item);
                            return true; //we're done!
                        }
                    }
                    catch { }
                            
                    return false; // fail
                }

                DataItem = _Browser.GetElement("tr", "data-item-id", null, ref StartIndex);
            }

            return false;
        }
    }
}
