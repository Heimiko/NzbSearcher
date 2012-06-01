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

namespace NzbSearcher
{
    public class CollectiveConfig : ProviderConfig<SearchSettings_Collective>
    {

    }

    [Serializable]
    public class SearchSettings_Collective : SearchSettings
    {
        public SortColumn Sort { get; set; }
    }

    public class Collective : ISearchProvider
    {
        public event ResultsUpdatedEvent SearchResultsUpdated;
        public event DownloadCompletedEvent DownloadCompleted;

        SearchSettings_Collective _SearchParams = new SearchSettings_Collective();

        //Properties
        public Image Icon { get { return Properties.Resources.AllProviders; } }
        public string Name { get { return "Collective"; } }
        public string DisplayName { get { return "All"; } }
        public string ToolTip { get { return "Collective Search Providers"; } }
        public string HistoryName { get { return "Collective"; } }
        public object Tag { get; set; }

        public SearchSettings SearchParams { get { return _SearchParams; } set { _SearchParams = (SearchSettings_Collective)value; } }
        public bool IsItemFromProvider(NzbItem item) { return false; }
        public IProviderConfig Config { get { return Config<CollectiveConfig>.Value; } }

        public int SearchResultItemCount { get { return _SearchResult.Count; } }

        List<ISearchProvider> _Providers = new List<ISearchProvider>();
        List<ISearchProvider> _ProviderResults = new List<ISearchProvider>();
        NzbItemsCollection _SearchResult = new NzbItemsCollection();

        bool _DoingSynchronousSearch = false;
        string _SearchErrorMessage = null;

        /// <summary>
        /// initializes the provider, loads history, and such
        /// </summary>
        public void Initialize()
        {
            NzbSearcher.Config.Saving += new ConfigEvent(Config_Saving);
            SearchParams = Config.CurrentSearch;

            //create all instances of providers which are configured to be enabled
            foreach (ISearchProvider prov in SearchProvidersCollection.Providers)
            {
                if (!(prov is Collective) && prov.Config.UseForCollectiveSearch)
                {
                    ISearchProvider NewProv = (ISearchProvider) Activator.CreateInstance(prov.GetType());

                    NewProv.Initialize();
                    NewProv.SearchResultsUpdated += new ResultsUpdatedEvent(prov_SearchResultsUpdated);
                    NewProv.DownloadCompleted += new DownloadCompletedEvent(prov_DownloadCompleted);
                    
                    _Providers.Add(NewProv);
                }
            }
        }

        void Config_Saving()
        {
            //store current search params
            Config.CurrentSearch = SearchParams;
        }

        void CopySearchParams()
        {
            foreach (ISearchProvider prov in _Providers)
            {
                prov.NewSearchParams(); //always start clean

                prov.SearchParams.MaxDays = _SearchParams.MaxDays;
                prov.SearchParams.MaxSize = _SearchParams.MaxSize;
                prov.SearchParams.MinDays = _SearchParams.MinDays;
                prov.SearchParams.MinSize = _SearchParams.MinSize;
                //prov.SearchParams.SABcat = _SearchParams.SABcat;
                prov.SearchParams.Text = _SearchParams.Text;
            }
        }

        /// <summary>
        /// Search with current provided search parameters
        /// </summary>
        public void Search_Synchronous()
        {
            _DoingSynchronousSearch = true;
            _SearchErrorMessage = null;
            _SearchResult.Clear();

            CopySearchParams();
            foreach (ISearchProvider prov in _Providers)
            {
                if (prov is NzbMatrix && !IsAlfaNumericString(_SearchParams.Text))
                    continue; // NzbMatrix gives WEIRD results when used with non-alfa-numeric chars

                prov.Search_Synchronous();
                AddResultsFromProvider(prov);
            }

            _DoingSynchronousSearch = false;
            
            //call within the SAME thread!!  do NOT post this on the GUI thread
            if (SearchResultsUpdated != null)
                SearchResultsUpdated(this, UpdateReason.NewSearch, _SearchErrorMessage);
        }

