using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading;

namespace NzbSearcher
{
    public delegate void AutoEpisodeChangedEvent(AutoEpisode ep, int ListIndex);

    [Serializable]
    public class AutoEpisode
    {
        public AutoEpisode()
        {
            //Set defaults
            SeasonNrMinDigits = 2;
            EpisodeNrMinDigits = 2;
        }

        public string DisplayName { get; set; }

        public string SearchProviderType { get; set; }
        public string SearchText { get; set; }
        
        public int MinSize { get; set; }
        public int MaxSize { get; set; }

        public string FriendlyName { get; set; }

        public int SeasonNr { get; set; }
        public int SeasonNrMinDigits { get; set; }
        public int EpisodeNr { get; set; }
        public int EpisodeNrMinDigits { get; set; }

        public long LastCheck { get; set; }
        public long LastDownload { get; set; }

        public string SABcat { get; set; }

        public string ResultFilter { get; set; }

        public TimeSpan GetLastCheckAge()
        {
            if (LastCheck == 0)
                return new TimeSpan(365, 0, 0, 0); //return one year old
            return (DateTime.Now - new DateTime(LastCheck));
        }
        
        public TimeSpan GetLastDownloadAge()
        {
            if (LastDownload == 0)
                return new TimeSpan(365, 0, 0, 0); //return one year old
            return (DateTime.Now - new DateTime(LastDownload));
        }
        
        public AutoEpisode Clone()
        {
            return (AutoEpisode)this.MemberwiseClone();
        }

        public void FillSearchParams(SearchSettings SearchParams)
        {
            SearchParams.DisplayName = DisplayName;
            SearchParams.MinSize = MinSize;
            SearchParams.MaxSize = MaxSize;
            SearchParams.Text = ReplaceVars(SearchText);
            //SearchParams.SABcat = SABcat;
        }

        public string ReplaceVars(string Str)
        {
            return ReplaceVars(Str, null);
        }

        public string ReplaceVars(string Str, NzbItem nzb)
        {
            Str = Str.Replace("%E", string.Format("{0:D" + this.EpisodeNrMinDigits + "}", this.EpisodeNr));
            Str = Str.Replace("%S", string.Format("{0:D" + this.SeasonNrMinDigits + "}", this.SeasonNr));
            Str = Str.Replace("%N", this.DisplayName);

            if (Str.Contains("%T"))
            {
                if (nzb == null)
                {
                    Str = Str.Replace("%T", "<title>");
                }
                else if (nzb.Title != null)
                {
                    string[] SearchParts = ReplaceVars(SearchText.Replace("%T", ""), null).Split(new char[] { ' ' },  StringSplitOptions.RemoveEmptyEntries);
                    int TitlePos = 0;
                    foreach(string searchPart in SearchParts)
                    {
                        int pos = nzb.Title.IndexOf(searchPart, StringComparison.OrdinalIgnoreCase);
                        if (pos > TitlePos)
                            TitlePos = pos + searchPart.Length;
                    }

                    string Title = Global.RemoveNonAlfaNumeric(nzb.Title.Substring(TitlePos));
                    Str = Str.Replace("%T", Title);
                    Str = Global.RemoveMultiOccurrances(Str);
                }
            }

            return Str;
        }

    }

    [Serializable]
    public class AutoEpisodesCollection : List<AutoEpisode>
    {
        public AutoEpisodesCollection() : base() { }
        public AutoEpisodesCollection(IEnumerable<AutoEpisode> col) : base(col) { }

        /// <summary>
        /// Adds new items in alfabetical order
        /// new added item will overwrite existing ones if same name
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        public new int Add(AutoEpisode ep)
        {
            int Index = 0;
            while(Index <  this.Count)
            {
                int comp = this[Index].DisplayName.CompareTo(ep.DisplayName);
                if (comp > 0)
                    break;
                if(comp == 0) //exact same name? then remove old item
                {
                    RemoveAt(Index);
                    break;
                }
                Index++;
            }
            Insert(Index, ep);
            return Index;
        }
    }



