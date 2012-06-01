using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;

using System.Net;
using System.Xml;
using System.Threading;
using System.Reflection;
using System.IO;

namespace NzbSearcher
{
    public delegate void StatusUpdatedEvent(SABnzbdStatus status);

    public enum NzbHandling
    {
        UploadNZB, //default on top!
        SendURL,
        SaveToDir,
        OpenWith
    }

    [Serializable]
    public class SABnzbdConfig : GUIelementConfig
    {
        public SABnzbdConfig()
        { //set default values
            APIkey = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            HostURL = "http://localhost:8080/";
            ActivePollInterval = 1; //interval in seconds
            InactivePollInterval = 60; //interval in seconds
        }

        public string APIkey { get; set; }
        public string HostURL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public int ActivePollInterval { get; set; }
        public int InactivePollInterval { get; set; }
    }

    public class SABnzbdItem
    {
        public string FileName { get; set; }
        public string Directory { get; set; }
        public string Status { get; set; }
        public string TimeLeft { get; set; }
        public string Size { get; set; }
        public string SizeLeft { get; set; }
        public string Age { get; set; }
        public float MB { get; set; }
        public float MBleft { get; set; }
        public string Category { get; set; }
        public int Percentage { get; set; }
        public string nzo_id { get; set; }
        public int Index { get; set; }
        public string FailMessage { get; set; }
        public string Priority { get; set; }

        public Color BackColor { get; set; }
    }

    public class SABnzbdItemsCollection : List<SABnzbdItem>
    {
        public bool HasChanged { get; internal set; }
        public bool HasErrors { get; set; }
    }

    public class SABnzbdStatus
    {
        public SABnzbdStatus()
        {
            QueuedItems = new SABnzbdItemsCollection();
            HistoryItems = new SABnzbdItemsCollection();
            Categories = new List<string>();
        }

        public bool IsPaused { get; internal set; }
        public bool IsDownloading { get { return QueuedItems.Count > 0 || (Speed != null && !Speed.StartsWith("0 ")); } }
        public bool IsExtracting { get; internal set; }

        public float MB { get; set; }
        public float MBleft { get; set; }
        public string TimeLeft { get; set; }
        public string SizeLeft { get; set; }

        public string Speed { get; set; }
        public string DiskSpace { get; set; }
        public int SpeedLimit { get; set; }

        public SABnzbdItemsCollection QueuedItems { get; private set; }
        public SABnzbdItemsCollection HistoryItems { get; set; }
        public List<string> Categories { get; private set; }
    }

    public class SABnzbd
    {
        // Events

        public event StatusUpdatedEvent StatusUpdated;

        // Members

        AutoResetEvent _CommandTrigger = new AutoResetEvent(false);
        SABnzbdStatus _Status = new SABnzbdStatus();
        List<string> _CommandQueue = new List<string>();
        bool _RefreshHistory = false;

        // Properties

        public SABnzbdConfig Config { get { return Config<SABnzbdConfig>.Value; } }
        public SABnzbdStatus Status { get { return _Status; } }
        
        // Functions

        public void Start()
        {
            if (!Config.Disabled)
            {
                //we need to register on program closing event, so we can terminate our thread
                Program.MainForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(MainForm_FormClosed);

                Thread NewThread = new Thread(new ThreadStart(SABnzbd_Thread));
                NewThread.Name = "SABnzbd";
                NewThread.Priority = ThreadPriority.BelowNormal;
                NewThread.Start();
            }
        }

        string GetCommandURL(string Command)
        {
            string strURL = Config.HostURL;
            if (!strURL.EndsWith("/"))
                strURL += '/';
            if (!strURL.EndsWith("/sabnzbd/"))
                strURL += "sabnzbd/";
            strURL += "api?apikey=" + Config.APIkey;
            if (Config.UserName != null && Config.UserName.Length > 0)
                strURL += "&ma_username=" + Config.UserName;
            if (Config.Password != null && Config.Password.Length > 0)
                strURL += "&ma_password=" + Config.Password;
            if (Command != null && Command.Length > 0)
                strURL += "&" + Command;
            return strURL;
        }

        string FilterForbiddenChars(string str)
        {
            char[] NotAllowedChars = new char[] { '\\', ':', '*', '?', '\"', '<', '>', '|', '/' }; //allow '/' for passwords
            return string.Join(" ", str.Split(NotAllowedChars, StringSplitOptions.RemoveEmptyEntries));
        }

        public void AddNzbFromURL(string strURL, string FriendlyName, string Category)
        {
            string Command = "mode=addurl&name=" + strURL;
            if (Category != null && Category.Length > 0)
                Command += "&cat=" + Category;
            if (FriendlyName != null && FriendlyName.Length > 0)
                Command += "&nzbname=" + System.Web.HttpUtility.UrlEncode(FilterForbiddenChars(FriendlyName));

            AddCommandToQueue(Command);
        }