        /// <summary>
        /// Search with current provided search parameters
        /// should ALWAYS fire SearchResultsUpdated when done (either with error or not)
        /// </summary>
        public void Search()
        {
            _SearchErrorMessage = null;
            _ProviderResults = new List<ISearchProvider>();
            _SearchResult.Clear();
            CopySearchParams();
            foreach (ISearchProvider prov in _Providers)
                prov.Search(); //start a new thread for every provider (faster results)
        }

        void AddResultsFromProvider(ISearchProvider prov)
        {
            for(int i = 0; i < prov.SearchResultItemCount; i++)
                AddNzbItemToResult(prov[i]);
        }

        double ParseDouble(string s)
        {
            return double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
        }

        double GetItemSizeInKB(NzbItem item)
        {
            Match SizeMatch = Regex.Match(item.Size, "^([0-9.,]*) ?([A-Z])", RegexOptions.IgnoreCase);
            double Size = -1;

            if (SizeMatch.Success)
            {
                Size = ParseDouble(SizeMatch.Groups[1].Value);
                string Measurement = SizeMatch.Groups[2].Value;
                switch (Measurement.ToUpper())
                {
                    case "K": //already in KB
                        break;
                    case "M":
                        Size *= 1024;
                        break;
                    case "G":
                        Size *= 1024 * 1024;
                        break;
                }
            }

            return Size;
        }

        double GetItemAgeInMinutes(NzbItem item)
        {
            Match AgeMatch = Regex.Match(item.Age, "^([0-9.,]*) ?([A-Z])", RegexOptions.IgnoreCase);
            double Age = -1;
            if (AgeMatch.Success)
            {
                Age = ParseDouble(AgeMatch.Groups[1].Value);
                string Measurement = AgeMatch.Groups[2].Value;
                switch (Measurement.ToUpper())
                {
                    case "M": //already in minutes
                        break;
                    case "H":
                        Age *= 60;
                        break;
                    case "D":
                        Age *= 60 * 24;
                        break;
                }
            }
            return Age;
        }

        int CompareNzbItems(NzbItem x, NzbItem y)
        {
            int result = 0;

            switch ((SortColumn)((int)_SearchParams.Sort & 0xF))
            {
                case SortColumn.Age:
                    double AgeDiff = GetItemAgeInMinutes(x) - GetItemAgeInMinutes(y);
                    result = AgeDiff > int.MaxValue ? int.MaxValue : AgeDiff < int.MinValue ? int.MinValue : (int)AgeDiff;
                    break;
                case SortColumn.FileName:
                    result = x.Title.CompareTo(y.Title);
                    break;
                case SortColumn.Size:
                    double SizeDiff = GetItemSizeInKB(x) - GetItemSizeInKB(y);
                    result = SizeDiff > int.MaxValue ? int.MaxValue : SizeDiff < int.MinValue ? int.MinValue : (int)SizeDiff;
                    break;
            }

            if ((_SearchParams.Sort & SortColumn.Descending) != 0)
                return (result * -1); // invert result

            return result;
        }

        bool IsAlfaNumericString(string str)
        {
            foreach(char chr in str)
            {
                if (chr == ' ')
                    continue;
                if (chr >= '0' && chr <= '9') 
                    continue;
                if (chr >= 'a' && chr <= 'z')
                    continue;
                if (chr >= 'A' && chr <= 'Z')
                    continue;
                return false;
            }
            return true;
        }

        void AddNzbItemToResult(NzbItem item) 
        {
            if (_SearchResult.Contains(item))
                return; //no need to add

            //see if item should be filtered (Size)
            if (_SearchParams.MinSize != 0 || _SearchParams.MaxSize != 0)
            {
                double ItemSizeInMB = GetItemSizeInKB(item) / 1024;
                if (ItemSizeInMB < _SearchParams.MinSize)
                    return; //this item should be filtered
                if (_SearchParams.MaxSize != 0 && ItemSizeInMB > _SearchParams.MaxSize)
                    return; //this item should be filtered
            }

            //see if item should be filtered (Age)
            if (_SearchParams.MinDays != 0 || _SearchParams.MaxDays != 0)
            {
                double ItemDays = GetItemAgeInMinutes(item) / (60 * 24);
                if (ItemDays < _SearchParams.MinDays)
                    return; //this item should be filtered
                if (_SearchParams.MaxDays != 0 && ItemDays > _SearchParams.MaxDays)
                    return; //this item should be filtered
            }

            //Make sure the item is a propper search result
            string[] SearchParts = _SearchParams.Text.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string ItemTitle = item.Title.ToLower();
            foreach (string SearchPart in SearchParts)
            {
                if (IsAlfaNumericString(SearchPart) && ItemTitle.IndexOf(SearchPart) < 0)
                    return; //keyword is not found in this NzbItem
            }

            //Find out where to insert the item
            int i = 0;
            for (; i < _SearchResult.Count; i++)
                if (CompareNzbItems(item, _SearchResult[i]) < 0)
                    break;

            item.Description = GetProviderForItem(item).Name; //change description
            _SearchResult.Insert(i, item); //insert the item (sorted!)
        }