    public class AutoDownloaderConfig : GUIelementConfig, ICanBeDisabled
    {
        int _Interval = 1; // default value: 1 hour

        public AutoDownloaderConfig()
        {
            DontCheckRecentEp = true;
            DontCheckOldEp = true;
            CheckNextEpisode = true;
            CheckNextSeason = true;
        }

        /// <summary>
        /// Search interval in minutes
        /// </summary>
        public int Interval { get { return _Interval; } set { _Interval = value; FirePropChanged(); } }

        public bool DontCheckRecentEp { get; set; } //6 days
        public bool DontCheckOldEp { get; set; } //1 month
        public bool CheckNextEpisode { get; set; } //after 13 days
        public bool CheckNextSeason { get; set; } // after 2 months
    }

    public class AutoEpisodeDownloader
    {
        public event AutoEpisodeChangedEvent AutoEpisodeChanged;
        
        AutoResetEvent _CheckEpisodesTrigger = new AutoResetEvent(false);

        public AutoEpisodesCollection Episodes { get { return Config<AutoEpisodesCollection>.Value; } }
        public AutoDownloaderConfig Config { get { return Config<AutoDownloaderConfig>.Value; } }

        bool _CheckAllEpisodes = false;
        object _CheckHistoryLock = new object();

        public void Start()
        {
            if (!Config.Disabled)
            {
                Config.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Config_PropertyChanged);
                Global.SABnzbd.StatusUpdated += new StatusUpdatedEvent(SABnzbd_StatusUpdated);
                Program.MainForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(MainForm_FormClosed);


                Thread NewThread = new Thread(new ThreadStart(AutoDownloaderThread));
                NewThread.Name = "AutoDownloader";
                NewThread.Priority = ThreadPriority.BelowNormal;
                NewThread.Start();
            }
        }

