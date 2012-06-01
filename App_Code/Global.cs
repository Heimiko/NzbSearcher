using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;

using System.Windows.Forms;
using System.Collections;
using System.Windows.Threading;

using System.Reflection;
using System.Diagnostics;

namespace NzbSearcher
{
    delegate void VoidDelegate();
    delegate void VoidDelegateBool(bool b);
    delegate void VoidDelegateString(string s);

    public interface ICanBeDisabled
    {
        bool Disabled { get; set; }
    }

    public abstract class PropChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void FirePropChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null));
        }

        protected void FirePropChanged(string PropName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
    }

    public interface IGUIelementConfig : ICanBeDisabled
    {
        StringIntDictionary ColumnWidths { get; }

        void SaveColumns(ListView lst);
        void LoadColumns(ListView lst);
    }

    [Serializable]
    public class GeneralConfig
    {
        public GeneralConfig()
        {
            //default values
            ShowButtonText = true;
            ItemsPerPage = 25;  //default value
        }

        public Rectangle FormBounds { get; set; }
        public FormWindowState WindowState { get; set; }
        public bool ShowInTray { get; set; }
        public bool ShowButtonText { get; set; }
        public bool PortableConfig { get; set; }

        //Global Search Provider config items
        public bool SearchInToolbar { get; set; }
        public int ItemsPerPage { get; set; }

        //Toolbar positions
        public Point ToolbarPos_Tabs { get; set; }
        public Point ToolbarPos_Main { get; set; }
        public Point ToolbarPos_Client { get; set; }

        public int FriendlyNameProcessing { get; set; }
        public string LastSABcat { get; set; }

        public NzbHandling NzbHandling { get; set; }
        public string NzbSaveToDir { get; set; }
        public string NzbOpenWith { get; set; }
    }

    public class GUIelementConfig : PropChangedBase, IGUIelementConfig
    {
        public GUIelementConfig()
        {
            ColumnWidths = new StringIntDictionary(); //default
        }

        public bool Disabled { get; set; }
        public StringIntDictionary ColumnWidths { get; set; }

        public void SaveColumns(ListView lst)
        {
            try
            {
                foreach (ColumnHeader col in lst.Columns)
                    ColumnWidths[col.Name] = col.Width;
            }
            catch { }
        }

        public void LoadColumns(ListView lst)
        {
            try
            {
                foreach (ColumnHeader col in lst.Columns)
                    if (ColumnWidths.ContainsKey(col.Name))
                        col.Width = ColumnWidths[col.Name];
            }
            catch { }
        }
    }

    public static class Global
    {
        static SABnzbd _SABnzbd = null;
        public static SABnzbd SABnzbd { get { return _SABnzbd ?? (_SABnzbd = new SABnzbd()); } }

        public static GeneralConfig Config { get { return Config<GeneralConfig>.Value; } }

        static AutoEpisodeDownloader _AutoEpisodeDownloader = null;
        public static AutoEpisodeDownloader AutoEpisodeDownloader { get { return _AutoEpisodeDownloader ?? (_AutoEpisodeDownloader = new AutoEpisodeDownloader()); } }

        static AutoMovieDownloader _AutoMovieDownloader = null;
        public static AutoMovieDownloader AutoMovieDownloader { get { return _AutoMovieDownloader ?? (_AutoMovieDownloader = new AutoMovieDownloader()); } }

        static IMDB _IMDB = null;
        public static IMDB IMDB { get { return _IMDB ?? (_IMDB = new IMDB()); } }

        static IMDB_WatchList _ImdbWatchList = null;
        public static IMDB_WatchList ImdbWatchList { get { return _ImdbWatchList ?? (_ImdbWatchList = new IMDB_WatchList()); } }

        static FolderWatcher _FolderWatcher = null;
        public static FolderWatcher FolderWatcher { get { return _FolderWatcher ?? (_FolderWatcher = new FolderWatcher()); } }

        public static bool ApplicationIsTerminating { get; set; }

        public static string GetStorageDirectory() 
        { 
            return GetStorageDirectory(Global.Config.PortableConfig); 
        }

        public static string GetStorageDirectory(bool Portable)
        {
            if (Portable)
                return Path.GetDirectoryName(Application.ExecutablePath);
            else
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NzbSearcher";
        }

        public static int TryParseInt(string s, int Default)
        {
            try { return int.Parse(s); }
            catch { return Default; }
        }

        public static string GetAgeFrom(DateTime TimeStamp)
        {
            TimeSpan diff = DateTime.Now - TimeStamp;
            if (diff.TotalMinutes < 60)
                return diff.Minutes + " min";
            if (diff.TotalHours < 24)
                return string.Format("{0:0.0} hrs", diff.TotalHours);

            return string.Format("{0:0.0} days", diff.TotalDays);
        }

        public static string GetSizeFromBytes(double Bytes)
        {
            double Size = Bytes / 1024.0;
            if (Size < 1024)
                return string.Format("{0:0.00} KB", Size);
            
            Size /= 1024.0;
            if (Size < 1024)
                return string.Format("{0:0.00} MB", Size);
            
            Size /= 1024.0;
            return string.Format("{0:0.00} GB", Size);
        }

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate(object f) 
                    {
                        ((DispatcherFrame)f).Continue = false;
                        System.Threading.Thread.Sleep(10); // wait a little bit here, otherwise GUI thread gets real slow and eats proc power
                        return null;
                    }
                    ), frame);
            Dispatcher.PushFrame(frame);
        }

        public static void InvokeOnGUI(Delegate method, params object[] args)
        {
            if (Program.MainForm.InvokeRequired)
            {
                Exception ThrowException = null;

                Program.MainForm.Invoke((VoidDelegate)delegate()
                {
                    try
                    {
                        method.DynamicInvoke(args);
                    }
                    catch(Exception exc)
                    {
                        ThrowException = exc;
                    }
                });

                if (ThrowException != null)
                    throw ThrowException;
            }
            else
            {
                method.DynamicInvoke(args);
            }
        }

        public static string RemoveNonAlfaNumeric(string Str)
        {
            string FriendlyName = string.Empty;

            //filter all non alfa-numeric chars
            for (int i = 0; i < Str.Length; i++)
            {
                char Chr = Str[i];
                if (Chr >= 'a' && Chr <= 'z')
                    FriendlyName += Chr;
                else if (Chr >= 'A' && Chr <= 'Z')
                    FriendlyName += Chr;
                else if (Chr >= '0' && Chr <= '9')
                    FriendlyName += Chr;
                else if (FriendlyName.Length > 0 && FriendlyName[FriendlyName.Length - 1] != ' ')
                    FriendlyName += ' '; //add space instead of odd character
            }

            return FriendlyName;
        }

        /// <summary>
        /// removes later word occurances in the string, example:   "Fairy Tail s01e03 - bla fairy tail 03 bla" -> "Fairy Tail s01e03 - bla"
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string RemoveMultiOccurrances(string Str)
        {
            string result = string.Empty;
            foreach (string SingleWord in Str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (result.ToUpper().IndexOf(SingleWord.ToUpper()) < 0)
                    result += (result.Length > 0) ? " " + SingleWord : SingleWord;
            }

            return result;        
        }


        public static void CopyStream(Stream input, Stream output)
        {
            const int blockSize = 1024; // 1Kb
            byte[] buffer = new byte[blockSize];

            // copy data block by block
            int bytesRead = input.Read(buffer, 0, blockSize);
            while (bytesRead > 0)
            {
                output.Write(buffer, 0, bytesRead);
                bytesRead = input.Read(buffer, 0, blockSize);
            }
        }

        static public NzbHandling HandleDownloadedNZB(string TempFileName, string FriendlyName, string SABcat)
        {
            switch (Global.Config.NzbHandling)
            {
                case NzbHandling.SaveToDir:
                    {
                        string NewFileName = Path.Combine(Global.Config.NzbSaveToDir, FriendlyName + ".nzb");
                        Directory.CreateDirectory(Global.Config.NzbSaveToDir);
                        if (File.Exists(NewFileName))
                            File.Delete(NewFileName);
                        File.Move(TempFileName, NewFileName);
                        break;
                    }
                case NzbHandling.OpenWith:
                    {
                        string NewFileName = Path.Combine(Path.GetDirectoryName(TempFileName), FriendlyName + ".nzb");
                        if (File.Exists(NewFileName))
                            File.Delete(NewFileName);
                        File.Move(TempFileName, NewFileName);
                        System.Diagnostics.Process.Start(Global.Config.NzbOpenWith, "\"" + NewFileName + "\"");
                        break;
                    }
                default: //NzbHandling.UploadNZB:
                    {
                        Global.SABnzbd.AddNzbByUpload(TempFileName, FriendlyName, SABcat);
                        return NzbHandling.UploadNZB;
                    }
            }
            return Global.Config.NzbHandling;
        }

        static public string UploadFormFile(string url, string key, string path)
        {
            using (System.Net.WebClient request = new System.Net.WebClient())
            {
                StringBuilder sb = new StringBuilder();

                string boundary = string.Format("{0}", DateTime.Now.Ticks.ToString("x", System.Globalization.CultureInfo.InvariantCulture));
                string b = string.Format("--------{0}", boundary);

                sb.AppendFormat("--{0}", b);
                sb.Append("\r\n");
                sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", key, Path.GetFileName(path));
                sb.Append("\r\n");
                sb.Append("Content-Type: application/octet-stream");
                sb.Append("\r\n");
                sb.Append("\r\n");

                byte[] bufferHeader = Encoding.ASCII.GetBytes(sb.ToString());
                byte[] bufferFooter = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}--\r\n", b)
                );

                using (Stream rstream = new MemoryStream())
                {
                    int nbTry = 0;
                    while (nbTry < 40)
                    {
                        try
                        {
                            using (Stream fstream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                rstream.Write(bufferHeader, 0, bufferHeader.Length);
                                Global.CopyStream(fstream, rstream);
                                rstream.Write(bufferFooter, 0, bufferFooter.Length);
                                break;
                            }
                        }
                        catch (IOException) {}
                        System.Threading.Thread.Sleep(500);
                        nbTry++;
                    }

                    rstream.Seek(0, SeekOrigin.Begin);
                    byte[] ByteBuffer = new byte[rstream.Length];
                    int ReadBytes = rstream.Read(ByteBuffer, 0, (int)rstream.Length);

                    request.Headers.Add(string.Format("Content-Type: multipart/form-data; boundary={0}", b));
                    return Encoding.ASCII.GetString(request.UploadData(new Uri(url), "POST", ByteBuffer));
                }
            }
        }
    }

    public class CastableList<T, Tsub> : List<T>, IList<Tsub> where T : Tsub
    {
        // IList<Tsub>

        Tsub IList<Tsub>.this[int index] { get { return this[index]; } set { this[index] = (T)value; } }
        
        int IList<Tsub>.IndexOf(Tsub item) { return this.IndexOf((T)item); }
        void IList<Tsub>.Insert(int index, Tsub item) { this.Insert(index, (T)item); }
        void IList<Tsub>.RemoveAt(int index) { this.RemoveAt(index); }

        // ICollection<T> 

        void ICollection<Tsub>.Add(Tsub item) { this.Add((T)item); }
        bool ICollection<Tsub>.Contains(Tsub item) { return this.Contains((T)item); }
        void ICollection<Tsub>.CopyTo(Tsub[] array, int arrayIndex) { throw new NotImplementedException(); }
        bool ICollection<Tsub>.Remove(Tsub item) { return this.Remove((T)item); }
        bool ICollection<Tsub>.IsReadOnly { get { return false; } }

        // IEnumerable<T>

        IEnumerator<Tsub> IEnumerable<Tsub>.GetEnumerator() { return new CastedEnumerator(this); }

        public class CastedEnumerator : IEnumerator<Tsub>
        {
            List<T>.Enumerator _enumerator;

            public CastedEnumerator(List<T> lst)
            {
                _enumerator = lst.GetEnumerator();
            }

            public Tsub Current { get { return _enumerator.Current; } }
            
            object IEnumerator.Current { get { return _enumerator.Current; } }
            public bool MoveNext() { return _enumerator.MoveNext(); }
            public void Reset() { ((IEnumerator)_enumerator).Reset(); }

            public void Dispose() { _enumerator.Dispose(); }
        }
    }

}