        void prov_SearchResultsUpdated(ISearchProvider provider, UpdateReason Reason, string ErrorMessage)
        {
            if (ErrorMessage != null && ErrorMessage.Length > 0) //accumulate error messages
            {
                if (_SearchErrorMessage == null)
                    _SearchErrorMessage = string.Empty;
                else
                    _SearchErrorMessage += "\r\n";

                _SearchErrorMessage += provider.Name + ": " + ErrorMessage;
            }

            if (_DoingSynchronousSearch)
                return; // do none of this fancy stuff when doing synchronus search

            if (provider.SearchResultItemCount > 0)
                AddResultsFromProvider(provider);

            if (!_ProviderResults.Contains(provider))
                _ProviderResults.Add(provider);

            if (_ProviderResults.Count == _Providers.Count) //all providers reported back? then now we need to see if we need to fire an extra event
                FireResultsUpdated(Reason, _SearchErrorMessage);
        }

        public void SearchMore() 
        { 
            /* no implementation */ 
        }

        /// <summary>
        /// Set sorting column for current search result
        /// should ALWAYS fire SearchResultsUpdated when done (either with error or not)
        /// </summary>
        /// <param name="col"></param>
        public void SetSorting(SortColumn col)
        {
            _SearchParams.Sort = col;
            //_SearchResult.Sort(CompareNzbItems);

            _SearchErrorMessage = null;
            _ProviderResults = new List<ISearchProvider>();
            _SearchResult.Clear();

            foreach (ISearchProvider prov in _Providers)
                prov.SetSorting(col); //start a new thread for every provider (faster results)

            //FireResultsUpdated(UpdateReason.Sorting, null);
        }

        /// <summary>
        /// retrieves item from result list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public NzbItem this[int index] { get { return _SearchResult[index]; } }

        public SortColumn GetSorting() { return _SearchParams.Sort; }

        public void ClearSearchResults() 
        {
            _SearchResult.Clear(); 
        }

        public void NewSearchParams() 
        { 
            _SearchParams = new SearchSettings_Collective();
            _SearchResult = new NzbItemsCollection();
        }

        public void CreateAdditionalControls(Panel parent) 
        { 
            /* nothing */ 
        }
        
        public void ScreenToProvider() 
        { 
            /* nothing */
        }
        
        public void ProviderToScreen() 
        { 
            /* nothing */ 
        }

        ISearchProvider GetProviderForItem(NzbItem item)
        {
            foreach (ISearchProvider prov in _Providers)
                if (prov.IsItemFromProvider(item))
                    return prov;

            return null;
        }

        public string GetNzbInfo(NzbItem item)
        {
            return GetProviderForItem(item).GetNzbInfo(item);
        }

        public string GetIMDBurl(NzbItem item)
        {
            return GetProviderForItem(item).GetNzbInfo(item);
        }

        public void MarkAsDownloaded(NzbItem item)
        {
            GetProviderForItem(item).MarkAsDownloaded(item);
        }

        public string DownloadNZB(NzbItem item)
        {
            return GetProviderForItem(item).DownloadNZB(item);
        }

        public void DownloadNZB_Async(NzbItem item)
        {
            GetProviderForItem(item).DownloadNZB_Async(item);
        }

        void prov_DownloadCompleted(ISearchProvider provider, NzbItem item, string TempFileName, string ErrorMessage)
        {
 	        if (DownloadCompleted != null)
                DownloadCompleted(this, item, TempFileName, ErrorMessage);
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
    }
}