        void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _CheckEpisodesTrigger.Set();
        }

        void MainForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _CheckEpisodesTrigger.Set(); //exit thread
        }

        public void CheckNow()
        {
            _CheckAllEpisodes = true;
            _CheckEpisodesTrigger.Set();
        }

        AutoEpisode GetOldestCheckedEpisodeToCheck()
        {
            AutoEpisode OldestEP = null;
            foreach (AutoEpisode ep in Episodes)
            {
                if (Config.DontCheckRecentEp && ep.GetLastDownloadAge().TotalDays < 6)
                    continue; //don't check recent eps
                if (Config.DontCheckOldEp && ep.GetLastDownloadAge().TotalDays > 16 && ep.GetLastCheckAge().TotalHours < 24)
                    continue; //if episode older than 30 days and has been checked today already, skip it
                if (OldestEP == null || ep.LastCheck < OldestEP.LastCheck)
                    OldestEP = ep;
            }
            return OldestEP;
        }

        void AutoDownloaderThread()
        {
            while (!Global.ApplicationIsTerminating)
            {
                AutoEpisode OldestEP = GetOldestCheckedEpisodeToCheck();

                //before checking anything, wait until our interval has expired
                _CheckEpisodesTrigger.Reset(); //make sure we're in unset mode
                int WaitTime = OldestEP != null ? (Config.Interval * 60) - (int)OldestEP.GetLastCheckAge().TotalMinutes : (Config.Interval * 60);
                if (WaitTime > 0)
                    _CheckEpisodesTrigger.WaitOne(WaitTime * 60 * 1000); //wait until next event
                else
                    _CheckEpisodesTrigger.WaitOne(10 * 1000); //wait at least 10 seconds, protect against too fast polling

                if (!Global.ApplicationIsTerminating)
                {
                    if (_CheckAllEpisodes)
                    {
                        _CheckAllEpisodes = false;
                        
                        foreach (AutoEpisode ep in Episodes)
                        {
                            Thread.Sleep(1000);
                            CheckEpisode(ep);
                        }

                        _CheckAllEpisodes = false; //disable it once more (for unpatient users)
                    }
                    else
                    {
                        OldestEP = GetOldestCheckedEpisodeToCheck(); //retrieve once again, the oldest
                        if (OldestEP != null &&  //and also within 10 percent of configured interval?
                            OldestEP.GetLastCheckAge().TotalMinutes > (Config.Interval * 0.9)) 
                                CheckEpisode(OldestEP);
                    }
                }
            }
        }

        public bool IsEpisodeDownloading(AutoEpisode ep)
        {
            foreach (SABnzbdItem item in Global.SABnzbd.Status.QueuedItems)
                if (EpisodeMatchesItem(item, ep))
                    return true;

            return false;
        }

        bool EpisodeMatchesItem(SABnzbdItem item, AutoEpisode ep)
        {
            string FriendlyName = ep.ReplaceVars(ep.FriendlyName.Replace("%T", "")); //filter out %T
            string[] SearchParts = FriendlyName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int PartIndex = 0;
            for (; PartIndex < SearchParts.Length; PartIndex++)
                if (item.FileName.IndexOf(SearchParts[PartIndex], StringComparison.OrdinalIgnoreCase) < 0)
                    return false;

            return true;
        }

        public void CheckEpisode(AutoEpisode ep)
        {
            if (!IsEpisodeDownloading(ep))
            {
                //ISearchProvider prov = (ISearchProvider)Activator.CreateInstance(Type.GetType(ep.SearchProviderType));
                ISearchProvider prov = new Collective(); //new NzbIndex();
                prov.Initialize();

                ep.FillSearchParams(prov.SearchParams);

                prov.SearchResultsUpdated += new ResultsUpdatedEvent(prov_SearchResultsUpdated);
                prov.Tag = ep;
                prov.Search_Synchronous(); //use the synchronous version
                prov.SearchResultsUpdated -= prov_SearchResultsUpdated; //its safe to unhook now
            }
        }

        void prov_SearchResultsUpdated(ISearchProvider provider, UpdateReason Reason, string ErrorMessage)
        {
            //heiko: even if we had an error,  there may still be results (since we're checking more than a single provider now)
            //if (ErrorMessage != null && ErrorMessage.Length > 0)
            //    return; //we had an error, do nothing

            AutoEpisode ep = provider.Tag as AutoEpisode;
            if (ep == null)
                return; //we couldn't find the episode class belonging to this provider

            //instantiate regular expression if needed
            Regex ResultFilter = null;
            if (ep.ResultFilter != null && ep.ResultFilter.Length > 0)
            {
                try { ResultFilter = new Regex(ep.ReplaceVars(ep.ResultFilter)); }
                catch { }
            }

            ep.LastCheck = DateTime.Now.Ticks;
            
            NzbItem DownloadItem = null;
            for (int i = 0; i < provider.SearchResultItemCount; i++)
            {
                NzbItem item = provider[i];
                if (DownloadItem == null || item.PostedDate < DownloadItem.PostedDate)
                {
                    if (!item.HasBeenDownloaded && !item.HasMissingParts)
                    {
                        if (ResultFilter == null || ResultFilter.IsMatch(item.Title))
                            DownloadItem = item;
                    }
                }
            }

            if (DownloadItem != null)
            {
                if (!Episodes.Contains(ep))
                    Episodes.Add(ep); //will remove old version

                try
                {
                    //add to download queue
                    string FileName = provider.DownloadNZB(DownloadItem);
                    string FriendlyName = ep.ReplaceVars(ep.FriendlyName, DownloadItem);
                    if (Global.HandleDownloadedNZB(FileName, FriendlyName, ep.SABcat) == NzbHandling.UploadNZB)
                    {
                        //File uploaded to SABnzbd, no longer need this file
                        System.IO.File.Delete(FileName);
                    }
                    else
                    {
                        ep.EpisodeNr++; //no success checking from SABnzbd, simply increase version nr
                        if (AutoEpisodeChanged != null)
                            Global.InvokeOnGUI(AutoEpisodeChanged, ep, Episodes.IndexOf(ep));
                    }
                    
                    provider.MarkAsDownloaded(DownloadItem);
                    ep.LastDownload = DownloadItem.PostedDate.Ticks;
                    NzbSearcher.Config.Save(); //make sure we store our current state
                }
                catch (Exception) { /* not successful */ }
            }
            else
            {
                //not found? then try next ep, and new season

                //We were just searching with original? then search next ep in this season
                if (Episodes.Contains(ep)) 
                {
                    if (Config.CheckNextEpisode)
                    {
                        TimeSpan LastDownloadAge = DateTime.Now - new DateTime(ep.LastDownload);
                        if (LastDownloadAge.TotalDays >= 13)
                        {
                            //Search for next ep
                            ep = ep.Clone();
                            ep.EpisodeNr++;
                            ep.FillSearchParams(provider.SearchParams);
                            provider.Tag = ep;
                            provider.Search_Synchronous();
                        }
                    }
                    else if (Config.CheckNextSeason)
                    {
                        TimeSpan LastDownloadAge = DateTime.Now - new DateTime(ep.LastDownload);
                        if (LastDownloadAge.TotalDays >= 61)
                        {
                            //we were already searching next ep, now search for new season
                            ep.EpisodeNr = 1;
                            ep.SeasonNr++;
                            ep.FillSearchParams(provider.SearchParams);
                            provider.Search_Synchronous();
                        } 
                    }
                }
                else if (ep.EpisodeNr != 1) //only search new season if we're not on 1st ep already
                {
                    if (Config.CheckNextSeason)
                    {
                        TimeSpan LastDownloadAge = DateTime.Now - new DateTime(ep.LastDownload);
                        if (LastDownloadAge.TotalDays >= 61)
                        {
                            //we were already searching next ep, now search for new season
                            ep.EpisodeNr = 1;
                            ep.SeasonNr++;
                            ep.FillSearchParams(provider.SearchParams);
                            provider.Search_Synchronous();
                        }
                    }
                }
            }

            if (AutoEpisodeChanged != null && Episodes.Contains(ep))
            {
                try { Global.InvokeOnGUI(AutoEpisodeChanged, ep, Episodes.IndexOf(ep)); }
                catch { }
            }
        }

        void SABnzbd_StatusUpdated(SABnzbdStatus status)
        {
            if (!status.HistoryItems.HasChanged) //no need to check if not changed
                return;

            Thread NewThread = new Thread(new ParameterizedThreadStart(CheckHistoryThread));
            NewThread.Name = "CheckHistory";
            NewThread.Start(status);
        }

        void CheckHistoryThread(object StatusObj)
        {
            SABnzbdStatus status = (SABnzbdStatus)StatusObj;

            lock (_CheckHistoryLock) //wait for other check to complete
            {
                //go through all history items
                foreach (SABnzbdItem HistoryItem in status.HistoryItems)
                {
                    bool HasFailed = (HistoryItem.FailMessage != null && HistoryItem.FailMessage.Length > 0) || HistoryItem.Status.StartsWith("Fail");
                    bool IsCompleted = !HasFailed && HistoryItem.Status.StartsWith("Completed");

                    if (HasFailed || IsCompleted) //only try matching episode if completely done
                    {
                        AutoEpisodesCollection eps = new AutoEpisodesCollection(Episodes); //make copy or array before doing this
                        foreach (AutoEpisode ep in eps)
                        {
                            if (EpisodeMatchesItem(HistoryItem, ep)) //we found a matching episode
                            {
                                if (IsCompleted)
                                {
                                    ep.EpisodeNr++; // current episode nr has been completed, goto next!
                                    NzbSearcher.Config.Save(); //make sure we store our current state

                                    if (AutoEpisodeChanged != null)
                                        Global.InvokeOnGUI(AutoEpisodeChanged, ep, Episodes.IndexOf(ep));
                                }

                                CheckEpisode(ep); //if failed, retry, if completed, try next
                            }
                        }
                    }
                }
            }

        }


    }
}
