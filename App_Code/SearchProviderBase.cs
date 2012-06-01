using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

using System.Net;
using System.IO;
using System.Reflection;

namespace NzbSearcher
{
    public class ProviderConfig<T> : GUIelementConfig, IProviderConfig where T : SearchSettings
    {
        CastableList<T, SearchSettings> _Favorites = new CastableList<T, SearchSettings>();
        List<string> _SearchHistory = new List<string>();
        T _CurrentSearch = Activator.CreateInstance<T>();

        public T CurrentSearch { get { return _CurrentSearch; } set { _CurrentSearch = value; } }
        public List<T> Favorites { get { return _Favorites; } }
        public IList<string> SearchHistory { get { return _SearchHistory; } }

        bool _UseForCollectiveSearch = true; //default value
        public bool UseForCollectiveSearch { get { return _UseForCollectiveSearch; } set { _UseForCollectiveSearch = value; } }

        SearchSettings IProviderConfig.CurrentSearch { get { return this.CurrentSearch; } set { this.CurrentSearch = (T)value; } }
        IList<SearchSettings> IProviderConfig.Favorites { get { return _Favorites; } }
    }

    public abstract class SearchProviderBase : ISearchProvider
    {
        //abstracts

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract string ToolTip { get; }
        public abstract string HistoryName { get; }
        public abstract Image Icon { get; }
        public abstract SearchSettings SearchParams { get; set; }
        public abstract bool IsItemFromProvider(NzbItem item);
        
        public abstract IProviderConfig Config { get; }

        public virtual string GetNzbInfo(NzbItem item) { return null; }
        public virtual void SetSorting(SortColumn col) { FireResultsUpdated(UpdateReason.Sorting, null); }
        public virtual SortColumn GetSorting() { return SortColumn.Age; }
        public virtual void CreateAdditionalControls(Panel parent) { /* no controls */ }

        protected virtual void Search(object bNextPage) { FireResultsUpdated(UpdateReason.NewSearch, null); }
        protected virtual CookieContainer Cookies { get { return null; } }

        //Events

        public event ResultsUpdatedEvent SearchResultsUpdated;
        public event DownloadCompletedEvent DownloadCompleted;

        //Members

        protected NzbHistory _History = null;
        protected NzbItemsCollection _SearchResult = new NzbItemsCollection();
        protected bool _bMoreAvailable = false;

        //Properties

        public object Tag { get; set; }
        public int SearchResultItemCount { get { return _SearchResult.Count; } }
        public NzbItem this[int index] { get { return _SearchResult[index]; } }
        

        public virtual void Initialize()
        {
            if (_History == null)
            {
                _History = NzbHistory.GetHistory(this.HistoryName);
                SearchParams = Config.CurrentSearch;
                NzbSearcher.Config.Saving += new ConfigEvent(Config_Saving);
            }
        }

        void Config_Saving()
        {
            //store current search params
            Config.CurrentSearch = SearchParams;
        }

        public virtual void Search_Synchronous()
        {
            Search(null);
        }
        
        public virtual void Search()
        {
            Thread NewSearchThread = new Thread(new ParameterizedThreadStart(Search));
            NewSearchThread.Name = "Search";
            NewSearchThread.Priority = ThreadPriority.BelowNormal;
            NewSearchThread.Start(null);
        }

        public virtual void SearchMore()
        {
            if (_bMoreAvailable)
            {
                _bMoreAvailable = false;
                Thread NewSearchThread = new Thread(new ParameterizedThreadStart(Search));
                NewSearchThread.Name = "Search";
                NewSearchThread.Priority = ThreadPriority.BelowNormal;
                NewSearchThread.Start(new object());
            }
        }

        public virtual string GetIMDBurl(NzbItem item)
        {
            string NFO = GetNzbInfo(item);
            if (NFO == null || NFO.Length == 0)
                return null;

            int Pos1 = NFO.IndexOf("imdb.com/title", StringComparison.OrdinalIgnoreCase);
            if (Pos1 > 0)
            {
                int Pos2 = Pos1;
                while (Pos2 < NFO.Length && NFO[Pos2] > 32 && NFO[Pos2] != '\"' && NFO[Pos2] != '\'')
                    Pos2++;

                return "http://www." + NFO.Substring(Pos1, Pos2 - Pos1);
            }

            return null;
        }

        protected void FireResultsUpdated(UpdateReason Reason, string ErrorMessage)
        {
            if (SearchResultsUpdated != null)
            {
                if (Program.MainForm.InvokeRequired)
                    Global.InvokeOnGUI(SearchResultsUpdated, this, Reason, ErrorMessage);
                else
                    SearchResultsUpdated(this, Reason, ErrorMessage);
            }
        }

        public void MarkAsDownloaded(NzbItem item)
        {
            item.HasBeenDownloaded = true;
            _History.AddHistory(item.UniqueID);
        }

        /// <summary>
        /// filters a unique ID from a string, the firt nummeric value within it is taken
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        protected long GetUniqueID(string URL)
        {
            string UniqueID = string.Empty;
            foreach (char Chr in URL)
            {
                if (Chr >= '0' && Chr <= '9')
                    UniqueID += Chr;
                else if (UniqueID.Length > 0)
                    break;
            }
            return long.Parse(UniqueID);
        }

        public virtual void ScreenToProvider()
        {

        }

        public virtual void ProviderToScreen()
        {

        }

        public virtual void ClearSearchResults()
        {
            _SearchResult = new NzbItemsCollection();
        }

        public abstract void NewSearchParams();

        protected virtual string GetNzbItemDownloadURL(NzbItem item)
        {
            return item.DownloadLink;
        }

        public void DownloadNZB_Async(NzbItem item)
        {
            Thread NewSearchThread = new Thread(new ParameterizedThreadStart(DownloadNzbThread));
            NewSearchThread.Name = "DownloadNzb";
            NewSearchThread.Priority = ThreadPriority.BelowNormal;
            NewSearchThread.Start(item);
        }

        protected void DownloadNzbThread(object item)
        {
            string ErrorMessage = null;
            string TempFileName = null;

            try
            {
                TempFileName = DownloadNZB((NzbItem)item);
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;
            }
            
            if (DownloadCompleted != null)
                Global.InvokeOnGUI(DownloadCompleted, this, (NzbItem)item, TempFileName, ErrorMessage);
        }

        public virtual string DownloadNZB(NzbItem item)
        {
            string FileName = "NzbSearcher_" + this.DisplayName + "_" + item.UniqueID.ToString() + ".nzb";
            string TempFileName = Path.Combine(Path.GetTempPath(), FileName);
            if (!File.Exists(TempFileName))
            {
                try
                {
                    using (FileStream fs = File.Open(TempFileName, FileMode.CreateNew))
                    {
                        string DownloadURL = GetNzbItemDownloadURL(item);
                        HttpWebRequest SearchRequest = (HttpWebRequest)HttpWebRequest.Create(DownloadURL);
                        SearchRequest.CookieContainer = Cookies;
                        SearchRequest.Timeout = 10000;

                        using (WebResponse resp = SearchRequest.GetResponse())
                        using (Stream respStream = resp.GetResponseStream())
                            Global.CopyStream(respStream, fs);
                    }
                }
                catch
                {
                    File.Delete(TempFileName); // always delete failed download
                    throw;
                }
            }

            return TempFileName;
        }

    }
}
