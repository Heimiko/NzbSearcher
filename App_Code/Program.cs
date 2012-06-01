using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Web;

using System.Diagnostics;
using System.IO;

namespace NzbSearcher
{
    static class Program
    {
        public static frmMain MainForm { get; private set; }
        public static frmSplash SplashForm { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (SingleApplication.IfAlreadyRunningDoSwitch())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
#else
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            SplashForm = new frmSplash();
            SplashForm.Show();
#endif

            //the one time loading of the config
            Config.Load();

            if (!Global.ApplicationIsTerminating)
            {
                MainForm = new frmMain();
                MainForm.WindowState = Global.Config.WindowState;
                MainForm.Init();

#if DEBUG
                Application.Run(MainForm);
#else
                try
                {
                    Application.Run(MainForm); 
                }
                catch (Exception e) 
                {
                    Application_ThreadException(null, new System.Threading.ThreadExceptionEventArgs(e));
                }
#endif
            }
        }

        static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating && e.ExceptionObject is Exception)
                DoCrashReport(e.ExceptionObject as Exception);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DoCrashReport(e.Exception);
        }

        static object _CrashLockObject = new object();
        public static void DoCrashReport(Exception exc)
        {
            lock (_CrashLockObject)
            {
                try
                {
                    SaveCrashReport(exc);

                    if (SplashForm != null)
                        SplashForm.StopCloseTimer();

                    Global.ApplicationIsTerminating = true;
                    DialogResult res = MessageBox.Show("NzbSearcher stopped working.\r\nDo you want to send a crash report to the developer?", "NzbSearcher - CRASH", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (res == DialogResult.Yes)
                        ReportException(exc);
                }
                catch
                {
                    /* couldn't ask user ??*/
                }

                //terminate application    
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        static string GetCrashReportString(Exception exc)
        {
            try
            {
                string CrashReport = string.Empty;
                
                while (exc != null)
                {
                    string CurrentReport =  "\r\n\r\n" +
                                            "========================================================================" +
                                            "\r\nNzbSearcher " + Application.ProductVersion +
                                            "\r\n\r\n";

                    CurrentReport += exc.Message + "\r\n";
                    CurrentReport += exc.StackTrace.ToString(); //stack trace of inner most exception

                    CrashReport = CurrentReport + CrashReport;
                    exc = exc.InnerException;
                }
                
                return CrashReport;
            }
            catch (Exception e) { return GetCrashReportString(e); }
        }

        static void SaveCrashReport(Exception exc)
        {
            try
            {
                using (FileStream CrashReportFile = File.Create(Global.GetStorageDirectory() + "\\LastCrashReport.log"))
                using (StreamWriter writer = new StreamWriter(CrashReportFile))
                    writer.Write(GetCrashReportString(exc));
            }
            catch { /* saving file failed */ }
        }

        static void ReportException(Exception exc)
        {
            try
            {
                string CrashReport = "Dear Heimiko,\r\n\r\n" +
                             "I'm very annoyed because NzbSearcher crashed!!\r\n" +
                             "Find the details below, have fun fixing it, and make it pronto! (please)";

                CrashReport += GetCrashReportString(exc);

                try //try sending a direct message
                {
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                    mail.From = new System.Net.Mail.MailAddress("NzbSearcherCrashReport@heimiko.com");
                    mail.ReplyTo = mail.From;
                    mail.Body = CrashReport;
                    mail.Subject = "NzbSearcher Crash Report";
                    mail.To.Add("NzbSearcher@heimiko.com");

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("NzbSearcherCrashReport@heimiko.com", "kill:me:now");
                    smtp.Send(mail);

                    MessageBox.Show("The developer has been notified, thank you for your support!", "NzbSearcher Crash Report");
                }
                catch
                {
                    //unable to send directly: try use local email client
                    string MailTo = "mailto:NzbSearcher@heimiko.com";
                    MailTo += "?SUBJECT=NzbSearcher Crash Report";
                    MailTo += "&BODY=" + HttpUtility.UrlEncode(CrashReport).Replace('+', ' ');
                    Process.Start(MailTo);
                }
            }
            catch { /* do nothing */ }
        }

        public static void ProcessCommandLine(string CommandLine)
        {
            if (Program.MainForm.InvokeRequired)
            {
                Global.InvokeOnGUI((VoidDelegateString)ProcessCommandLine, CommandLine);
                return;
            }

            CommandLine = CommandLine.Trim(' ', '\"');

            if (CommandLine.ToLower().EndsWith(".nzb") && System.IO.File.Exists(CommandLine))
            {
                frmAddNZB NzbForm = new frmAddNZB(CommandLine);
                NzbForm.StartPosition = FormStartPosition.CenterScreen;
                NzbForm.TopMost = true;
                NzbForm.Show();
            }
        }
    }
}
