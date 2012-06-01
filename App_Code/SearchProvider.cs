using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace NzbSearcher
{
    [Flags]
    public enum SortColumn
    {
        FileName = 0x1,
        Description = 0x2,
        Age = 0x4,
        Size = 0x8,
        Availability = 0x10,

        Descending = 0xF00
    }

    public enum UpdateReason
    {
        NewSearch,
        MoreSearch,
        Sorting
    }

    public delegate void ResultsUpdatedEvent(ISearchProvider provider, UpdateReason Reason, string ErrorMessage);
    public delegate void DownloadCompletedEvent(ISearchProvider provider, NzbItem item, string TempFileName, string ErrorMessage);

    public class SearchProvidersCollection : List<ISearchProvider>
    {
        private SearchProvidersCollection(IEnumerable<ISearchProvider> collection) : base(collection) { }

        /// <summary>
        /// Here we define all our search providers
        /// </summary>
        static SearchProvidersCollection _SearchProdivers = new SearchProvidersCollection(new ISearchProvider[]
        {
            new Collective(),
            new NzbIndex(),
            new NzbMatrix(),
            new YabSearch(),
            new NZBsu(),
            new UsenetServer()
        });

        public static SearchProvidersCollection Providers { get { return _SearchProdivers; } }

        public static T GetProvider<T>() where T : ISearchProvider
        {
            foreach (ISearchProvider prov in _SearchProdivers)
                if (prov is T)
                    return (T)prov;
            return default(T);
        }
    }

    [Serializable]
    public class SearchSettings
    {
        public string Text { get; set; }
        public int MinDays { get; set; }
        public int MaxDays { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }

        /// <summary>
        /// This is the display Name when stored as a favorite
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// When stored as a favorite, indicates latest downloaded article, in Ticks
        /// </summary>
        public long LatestDownload { get; set; }

        public SearchSettings Clone() { return (SearchSettings)this.MemberwiseClone(); }

        public virtual bool SupportsMaxDays { get { return true; } }
        public virtual bool SupportsMinSize { get { return true; } }
    }

    public interface IProviderConfig : IGUIelementConfig, ICanBeDisabled
    {
        SearchSettings CurrentSearch { get; set; }
        IList<SearchSettings> Favorites { get; }
        IList<string> SearchHistory { get; }
        bool UseForCollectiveSearch { get; set; }
    }

    public interface ISearchProvider
    {
        event ResultsUpdatedEvent SearchResultsUpdated;
        event DownloadCompletedEvent DownloadCompleted;

        Image Icon { get; }
        string Name { get; }
        string DisplayName { get; }
        string ToolTip { get; }
        string HistoryName { get; }

        /// <summary>
        /// initializes the provider, loads history, and such
        /// </summary>
        void Initialize();

        int SearchResultItemCount { get; }
        SearchSettings SearchParams { get; set; }
        IProviderConfig Config { get; }

        object Tag { get; set; }

        /// <summary>
        /// Search with current provided search parameters
        /// </summary>
        void Search_Synchronous(); 

        /// <summary>
        /// Search with current provided search parameters
        /// should ALWAYS fire SearchResultsUpdated when done (either with error or not)
        /// </summary>
        void Search();
        void SearchMore(); //Fetches more items from last search

        /// <summary>
        /// Set sorting column for current search result
        /// should ALWAYS fire SearchResultsUpdated when done (either with error or not)
        /// </summary>
        /// <param name="col"></param>
        void SetSorting(SortColumn col);

        /// <summary>
        /// retrieves item from result list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        NzbItem this[int index] { get; }

        SortColumn GetSorting();

        void ClearSearchResults();
        void NewSearchParams();
        void CreateAdditionalControls(Panel parent);
        void ScreenToProvider();
        void ProviderToScreen();

        string GetNzbInfo(NzbItem item);
        string GetIMDBurl(NzbItem item);
        bool IsItemFromProvider(NzbItem item);
        void MarkAsDownloaded(NzbItem item);

        /// <summary>
        /// returns the name of the temp file which has been downloaded
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string DownloadNZB(NzbItem item);

        /// <summary>
        /// Always fires DownloadCompleted when finished
        /// </summary>
        /// <param name="item"></param>
        void DownloadNZB_Async(NzbItem item);
    }

    public abstract class NzbItem
    {
        static ColorConverter ColConv = new ColorConverter();

        public static Color NewBackgroundColor = (Color)ColConv.ConvertFrom("#C6FFC6"); //green-ish
        public static Color DownloadedBackgroundColor = Color.White;
        public static Color ErrorBackgroundColor = (Color)ColConv.ConvertFrom("#FFC3C6"); //red-ish

        public object UniqueID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string DownloadLink { get; set; }
        public string Size { get; set; }
        public string Age { get; set; }

        public DateTime PostedDate { get; set; }

        public bool HasBeenDownloaded { get; set; }
        public bool HasMissingParts { get; set; }
        public virtual Color BackgroundColor
        {
            get
            {
                if (HasBeenDownloaded) // downloaded before?
                    return NzbItem.DownloadedBackgroundColor;
                else if (HasMissingParts) // has missing parts?
                    return NzbItem.ErrorBackgroundColor;
                else
                    return NzbItem.NewBackgroundColor;
            }
        }

        public int PercentAvailable { get; set; }
        public virtual bool HasNFO { get { return false; } }
    }

    public class NzbItemsCollection : List<NzbItem>
    {
        public NzbItemsCollection() { }
        public NzbItemsCollection(IEnumerable<NzbItem> collection) : base(collection) { }
    }

    public class NzbHistory
    {
        static Dictionary<string, NzbHistory> _AllHistory = new Dictionary<string, NzbHistory>();

        NzbHistoryFile _File = null;
        Dictionary<long, bool> _History = null; //new Dictionary<long, bool>();
        Dictionary<string, bool> _SlowHistory = null; //new Dictionary<string, bool>();

        private NzbHistory(string ProviderName)
        {
            _File = new NzbHistoryFile(ProviderName);
        }

        public static NzbHistory GetHistory(string ProviderName)
        {
            if (_AllHistory.ContainsKey(ProviderName))
                return _AllHistory[ProviderName];

            NzbHistory h = new NzbHistory(ProviderName);
            _AllHistory.Add(ProviderName, h);
            return h;
        }

        void InitHistory()
        {
            if (_History == null)
            {
                _History = new Dictionary<long, bool>();
                string UniqueID;
                while ((UniqueID = _File.ReadNextHistoryID()) != null)
                    _History[long.Parse(UniqueID)] = true;
            }
        }

        void InitSlowHistory()
        {
            if (_SlowHistory == null)
            {
                _SlowHistory = new Dictionary<string, bool>();
                string UniqueID;
                while ((UniqueID = _File.ReadNextHistoryID()) != null)
                    _SlowHistory[UniqueID] = true;
            }
        }

        public void AddHistory(object UniqueID)
        {
            if (UniqueID is long)
            {
                InitHistory();
                if (!_History.ContainsKey((long)UniqueID))
                {
                    _History[(long)UniqueID] = true;
                    _File.AddHistory(UniqueID.ToString());
                }
            }
            else
            {
                InitSlowHistory();
                string UniqueString = UniqueID.ToString();
                if (!_SlowHistory.ContainsKey(UniqueString))
                {
                    _SlowHistory[UniqueString] = true;
                    _File.AddHistory(UniqueString);
                }
            }
        }

        public bool ContainsHistory(object UniqueID)
        {
            if (UniqueID is long)
            {
                InitHistory();
                return _History.ContainsKey((long)UniqueID);
            }
            else
            {
                InitSlowHistory();
                return _SlowHistory.ContainsKey(UniqueID.ToString());
            }
        }
    }

    public class NzbHistoryFile
    {
        string FileName;
        StreamReader _Read = null;

        public NzbHistoryFile(string ProviderName)
        {
            FileName = Global.GetStorageDirectory() + "\\History_" + ProviderName + ".txt";
        }

        public string ReadNextHistoryID()
        {
            if (_Read == null)
            {
                if (!File.Exists(FileName))
                    return null;
                _Read = File.OpenText(FileName);
            }
            if (_Read == null)
                return null;
            if (_Read.EndOfStream)
            {
                _Read.Close();
                _Read = null;
                return null;
            }
            
            return _Read.ReadLine();
        }

        public void AddHistory(string UniqueID)
        {
            File.AppendAllText(FileName, UniqueID + "\r\n");
        }
    }

}
