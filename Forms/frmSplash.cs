using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace NzbSearcher
{
    public partial class frmSplash : Form
    {
        Timer CloseTimer = new Timer();

        public frmSplash()
        {
            InitializeComponent();

            CloseTimer.Interval = 3000;
            CloseTimer.Tick += new EventHandler(CloseTimer_Tick);
            CloseTimer.Start();
        }

        void CloseTimer_Tick(object sender, EventArgs e)
        {
            CloseTimer.Stop();
            this.Close();
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {

        }

        public void StartCloseTimer()
        {
            CloseTimer.Start();
        }

        public void StopCloseTimer()
        {
            CloseTimer.Stop();
        }

        private void frmSplash_MouseClick(object sender, MouseEventArgs e)
        {
            CloseTimer.Stop();
            this.Close();
        }
    }
}
