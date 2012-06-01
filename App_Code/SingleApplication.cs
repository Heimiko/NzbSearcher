using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.IO;

using System.IO.Pipes;


namespace NzbSearcher
{
	/// <summary>
	/// Summary description for SingleApp.
	/// </summary>
	public class SingleApplication
	{
        const string NamedPipeName = "Heimiko.NzbSearcher.Pipe";

        //static NamedPipeServerStream _pipeStream = null;
        static Thread _PipeThread = null;
        static Mutex _mutex;
        static bool _StopThread = false;

		/// <summary>
		/// check if given exe already running or not
		/// </summary>
		/// <returns>returns true if already running</returns>
		public static bool IfAlreadyRunningDoSwitch()
		{
			string strLoc = Assembly.GetExecutingAssembly().Location;
			FileSystemInfo fileInfo = new FileInfo(strLoc);
			string sExeName = fileInfo.Name;
			bool bCreatedNew = false;

			_mutex = new Mutex(true, "Global\\"+sExeName, out bCreatedNew);
            if (bCreatedNew)
            {
                _mutex.ReleaseMutex();
                _PipeThread = new Thread(new System.Threading.ThreadStart(PipeThread));
                _PipeThread.Name = "Pipe";
                _PipeThread.Start();

                Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            }
            else
            {
                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(NamedPipeName))
                {
                    // The connect function will indefinitely wait for the pipe to become available
                    // If that is not acceptable specify a maximum waiting time (in ms)
                    pipeStream.Connect();

                    using (StreamWriter sw = new StreamWriter(pipeStream))
                    {
                        string[] Args = Environment.GetCommandLineArgs();

                        sw.AutoFlush = true;
                        sw.WriteLine(Args.Length >= 2 ? Args[1] : string.Empty);
                        pipeStream.WaitForPipeDrain();
                    }
                }
            }
            
			return !bCreatedNew;
		}

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            _StopThread = true;

            //to get pipeStream.WaitForConnection() to exit, we have to connect a client
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(NamedPipeName))
                pipeStream.Connect(); //just connect, then simply disconnect again
        }

        private static void PipeThread()
        {
            while (!_StopThread)
            {
                // Create a name pipe
                using (NamedPipeServerStream pipeStream = new NamedPipeServerStream(NamedPipeName))
                {
                    // Wait for a connection
                    pipeStream.WaitForConnection();

                    if (!_StopThread)
                    {
                        using (StreamReader sr = new StreamReader(pipeStream))
                        {
                            // We read a line from the pipe and process it
                            string command = sr.ReadLine();

                            if (command.Length != 0)
                                Program.ProcessCommandLine(command);
                            else
                                Program.MainForm.ShowHideForm(true);
                        }
                    }
                }
            }
        }
	}

}