        public void AddNzbByUpload(string FileName, string FriendlyName, string Category)
        {
            if (!Config.Disabled)
            {
                string Command = "mode=addfile&output=xml";
                if (Category != null && Category.Length > 0)
                    Command += "&cat=" + Category;
                if (FriendlyName != null && FriendlyName.Length > 0)
                    Command += "&nzbname=" + System.Web.HttpUtility.UrlEncode(FilterForbiddenChars(FriendlyName));

                string Response = Global.UploadFormFile(GetCommandURL(Command), "name", FileName);
                XmlDocument XML = new XmlDocument();
                XML.LoadXml(Response);
                XmlNode Result = XML.SelectSingleNode("/result/status");
                if (Result != null && !bool.Parse(Result.InnerText))
                    throw new Exception(XML.SelectSingleNode("/result/error").InnerText);

                RefreshQueueStatus();
            }
        }

        public void RemoveFromQueue(SABnzbdItem item)
        {
            string Command = "mode=queue&name=delete&value=" + item.nzo_id;
            AddCommandToQueue(Command);
        }

        public void RemoveFromHistory(SABnzbdItem item)
        {
            string Command = "mode=history&name=delete&value=" + item.nzo_id;
            _RefreshHistory = true;
            AddCommandToQueue(Command);
        }

        public void MoveItem(SABnzbdItem item, int MoveTo)
        {
            string Command = "mode=switch&value=" + item.nzo_id + "&value2=" + MoveTo;
            AddCommandToQueue(Command);
        }

        public void SetSpeedLimit(int KBperSec)
        {
            string Command = "mode=config&name=speedlimit&value=";
            if (KBperSec > 0)
                Command += KBperSec.ToString();
            AddCommandToQueue(Command);
        }

        void AddCommandToQueue(string Command)
        {
            if (!Config.Disabled)
            {
                lock (_CommandQueue)
                {
                    _CommandQueue.Add(Command);
                }
                _CommandTrigger.Set();
            }
        }


        public void RefreshQueueStatus()
        {
            _CommandTrigger.Set();
        }

        private enum CommandType { NormalCommand = 0, QueueCommand, HistoryCommand, Finished }
        private void SABnzbd_Thread()
        {
            const int DelayBeforeGoingSlowPoll = 10; //seconds before going from fast poll into slow poll

            int LastQueuedItemsCount = -1;
            DateTime LastDownloadComplete = DateTime.Now; //makes sure we're getting a history update the first time
            DateTime LastActivity = new DateTime();

            while (!Global.ApplicationIsTerminating)
            {
                SABnzbdStatus NewStatus = new SABnzbdStatus();

                try
                {
                    CommandType cmdType = CommandType.NormalCommand;
                    do
                    {
                        string Command = null;

                        lock (_CommandQueue)
                        {
                            if (cmdType == CommandType.NormalCommand && _CommandQueue.Count > 0)
                            {
                                Command = _CommandQueue[0];
                                _CommandQueue.RemoveAt(0);
                            }
                            else
                            {
                                cmdType++; //go to next command state

                                switch (cmdType)
                                {
                                    case CommandType.QueueCommand:
                                        Command = "mode=queue&start=START&limit=LIMIT&output=xml";
                                        break;
                                    case CommandType.HistoryCommand:
                                        //only fetch history when needed
                                        if (_RefreshHistory || _Status.IsExtracting || LastDownloadComplete.AddSeconds(DelayBeforeGoingSlowPoll) > DateTime.Now)
                                        {
                                            Command = "mode=history&start=START&limit=LIMIT&output=xml";
                                            _RefreshHistory = false;
                                        }
                                        else
                                        {
                                            NewStatus.HistoryItems = _Status.HistoryItems; //copy over history items
                                            NewStatus.HistoryItems.HasChanged = false; //make sure we mark it as not changed
                                        }
                                        break;
                                }
                            }
                        }

                        if (Command != null)
                        {
                            string CommandResult = null;
                            WebRequest http = HttpWebRequest.Create(GetCommandURL(Command));
                            http.Timeout = 5000;

                            using (WebResponse resp = http.GetResponse())
                                using (Stream respStream = resp.GetResponseStream())
                                    using (StreamReader reader = new StreamReader(respStream))
                                        CommandResult = reader.ReadToEnd();

                            switch (cmdType)
                            {
                                case CommandType.QueueCommand:
                                    ProcessQueueStatusXML(NewStatus, CommandResult);
                                    break;
                                case CommandType.HistoryCommand:
                                    ProcessHistoryStatusXML(NewStatus, CommandResult);
                                    break;
                            }
                        }
                    }
                    while (cmdType != CommandType.Finished);

                    Program.MainForm.SetStatus("");
                    _Status = NewStatus; //Finally, update actual status

                    if (LastQueuedItemsCount > NewStatus.QueuedItems.Count)
                        LastDownloadComplete = DateTime.Now;

                    LastQueuedItemsCount = NewStatus.QueuedItems.Count;

                    if (!Global.ApplicationIsTerminating)
                    {
                        if (Program.MainForm.Visible || NewStatus.HistoryItems.HasChanged) //only fire event when window visible
                        {
                            //See if we need to trigger any evets, do this on the main GUI thread
                            if (StatusUpdated != null)
                            {
                                try { Global.InvokeOnGUI(StatusUpdated, _Status); }
                                catch { /*  */ }
                            }
                        }
                    }
                }
                catch
                {
                    Program.MainForm.SetStatus("Error: Unable to connect to SABnzbd server");
                    lock (_CommandQueue)
                    {
                        //when in error, no need to try all of the remaining commands
                        _CommandQueue.Clear();
                    }
                }

                if (!Global.ApplicationIsTerminating)
                {
                    //_CommandTrigger.Reset(); //always reset before waiting
                    if (_Status.IsDownloading || _Status.IsExtracting) //  && !_Status.IsPaused)
                        LastActivity = DateTime.Now;

                    if (LastActivity.AddSeconds(DelayBeforeGoingSlowPoll) >= DateTime.Now) //delay slow-poll by 10 secs
                        _CommandTrigger.WaitOne(Config.ActivePollInterval * 1000);
                    else
                        _CommandTrigger.WaitOne(Config.InactivePollInterval * 1000);
                }
            }
        }

