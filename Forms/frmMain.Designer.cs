namespace NzbSearcher
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblLimit = new System.Windows.Forms.ToolStripDropDownButton();
            this.txtSpeedLimit = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lblSpeed = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSpace = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.MainTabControl = new System.Windows.Forms.ToolStrip();
            this.MainToolStrip = new System.Windows.Forms.ToolStrip();
            this.cmdWatchList = new System.Windows.Forms.ToolStripButton();
            this.cmdHelp = new System.Windows.Forms.ToolStripButton();
            this.cmdConfig = new System.Windows.Forms.ToolStripButton();
            this.ClientToolStip = new System.Windows.Forms.ToolStrip();
            this.mnuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuTrayShow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ImdbWatchList = new NzbSearcher.ImdbWatchList();
            this.statusStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.MainToolStrip.SuspendLayout();
            this.mnuTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "NzbSearcher";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblLimit,
            this.lblSpeed,
            this.lblSpace,
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 638);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(627, 24);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblLimit
            // 
            this.lblLimit.AutoSize = false;
            this.lblLimit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblLimit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtSpeedLimit,
            this.toolStripMenuItem10,
            this.toolStripMenuItem9,
            this.toolStripMenuItem8,
            this.toolStripMenuItem7,
            this.toolStripMenuItem6,
            this.toolStripMenuItem5,
            this.toolStripMenuItem4,
            this.toolStripMenuItem3,
            this.toolStripMenuItem2,
            this.toolStripMenuItem1});
            this.lblLimit.Image = ((System.Drawing.Image)(resources.GetObject("lblLimit.Image")));
            this.lblLimit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lblLimit.Name = "lblLimit";
            this.lblLimit.Size = new System.Drawing.Size(150, 22);
            this.lblLimit.Text = "Speed Limit:";
            this.lblLimit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLimit.DropDownClosed += new System.EventHandler(this.lblLimit_DropDownClosed);
            this.lblLimit.DropDownOpening += new System.EventHandler(this.lblLimit_DropDownOpening);
            this.lblLimit.DropDownOpened += new System.EventHandler(this.lblLimit_DropDownOpened);
            this.lblLimit.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.lblLimit_DropDownItemClicked);
            // 
            // txtSpeedLimit
            // 
            this.txtSpeedLimit.Name = "txtSpeedLimit";
            this.txtSpeedLimit.Size = new System.Drawing.Size(100, 23);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem10.Tag = "0";
            this.toolStripMenuItem10.Text = "No Limit";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem9.Tag = "4500";
            this.toolStripMenuItem9.Text = "4500 KB/sec";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem8.Tag = "3000";
            this.toolStripMenuItem8.Text = "3000 KB/sec";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem7.Tag = "2000";
            this.toolStripMenuItem7.Text = "2000 KB/sec";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem6.Tag = "1500";
            this.toolStripMenuItem6.Text = "1500 KB/sec";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem5.Tag = "750";
            this.toolStripMenuItem5.Text = "750 KB/sec";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem4.Tag = "400";
            this.toolStripMenuItem4.Text = "400 KB/sec";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem3.Tag = "200";
            this.toolStripMenuItem3.Text = "200 KB/sec";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem2.Tag = "100";
            this.toolStripMenuItem2.Text = "100 KB/sec";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItem1.Tag = "50";
            this.toolStripMenuItem1.Text = "50 KB/sec";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = false;
            this.lblSpeed.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.lblSpeed.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(100, 19);
            this.lblSpeed.Text = "Cur:";
            this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSpace
            // 
            this.lblSpace.AutoSize = false;
            this.lblSpace.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblSpace.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(110, 19);
            this.lblSpace.Text = "Space:";
            this.lblSpace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(52, 19);
            this.lblStatus.Text = "lblStatus";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitter1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.ImdbWatchList);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(627, 559);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(627, 638);
            this.toolStripContainer1.TabIndex = 6;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.MainTabControl);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.MainToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.ClientToolStip);
            // 
            // MainTabControl
            // 
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.None;
            this.MainTabControl.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.MainTabControl.Location = new System.Drawing.Point(3, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.Size = new System.Drawing.Size(111, 25);
            this.MainTabControl.TabIndex = 8;
            this.MainTabControl.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MainTabControl_ItemClicked);
            // 
            // MainToolStrip
            // 
            this.MainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.MainToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdWatchList,
            this.cmdHelp,
            this.cmdConfig});
            this.MainToolStrip.Location = new System.Drawing.Point(114, 0);
            this.MainToolStrip.Name = "MainToolStrip";
            this.MainToolStrip.Size = new System.Drawing.Size(148, 54);
            this.MainToolStrip.TabIndex = 7;
            this.MainToolStrip.Text = "toolStrip1";
            // 
            // cmdWatchList
            // 
            this.cmdWatchList.Image = global::NzbSearcher.Properties.Resources.imdb2;
            this.cmdWatchList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdWatchList.Name = "cmdWatchList";
            this.cmdWatchList.Size = new System.Drawing.Size(45, 51);
            this.cmdWatchList.Text = "Watch";
            this.cmdWatchList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdWatchList.ToolTipText = "Edit Configuration";
            this.cmdWatchList.Click += new System.EventHandler(this.cmdWatchList_Click);
            // 
            // cmdHelp
            // 
            this.cmdHelp.Image = global::NzbSearcher.Properties.Resources.help;
            this.cmdHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdHelp.Name = "cmdHelp";
            this.cmdHelp.Size = new System.Drawing.Size(44, 51);
            this.cmdHelp.Text = "About";
            this.cmdHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdHelp.ToolTipText = "Show about box, version info and link to website (help)";
            this.cmdHelp.Click += new System.EventHandler(this.cmdHelp_Click);
            // 
            // cmdConfig
            // 
            this.cmdConfig.Image = global::NzbSearcher.Properties.Resources.config;
            this.cmdConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdConfig.Name = "cmdConfig";
            this.cmdConfig.Size = new System.Drawing.Size(47, 51);
            this.cmdConfig.Text = "Config";
            this.cmdConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdConfig.ToolTipText = "Edit Configuration";
            this.cmdConfig.Click += new System.EventHandler(this.cmdConfig_Click);
            // 
            // ClientToolStip
            // 
            this.ClientToolStip.Dock = System.Windows.Forms.DockStyle.None;
            this.ClientToolStip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ClientToolStip.Location = new System.Drawing.Point(3, 54);
            this.ClientToolStip.Name = "ClientToolStip";
            this.ClientToolStip.Size = new System.Drawing.Size(111, 25);
            this.ClientToolStip.TabIndex = 9;
            // 
            // mnuTray
            // 
            this.mnuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTrayShow,
            this.mnuTrayExit});
            this.mnuTray.Name = "mnuTray";
            this.mnuTray.Size = new System.Drawing.Size(106, 48);
            // 
            // mnuTrayShow
            // 
            this.mnuTrayShow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.mnuTrayShow.Name = "mnuTrayShow";
            this.mnuTrayShow.Size = new System.Drawing.Size(105, 22);
            this.mnuTrayShow.Text = "Show";
            this.mnuTrayShow.Click += new System.EventHandler(this.mnuTrayShow_Click);
            // 
            // mnuTrayExit
            // 
            this.mnuTrayExit.Name = "mnuTrayExit";
            this.mnuTrayExit.Size = new System.Drawing.Size(105, 22);
            this.mnuTrayExit.Text = "Exit";
            this.mnuTrayExit.Click += new System.EventHandler(this.mnuTrayExit_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(246, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 559);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            this.splitter1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(256, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(371, 559);
            this.panel1.TabIndex = 2;
            // 
            // ImdbWatchList
            // 
            this.ImdbWatchList.Dock = System.Windows.Forms.DockStyle.Left;
            this.ImdbWatchList.Location = new System.Drawing.Point(0, 0);
            this.ImdbWatchList.Name = "ImdbWatchList";
            this.ImdbWatchList.Size = new System.Drawing.Size(246, 559);
            this.ImdbWatchList.TabIndex = 0;
            this.ImdbWatchList.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 662);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NzbSearcher";
            this.Activated += new System.EventHandler(this.frmMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.LocationChanged += new System.EventHandler(this.frmMain_LocationChanged);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.MainToolStrip.ResumeLayout(false);
            this.MainToolStrip.PerformLayout();
            this.mnuTray.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ImageList imgList;
        //private System.Windows.Forms.ToolStripButton toolStripButton2;
        //private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripStatusLabel lblSpeed;
        private System.Windows.Forms.ToolStripStatusLabel lblSpace;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip MainToolStrip;
        private System.Windows.Forms.ToolStripButton cmdConfig;
        private System.Windows.Forms.ToolStrip MainTabControl;
        private System.Windows.Forms.ToolStripButton cmdHelp;
        private System.Windows.Forms.ToolStrip ClientToolStip;
        private System.Windows.Forms.ToolStripDropDownButton lblLimit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripTextBox txtSpeedLimit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ContextMenuStrip mnuTray;
        private System.Windows.Forms.ToolStripMenuItem mnuTrayShow;
        private System.Windows.Forms.ToolStripMenuItem mnuTrayExit;
        private System.Windows.Forms.ToolStripButton cmdWatchList;
        private ImdbWatchList ImdbWatchList;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;

    }
}

