using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using Heimiko;

namespace NzbSearcher
{
    public class FolderWatcherConfig : PropChangedBase
    {
        public FolderWatcherConfig()
        {
            //default values
            _DeleteNzb = true;
        }

        private string _Path;
        public string Path { get { return _Path; } set { _Path = value; FirePropChanged("Path"); } }

        private bool _DeleteNzb;
        public bool DeleteNzb { get { return _DeleteNzb; } set { _DeleteNzb = value; FirePropChanged("DeleteNzb"); } }
    }

    public class FolderWatcher
    {
        public FolderWatcherConfig Config { get { Init(); return Config<FolderWatcherConfig>.Value; } }

        private FileSystemWatcher _watcher = null;
        private bool _HasInitialized = false;
        private void Init()
        {
            if (_HasInitialized)
                return;

            Config<FolderWatcherConfig>.Value.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Config_PropertyChanged);
            _HasInitialized = true;
        }

        private void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Path":
                    updatePath(Config.Path);
                    break;
                default:
                    break;
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            uploadAndDelete(e.FullPath);
        }

        private void uploadAndDelete(string fullPath)
        {
            try
            {
                Global.SABnzbd.AddNzbByUpload(fullPath, null, null);
                if (Config.DeleteNzb)
                    File.Delete(fullPath);
            }
            catch (System.Exception)
            {
            }
        }

        private void FolderWatcher_Thread()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Filter = "*.nzb";
            _watcher.Created += new FileSystemEventHandler(_watcher_Created);
            _watcher.Renamed += new RenamedEventHandler(_watcher_Created);
            updatePath(Config.Path);
        }

        public void Start()
        {
            Thread NewThread = new Thread(new ThreadStart(FolderWatcher_Thread));
            NewThread.Priority = ThreadPriority.BelowNormal;
            NewThread.Name = "FolderWatcher";
            NewThread.Start();
        }

        private void updatePath(string fullPath)
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;

                try
                {
                    DirectoryInfo dir = new DirectoryInfo(fullPath);
                    foreach (FileInfo f in dir.GetFiles("*.nzb"))
                    {
                        uploadAndDelete(f.FullName);
                    }
                    _watcher.Path = fullPath;
                    _watcher.EnableRaisingEvents = true;
                }
                catch (System.Exception)
                {
                }
            }
        }
    }
}