        int TryReadInt(XmlDocument X, string NodePath, int DefaultValue)
        {
            XmlNode Elm = X.SelectSingleNode(NodePath);
            if (Elm != null)
                int.TryParse(Elm.InnerText, out DefaultValue);
            return DefaultValue;
        }

        bool ProcessQueueStatusXML(SABnzbdStatus NewStatus, string XML)
        {
            XmlDocument X = new XmlDocument();

            try
            {
                X.LoadXml(XML);
            }
            catch
            {
                return false; //skip all the rest, no use
            }

            //First, check for errors
            XmlNode Elm = X.SelectSingleNode("/result/status");
            if (Elm != null)
            {
                Elm = X.SelectSingleNode("/result/error");
                if (Elm != null && Elm.InnerText.Length > 0)
                {
                    Program.MainForm.SetStatus("SABnzb error: " + Elm.InnerText);
                    return false;
                }
            }

            NewStatus.QueuedItems.HasChanged = true; //changed by default

            XmlNodeList Cats = X.GetElementsByTagName("category");
            XmlNodeList Items = X.GetElementsByTagName("slot");

            Elm = X.SelectSingleNode("/queue/paused");
            NewStatus.IsPaused = Elm != null ? bool.Parse(Elm.InnerText) : false;
            Elm = X.SelectSingleNode("/queue/mb");
            NewStatus.MB = Elm != null ? float.Parse(Elm.InnerText) : 0;
            Elm = X.SelectSingleNode("/queue/mbleft");
            NewStatus.MBleft = Elm != null ? float.Parse(Elm.InnerText) : 0;
            Elm = X.SelectSingleNode("/queue/timeleft");
            NewStatus.TimeLeft = Elm != null ? Elm.InnerText : string.Empty;
            Elm = X.SelectSingleNode("/queue/sizeleft");
            NewStatus.SizeLeft = Elm != null ? Elm.InnerText : string.Empty;
            Elm = X.SelectSingleNode("/queue/speed");
            NewStatus.Speed = Elm != null ? Elm.InnerText + "B/s" : string.Empty;
            Elm = X.SelectSingleNode("/queue/diskspace1");
            NewStatus.DiskSpace = Elm != null ? Elm.InnerText + " GB" : string.Empty;
            NewStatus.SpeedLimit = TryReadInt(X, "/queue/speedlimit", 0);
            
            foreach (XmlNode CatNode in Cats)
                NewStatus.Categories.Add(CatNode.InnerText);

            foreach (XmlNode ItemNode in Items)
            {
                SABnzbdItem NewItem = new SABnzbdItem();
                foreach (XmlNode ChildNode in ItemNode.ChildNodes)
                {
                    switch (ChildNode.Name.ToLower())
                    {
                        case "status": NewItem.Status = ChildNode.InnerText; break;
                        case "timeleft": NewItem.TimeLeft = ChildNode.InnerText; break;
                        case "avg_age": NewItem.Age = ChildNode.InnerText; break;
                        case "mb": NewItem.MB = StringToFloat(ChildNode.InnerText); break;
                        case "mbleft": NewItem.MBleft = StringToFloat(ChildNode.InnerText); break;
                        case "filename": NewItem.FileName = ChildNode.InnerText; break;
                        case "cat": NewItem.Category = ChildNode.InnerText; break;
                        case "percentage": NewItem.Percentage = int.Parse(ChildNode.InnerText); break;
                        case "nzo_id": NewItem.nzo_id = ChildNode.InnerText; break;
                        case "size": NewItem.Size = ChildNode.InnerText; break;
                        case "sizeleft": NewItem.SizeLeft = ChildNode.InnerText; break;
                        case "priority": NewItem.Priority = ChildNode.InnerText; break;
                    }
                }

                NewItem.Index = NewStatus.QueuedItems.Count;
                NewStatus.QueuedItems.Add(NewItem);
            }

            return true;
        }

