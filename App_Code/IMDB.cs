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
    public enum IMDBmenuDisplayType
    {
        BothYearsAndCategories, // default display type
        Years,
        Categories,
        Flat
    }

    public class IMDBconfig : PropChangedBase
    {
        string _UserName;
        public string UserName { get { return _UserName; } set { _UserName = value; FirePropChanged("UserName"); } }

        string _Password;
        public string Password { get { return _Password; } set { _Password = value; FirePropChanged("Password"); } }

        bool _DisplayMyMovs;
        public bool DisplayMyMovs { get { return _DisplayMyMovs; } set { _DisplayMyMovs = value; FirePropChanged("DisplayMyMovs"); } }

        IMDBmenuDisplayType _MenuDisplayType;
        public IMDBmenuDisplayType MenuDisplayType { get { return _MenuDisplayType; } set { _MenuDisplayType = value; FirePropChanged("MenuDisplayType"); } }

        bool _DisplayYear = true;
        public bool DisplayYear { get { return _DisplayYear; } set { _DisplayYear = value; FirePropChanged("DisplayYear"); } }
        
        bool _DisplayCategory = false;
        public bool DisplayCategory { get { return _DisplayCategory; } set { _DisplayCategory = value; FirePropChanged("DisplayCategory"); } }

        bool _IncludeYearOnSearch = false;
        public bool IncludeYearOnSearch { get { return _IncludeYearOnSearch; } set { _IncludeYearOnSearch = value; FirePropChanged("IncludeYearOnSearch"); } }

        List<WatchListDefenition> _AdditionalWatchLists = new List<WatchListDefenition>();
        public List<WatchListDefenition> AdditionalWatchLists { get { return _AdditionalWatchLists; } set { _AdditionalWatchLists = value; FirePropChanged("AdditionalWatchLists"); } }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //              Old IMDB My Movies stuff
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region IMDB's My Movies

    public class IMDBmovie
    {
        public string ID { get; set; }
        public string Title { get; set; }   //movie title
        public string Category { get; set; } // category within "My Movies"
        public int Year { get; set; }

        public string CheckBoxID { get; set; } // ID for use with the form's Copy / Move action

        public override string ToString()
        {
            string ToString = Title;
            if (Global.IMDB.Config.DisplayYear)
                ToString += " (" + Year + ")";
            if (Global.IMDB.Config.DisplayCategory)
                ToString += " [" + Category + "]";
            return ToString;
        }
    }

    public class IMDBmoviesCollection : List<IMDBmovie>
    {
        public bool Contains(string ID)
        {
            return this[ID] != null;
        }

        public IMDBmovie this[string ID]
        {
            get
            {
                foreach (IMDBmovie mov in this)
                    if (mov.ID == ID)
                        return mov;
                return null;
            }
        }
    }

    public class IMDB
    {
        const string _MyMoviesURL = "http://www.imdb.com/mymovies/list";
        const string _LoginURL = "https://secure.imdb.com/register-imdb/login?u=/mymovies/list";
        const string _LogoutURL = "http://www.imdb.com/register/logout";

        const string _MyMoviesTitle = "My Movies";
        const string _RecycleCategory = "Recycle Bin";

        IMDBmoviesCollection _MyMovs = null;
        WebTextBrowser _Browser = new WebTextBrowser();
        WebForm _MainMoviesForm = null;
        object _MenuTagObject = new object();
        Dictionary<string, string> _MovieCategories = null;
        bool _HasInitialized = false;
        ToolStripDropDown _mnuMoveToCategory = new ToolStripDropDown();

        public IMDBconfig Config { get { Init();  return Config<IMDBconfig>.Value; } }
        public IMDBmoviesCollection MyMovies { get { return _MyMovs ?? RefreshMyMovies(); } }
        public bool IsInitialized { get { return _MyMovs != null; } }

        void Init()
        {
            if (_HasInitialized)
                return;

            _mnuMoveToCategory.RenderMode = ToolStripRenderMode.Professional;
            _mnuMoveToCategory.ItemClicked += new ToolStripItemClickedEventHandler(mnuMoveToCategory_ItemClicked);
            _mnuMoveToCategory.Opening += new System.ComponentModel.CancelEventHandler(mnuMoveToCategory_Opening);
            Config<IMDBconfig>.Value.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Config_PropertyChanged);
            _HasInitialized = true;
        }

        void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UserName":
                case "Password":
                    _MyMovs = null; //need to re-login
                    _MenuTagObject = new object(); //need rebuild
                    break;
                default:
                    _MenuTagObject = new object(); //need rebuild
                    break;

            }
        }

        public IMDBmoviesCollection RefreshMyMovies()
        {
            using (frmMessage.Show("Refreshing IMDB Movies..."))
            {
                //make sure we once did a login
                if (_MyMovs == null && !DoLogin())
                    return new IMDBmoviesCollection(); 
                else if (_MyMovs != null) //already initialized? then re-request "my movies" page
                {
                    _Browser.Navigate(_MyMoviesURL);
                    if (!_Browser.Title.Equals(_MyMoviesTitle)) //no longer logged in?
                        DoLogin();
                }

                if (_Browser.Title.Equals(_MyMoviesTitle))
                {
                    _MainMoviesForm = _Browser.GetWebFormByName("list");
                    _MyMovs = GetMoviesFromHTML(_MainMoviesForm.FormHTML);
                    _MenuTagObject = new object(); //new tag object, cause we need to rebuild the menu now
                    _MovieCategories = _MainMoviesForm.GetComboValues("to");

                    if (!_MovieCategories.ContainsKey(_RecycleCategory))
                        _MovieCategories.Add(_RecycleCategory, "0"); //make SURE we have this category!
                    
                    //remove all menu items
                    while(_mnuMoveToCategory.Items.Count > 0)
                        _mnuMoveToCategory.Items.RemoveAt(_mnuMoveToCategory.Items.Count - 1);
                    //Add all categories
                    foreach (KeyValuePair<string,string> Cat in _MovieCategories)
                        if (Cat.Value != "0")
                            _mnuMoveToCategory.Items.Add(new ToolStripButton(Cat.Key));
                }
            }

            return _MyMovs;
        }

        bool DoLogin()
        {
            _Browser.Navigate(_LoginURL); //always begin by going to login site

            foreach (WebForm form in _Browser.GetWebForms())
            {
                if (form.Action.Contains("login"))
                {
                    form.Values["login"] = Config.UserName;
                    form.Values["password"] = Config.Password;
                    form.Submit(_Browser);
                    
                    if (_Browser.Title.Equals(_MyMoviesTitle)) //only success if not on login page again
                        return true;

                    Match Error = Regex.Match(_Browser.DocumentHTML, "<font color=\"#ff0000\"><b>\n(.*?)<");
                    if (Error.Success)
                    {
                        MessageBox.Show(Error.Groups[1].Value, "IMDB Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        frmConfig cfg = new frmConfig();
                        cfg.InitialActiveTab = "IMDB";
                        cfg.TopMost = true;
                        cfg.ShowDialog();
                    }

                    //if we have movies listed at this point, remove them
                    _MyMovs = null; //revert to un-initialized state
                    break;
                }
            }

            return false;
        }

        Match RegexMatch(string input, params string[] parameters)
        {
            return Regex.Match(input.Replace('\n', ' '), string.Join(".*?", parameters));
        }

        //<a href="/title/tt1217209/">Brave</a> (2012)&nbsp;[<a href="list?l=47696310">Recycle Bin</a>]</td>
        IMDBmoviesCollection GetMoviesFromHTML(string HTML)
        {
            IMDBmoviesCollection col = new IMDBmoviesCollection();

            Match MovieItem = RegexMatch(HTML, "<input type=\"checkbox\" name=\"e\" value=\"([^\"]*)\"",
                                                "/title/([^/]*)/\">(.*?)</a>[ (]*([0-9]*)[^>]*>([^<]*)",
                                                "</tr>");
            while (MovieItem.Success)
            {
                IMDBmovie mov = new IMDBmovie();

                mov.CheckBoxID = MovieItem.Groups[1].Value;
                mov.ID = MovieItem.Groups[2].Value;
                mov.Title = HttpUtility.HtmlDecode(MovieItem.Groups[3].Value);
                mov.Year = TryParseInt(MovieItem.Groups[4].Value);
                mov.Category = MovieItem.Groups[5].Value;
                col.Add(mov);

                MovieItem = MovieItem.NextMatch();
            }
            
            return col;
        }

        public static int TryParseInt(string s)
        {
            try { return int.Parse(s); }
            catch { return 0; }
        }

        public bool MoveMovieToCategory(IMDBmovie mov, string NewCategory)
        {
            _MainMoviesForm.Values["e"] = mov.CheckBoxID;
            _MainMoviesForm.Values["action"] = "Move";
            _MainMoviesForm.Values["to"] = _MovieCategories[NewCategory]; // this SHOULD be retrieved by now
            _MainMoviesForm.Submit(_Browser);

            IMDBmoviesCollection MovsInCurrentCategory = GetMoviesFromHTML(_Browser.DocumentHTML);
            if (MovsInCurrentCategory.Contains(mov.ID))
            {
                mov.Category = NewCategory; // success! 
                _MenuTagObject = new object(); //movies menu need rebuilding now
                return true;
            }
            return false;
        }

        public bool DeleteMovie(IMDBmovie mov)
        {
            return MoveMovieToCategory(mov, _RecycleCategory);
        }

        public void ManageMyMovies()
        {
            System.Diagnostics.Process.Start(_MyMoviesURL);
        }

        #region IMDB Menu Building

        public void ReBuildMenu(ContextMenuStrip mnuIMDBmovies, ToolStripDropDown EditMenu, string Filter)
        {
            if (mnuIMDBmovies.Tag == _MenuTagObject)
                return; //no need to rebuild

            //remove items below 1st seperator
            int i = 0;
            while (!(mnuIMDBmovies.Items[i++] is ToolStripSeparator)); //search seperator
            while (mnuIMDBmovies.Items.Count != i) // remove all items below seperator
                mnuIMDBmovies.Items.RemoveAt(i);

            foreach (ToolStripItem EditItem in EditMenu.Items)
                if (EditItem.Text.ToLower().Contains("category"))
                    ((ToolStripMenuItem)EditItem).DropDown = _mnuMoveToCategory;

            if (Filter != null && Filter.Length > 0 && Filter[0] != '<')
            {
                AddMoviesToMenu_Flat(mnuIMDBmovies, EditMenu, Filter);
            }
            else
            {
                switch (Config.MenuDisplayType)
                {
                    case IMDBmenuDisplayType.Flat:
                        AddMoviesToMenu_Flat(mnuIMDBmovies, EditMenu, null);
                        break;
                    case IMDBmenuDisplayType.Years:
                        AddMoviesToMenu_Years(mnuIMDBmovies, EditMenu);
                        break;
                    case IMDBmenuDisplayType.Categories:
                        AddMoviesToMenu_Categories(mnuIMDBmovies, EditMenu);
                        break;
                    case IMDBmenuDisplayType.BothYearsAndCategories:
                        AddMoviesToMenu_Categories(mnuIMDBmovies, EditMenu);
                        mnuIMDBmovies.Items.Add(new ToolStripSeparator());
                        AddMoviesToMenu_Years(mnuIMDBmovies, EditMenu);
                        break;
                }
            }

            //assign Menu Tag Object, so we don't need to rebuild everytime 
            mnuIMDBmovies.Tag = _MenuTagObject;
        }

        ToolStripMenuItem NewMovieMenuEntry(IMDBmovie mov, ToolStripDropDown EditMenu, bool HookEvent)
        {
            ToolStripMenuItem MovMenuItem = new ToolStripMenuItem(mov.ToString());
            MovMenuItem.Tag = mov;
            MovMenuItem.DropDown = EditMenu;
            if (HookEvent)
                MovMenuItem.Click += new EventHandler(MovMenuItem_Click);
            return MovMenuItem;
        }

        void AddMoviesToMenu_Flat(ContextMenuStrip mnuIMDBmovies, ToolStripDropDown EditMenu, string Filter)
        {
            //next call may a while, it may (or not) initialize IMDB movies list (retrieve it from server)
            foreach (IMDBmovie mov in MyMovies)
            {
                if (mov.Title.ToLower().Contains(Filter.ToLower()))
                    mnuIMDBmovies.Items.Add(NewMovieMenuEntry(mov, EditMenu, false));
            }
        }

        int SortMovsCollections(IMDBmoviesCollection Col1, IMDBmoviesCollection Col2)
        {
            if (Col1.Count == 0 || Col2.Count == 0)
                return 0; // equal
            return Col1[0].Year.CompareTo(Col2[0].Year);
        }

        void AddMoviesToMenu_Years(ContextMenuStrip mnuIMDBmovies, ToolStripDropDown EditMenu)
        {
            Dictionary<int, IMDBmoviesCollection> ByYear = new Dictionary<int, IMDBmoviesCollection>();
            int MaxPerYear = 1;

            //first, sort all movs by year
            foreach (IMDBmovie mov in MyMovies)
            {
                if (mov.Category != _RecycleCategory)
                {
                    if (ByYear.ContainsKey(mov.Year))
                    {
                        IMDBmoviesCollection MovsCol = ByYear[mov.Year];
                        MovsCol.Add(mov);
                        if (MovsCol.Count > MaxPerYear)
                            MaxPerYear = MovsCol.Count;
                    }
                    else
                    {
                        IMDBmoviesCollection MovsCol = new IMDBmoviesCollection();
                        MovsCol.Add(mov);
                        ByYear.Add(mov.Year, MovsCol);
                    }
                }
            }

            if (MaxPerYear < 20)
                MaxPerYear = 20;

            List<IMDBmoviesCollection> CombinedMovCols = new List<IMDBmoviesCollection>();
            List<IMDBmoviesCollection> SortedMovCols = new List<IMDBmoviesCollection>(ByYear.Values);
            SortedMovCols.Sort(new Comparison<IMDBmoviesCollection>(SortMovsCollections)); //sort all movie collections by year

            // now combine mutiple years together, otherwise we'll have some super short sub menu's
            CombinedMovCols.Add(new IMDBmoviesCollection());
            foreach (IMDBmoviesCollection col in SortedMovCols)
            {
                if (CombinedMovCols[CombinedMovCols.Count - 1].Count != 0)
                {
                    int CombinedCount = CombinedMovCols[CombinedMovCols.Count - 1].Count + col.Count;
                    if (CombinedCount > MaxPerYear)
                        CombinedMovCols.Add(new IMDBmoviesCollection());
                }
                CombinedMovCols[CombinedMovCols.Count - 1].AddRange(col);
            }

            //now finally, we can add all sub-menus !!

            foreach (IMDBmoviesCollection col in CombinedMovCols)
            {
                if (col.Count > 0)
                {
                    string MenuTitle = col[0].Year.ToString();
                    if (col[0].Year != col[col.Count - 1].Year)
                        MenuTitle += "-" + col[col.Count - 1].Year;

                    ToolStripMenuItem YearMenu = (ToolStripMenuItem)mnuIMDBmovies.Items.Add(MenuTitle);

                    foreach (IMDBmovie mov in col)
                        YearMenu.DropDownItems.Add(NewMovieMenuEntry(mov, EditMenu, true));
                }
            }
        }

        void AddMoviesToMenu_Categories(ContextMenuStrip mnuIMDBmovies, ToolStripDropDown EditMenu)
        {
            IMDBmoviesCollection col = this.MyMovies; //make SURE our collection has been initialized
            
            if (_MovieCategories != null)
            {
                Dictionary<string, ToolStripMenuItem> CategoryMenus = new Dictionary<string, ToolStripMenuItem>();
                foreach (KeyValuePair<string, string> MovCat in _MovieCategories)
                    if (MovCat.Value != "0")
                        CategoryMenus.Add(MovCat.Key, (ToolStripMenuItem)mnuIMDBmovies.Items.Add(MovCat.Key));

                foreach (IMDBmovie mov in col)
                    if (CategoryMenus.ContainsKey(mov.Category))
                        CategoryMenus[mov.Category].DropDownItems.Add(NewMovieMenuEntry(mov, EditMenu, true));

                //remove empty menus
                foreach (KeyValuePair<string, ToolStripMenuItem> CatMenu in CategoryMenus)
                    if (CatMenu.Value.DropDownItems.Count == 0)
                        mnuIMDBmovies.Items.Remove(CatMenu.Value);

                if (CategoryMenus.ContainsKey(_RecycleCategory) && CategoryMenus[_RecycleCategory].DropDownItems.Count > 0)
                {
                    CategoryMenus[_RecycleCategory].DropDownItems.Add(new ToolStripSeparator());
                    CategoryMenus[_RecycleCategory].DropDownItems.Add("Empty " + _RecycleCategory).Click += new EventHandler(EmptyRecycleBin_Click);
                }
            }
        }

        void EmptyRecycleBin_Click(object sender, EventArgs e)
        {
            _MainMoviesForm.Values["action"] = "Empty Recycle Bin";
            _MainMoviesForm.Submit(_Browser);
            
            _MenuTagObject = new object(); //need to rebuild menu
            //remove all "Recycle Bin" items
            for (int i = _MyMovs.Count - 1; i >= 0; i--)
                if (_MyMovs[i].Category == _RecycleCategory)
                    _MyMovs.RemoveAt(i);
        }

        void MovMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem MovMenuItem = sender as ToolStripMenuItem;
            ToolStripItem Parent = MovMenuItem;
            List<ToolStripItem> TempTags = new List<ToolStripItem>();
            while (Parent.OwnerItem != null)
            {
                Parent = Parent.OwnerItem;
                Parent.Tag = MovMenuItem.Tag; //assign temp tag
                TempTags.Add(Parent); //add to temp tag list
            }

            //now let parent (SearchControl) handle the click
            Parent.PerformClick();

            foreach (ToolStripItem TempTag in TempTags)
                TempTag.Tag = null; //remove temp tag
        }

        void mnuMoveToCategory_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IMDBmovie mov = _mnuMoveToCategory.OwnerItem.OwnerItem.Tag as IMDBmovie;
            if (mov != null)
                foreach (ToolStripButton CatMenuItem in _mnuMoveToCategory.Items)
                    CatMenuItem.Checked = (CatMenuItem.Text == mov.Category);
        }

        void mnuMoveToCategory_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            IMDBmovie mov = _mnuMoveToCategory.OwnerItem.OwnerItem.Tag as IMDBmovie;
            if (mov != null)
                MoveMovieToCategory(mov, e.ClickedItem.Text);
        }


        #endregion // IMDB Menu Building
    }

#endregion // IMDB's My Movies

}
