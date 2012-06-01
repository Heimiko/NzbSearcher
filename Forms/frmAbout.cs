using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NzbSearcher
{
    public partial class frmAbout : Form
    {
        Timer MovementTimer = new Timer();

        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            string CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            lblAbout.Text = "NzbSearcher\r\n v" + CurrentVersion;
            lblAbout.Text += "\r\n\r\nCreated by:\r\n Heimiko";
            lblAbout.BackColor = Color.Transparent;

            lblNZB.Image = Properties.Resources.nzb.ToBitmap();
            lblNZB.Text = string.Empty;
            lblNZB.BackColor = Color.Transparent;

            lblWebsite.BackColor = Color.Transparent;

            MovementTimer.Interval = 200;
            MovementTimer.Tick += new EventHandler(MovementTimer_Tick);
            //MovementTimer.Start();
        }

        void MovementTimer_Tick(object sender, EventArgs e)
        {
            MovementTimer.Stop();
            double ticks = DateTime.Now.Ticks / 10000000.0;
            double sin = Math.Sin(ticks);
            //double cos = Math.Cos(ticks) + 1;

            lblNZB.Left = (int) (sin * 5) + (this.Width - lblNZB.Width) / 2;
            //lblNZB.Top = (int)(cos * (this.Height - lblNZB.Height) / 2);

            //this.Refresh();
            MovementTimer.Start();
        }

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://forums.sabnzbd.org/index.php?topic=5116");
            this.Close();
        }

        private void frmAbout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