        /// <summary>
        /// Apparently, on some systems float.Parse(string) doesn't work correctly, hence this replacement.
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        float StringToFloat(string strFloat)
        {
            float flt = 0;
            bool HadDecimalPoint = false;
            float DecimalPointDevide = 1; //start at 1

            foreach (char Chr in strFloat.Trim())
            {
                if (Chr == '.' || Chr == ',')
                {
                    HadDecimalPoint = true;
                }
                else if (HadDecimalPoint)
                {
                    DecimalPointDevide *= 10;
                    flt += (Chr - '0') / DecimalPointDevide;
                }
                else
                {
                    flt *= 10;
                    flt += (Chr - '0');
                }
            }
            return flt;
        }

        bool ProcessHistoryStatusXML(SABnzbdStatus NewStatus, string XML)
        {
            XmlDocument X = new XmlDocument();

            try
            {
                X.LoadXml(XML);
            }
            catch
            {
                return false; //skip all the rest, no use
            }
            
            XmlNode Elm = X.SelectSingleNode("/history/slots");
            if (Elm == null) //First, check for errors
            {
                Elm = X.SelectSingleNode("/result/error");
                if (Elm != null && Elm.InnerText.Length > 0)
                    Program.MainForm.SetStatus("SABnzb error: " + Elm.InnerText);

                return false;
            }

            NewStatus.HistoryItems.HasChanged = false; // innocent until proven guilty


            XmlNodeList Items = Elm.ChildNodes;
            foreach (XmlNode ItemNode in Items)
            {
                SABnzbdItem NewItem = new SABnzbdItem();

                foreach (XmlNode ChildNode in ItemNode.ChildNodes)
                {
                    switch (ChildNode.Name.ToLower())
                    {
                        case "name": NewItem.FileName = ChildNode.InnerText; break;
                        case "status": NewItem.Status = ChildNode.InnerText; break;
                        case "nzo_id": NewItem.nzo_id = ChildNode.InnerText; break;
                        case "size": NewItem.Size = ChildNode.InnerText; break;
                        case "category": NewItem.Category = ChildNode.InnerText; break;
                        case "storage": NewItem.Directory = ChildNode.InnerText; break;
                        case "fail_message": NewItem.FailMessage = ChildNode.InnerText; break;
                    }
                }

                if ((NewItem.FailMessage != null && NewItem.FailMessage.Length > 0) || NewItem.Status.StartsWith("Failed"))
                {
                    NewStatus.HistoryItems.HasErrors = true;
                    NewItem.BackColor = NzbItem.ErrorBackgroundColor;
                }
                else if (NewItem.Status.StartsWith("Completed"))
                {
                    NewItem.BackColor = NzbItem.DownloadedBackgroundColor;
                }
                else
                {
                    NewStatus.IsExtracting = true;
                    NewItem.BackColor = NzbItem.NewBackgroundColor;
                }

                NewItem.Index = NewStatus.HistoryItems.Count;
                NewStatus.HistoryItems.Add(NewItem);
            }

            //now that we retrieved the history, we should check if its actually different (prevent too many redraws and checks)
            if (NewStatus.HistoryItems.Count == _Status.HistoryItems.Count)
            {
                for (int i = 0; i < NewStatus.HistoryItems.Count; i++)
                {
                    SABnzbdItem OldItem = _Status.HistoryItems[i];
                    SABnzbdItem NewItem = NewStatus.HistoryItems[i];
                    if (OldItem.FileName != NewItem.FileName)
                    {
                        NewStatus.HistoryItems.HasChanged = true;
                        break;
                    }
                    if (OldItem.Status != NewItem.Status)
                    {
                        NewStatus.HistoryItems.HasChanged = true;
                        break;
                    }
                }
            }
            else
                NewStatus.HistoryItems.HasChanged = true;
            
            return true;
        }

        void MainForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _CommandTrigger.Set();
        }

    }



}
