using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NzbSearcher
{
    public partial class frmMessage : Form, IDisposable
    {
        static frmMessage ShowingMessage = null;
        static List<frmMessage> ShowingMessages = new List<frmMessage>();

        string OriginalText { get; set; }

        private frmMessage()
        {
            InitializeComponent();
        }

        public static frmMessage Show(string Message)
        {
            frmMessage NewMessage = new frmMessage();
            NewMessage.txtMessage.Text = Message;
            NewMessage.OriginalText = Message;

            ShowingMessages.Add(NewMessage);

            if (ShowingMessage == null)
            {
                ShowingMessage = NewMessage;
                ShowingMessage.Show();
                Program.MainForm.Enabled = false;
                Global.DoEvents(); //be sure we're completely drawn
                Win32.LockWindowUpdate(Program.MainForm.Handle);
            }
            else
            {
                ShowingMessage.txtMessage.Text = Message;
                Global.DoEvents(); //be sure we're completely drawn
            }
            
            return NewMessage;
        }

        new public void Dispose()
        {
            ShowingMessages.Remove(this);

            if (ShowingMessage == this)
            {
                ShowingMessage = null;
                if (ShowingMessages.Count > 0)
                    ShowingMessage = ShowingMessages[0];
            }

            if (ShowingMessage != null)
            {
                ShowingMessage.txtMessage.Text = ShowingMessages[ShowingMessages.Count - 1].OriginalText;
                if (!ShowingMessage.Visible) //not dsplaying yet? we should do so now
                    ShowingMessage.Show();
            }
            else
            {
                Program.MainForm.Enabled = true; //we're done displaying
                Win32.LockWindowUpdate(IntPtr.Zero);
            }
            
            base.Dispose();
            Global.DoEvents(); //be sure we're completely un-drawn
        }
    }
}
