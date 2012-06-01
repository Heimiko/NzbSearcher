namespace NzbSearcher
{
    partial class frmAutoDownload
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.scrSeason = new System.Windows.Forms.VScrollBar();
            this.txtSeasonNr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEpisodeNr = new System.Windows.Forms.TextBox();
            this.scrEpisode = new System.Windows.Forms.VScrollBar();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMinSize = new System.Windows.Forms.TextBox();
            this.txtMaxSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cboSABcat = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblFriendlyName = new System.Windows.Forms.Label();
            this.txtFriendlyName = new System.Windows.Forms.TextBox();
            this.txtResultFilter = new System.Windows.Forms.TextBox();
            this.lblResultFilter = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblRegExHelp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(111, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(203, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // scrSeason
            // 
            this.scrSeason.LargeChange = 1;
            this.scrSeason.Location = new System.Drawing.Point(153, 65);
            this.scrSeason.Maximum = 99;
            this.scrSeason.Minimum = 1;
            this.scrSeason.Name = "scrSeason";
            this.scrSeason.Size = new System.Drawing.Size(17, 20);
            this.scrSeason.TabIndex = 8;
            this.scrSeason.Value = 1;
            this.scrSeason.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrSeason_Scroll);
            // 
            // txtSeasonNr
            // 
            this.txtSeasonNr.Location = new System.Drawing.Point(111, 65);
            this.txtSeasonNr.Name = "txtSeasonNr";
            this.txtSeasonNr.Size = new System.Drawing.Size(42, 20);
            this.txtSeasonNr.TabIndex = 9;
            this.txtSeasonNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSeasonNr.TextChanged += new System.EventHandler(this.txtSeasonNr_TextChanged);
            this.txtSeasonNr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSeasonNr_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Season / Episode:";
            // 
            // txtEpisodeNr
            // 
            this.txtEpisodeNr.Location = new System.Drawing.Point(178, 65);
            this.txtEpisodeNr.Name = "txtEpisodeNr";
            this.txtEpisodeNr.Size = new System.Drawing.Size(42, 20);
            this.txtEpisodeNr.TabIndex = 12;
            this.txtEpisodeNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtEpisodeNr.TextChanged += new System.EventHandler(this.txtEpisodeNr_TextChanged);
            this.txtEpisodeNr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEpisodeNr_KeyPress);
            // 
            // scrEpisode
            // 
            this.scrEpisode.LargeChange = 1;
            this.scrEpisode.Location = new System.Drawing.Point(219, 65);
            this.scrEpisode.Maximum = 99;
            this.scrEpisode.Minimum = 1;
            this.scrEpisode.Name = "scrEpisode";
            this.scrEpisode.Size = new System.Drawing.Size(17, 20);
            this.scrEpisode.TabIndex = 11;
            this.scrEpisode.Value = 1;
            this.scrEpisode.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrEpisode_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Size Min / Max:";
            // 
            // txtMinSize
            // 
            this.txtMinSize.Location = new System.Drawing.Point(111, 91);
            this.txtMinSize.Name = "txtMinSize";
            this.txtMinSize.Size = new System.Drawing.Size(42, 20);
            this.txtMinSize.TabIndex = 14;
            this.txtMinSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMaxSize
            // 
            this.txtMaxSize.Location = new System.Drawing.Point(178, 91);
            this.txtMaxSize.Name = "txtMaxSize";
            this.txtMaxSize.Size = new System.Drawing.Size(42, 20);
            this.txtMaxSize.TabIndex = 15;
            this.txtMaxSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(151, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "MB";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(218, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "MB";
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(239, 255);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 18;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(158, 255);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 19;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cboSABcat
            // 
            this.cboSABcat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSABcat.FormattingEnabled = true;
            this.cboSABcat.Location = new System.Drawing.Point(111, 38);
            this.cboSABcat.Name = "cboSABcat";
            this.cboSABcat.Size = new System.Drawing.Size(203, 21);
            this.cboSABcat.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Category:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Search Text:";
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Enabled = false;
            this.lblSearch.Location = new System.Drawing.Point(110, 137);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(43, 13);
            this.lblSearch.TabIndex = 4;
            this.lblSearch.Text = "sample:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(111, 117);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(203, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Friendly Name:";
            // 
            // lblFriendlyName
            // 
            this.lblFriendlyName.AutoSize = true;
            this.lblFriendlyName.Enabled = false;
            this.lblFriendlyName.Location = new System.Drawing.Point(110, 176);
            this.lblFriendlyName.Name = "lblFriendlyName";
            this.lblFriendlyName.Size = new System.Drawing.Size(43, 13);
            this.lblFriendlyName.TabIndex = 7;
            this.lblFriendlyName.Text = "sample:";
            // 
            // txtFriendlyName
            // 
            this.txtFriendlyName.Location = new System.Drawing.Point(111, 156);
            this.txtFriendlyName.Name = "txtFriendlyName";
            this.txtFriendlyName.Size = new System.Drawing.Size(203, 20);
            this.txtFriendlyName.TabIndex = 6;
            this.txtFriendlyName.TextChanged += new System.EventHandler(this.txtFriendlyName_TextChanged);
            // 
            // txtResultFilter
            // 
            this.txtResultFilter.Location = new System.Drawing.Point(111, 192);
            this.txtResultFilter.Name = "txtResultFilter";
            this.txtResultFilter.Size = new System.Drawing.Size(203, 20);
            this.txtResultFilter.TabIndex = 23;
            // 
            // lblResultFilter
            // 
            this.lblResultFilter.AutoSize = true;
            this.lblResultFilter.Enabled = false;
            this.lblResultFilter.Location = new System.Drawing.Point(110, 212);
            this.lblResultFilter.Name = "lblResultFilter";
            this.lblResultFilter.Size = new System.Drawing.Size(176, 26);
            this.lblResultFilter.TabIndex = 24;
            this.lblResultFilter.Text = "Warning: regular expression - leave \r\nempty if you don\'t know what that is";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 195);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Result Filter:";
            // 
            // lblRegExHelp
            // 
            this.lblRegExHelp.AutoSize = true;
            this.lblRegExHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRegExHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegExHelp.ForeColor = System.Drawing.Color.Blue;
            this.lblRegExHelp.Location = new System.Drawing.Point(287, 212);
            this.lblRegExHelp.Name = "lblRegExHelp";
            this.lblRegExHelp.Size = new System.Drawing.Size(27, 13);
            this.lblRegExHelp.TabIndex = 25;
            this.lblRegExHelp.Text = "help";
            this.lblRegExHelp.Click += new System.EventHandler(this.lblRegExHelp_Click);
            // 
            // frmAutoDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 290);
            this.Controls.Add(this.lblRegExHelp);
            this.Controls.Add(this.txtResultFilter);
            this.Controls.Add(this.lblResultFilter);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cboSABcat);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtMinSize);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtMaxSize);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtEpisodeNr);
            this.Controls.Add(this.scrEpisode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSeasonNr);
            this.Controls.Add(this.scrSeason);
            this.Controls.Add(this.txtFriendlyName);
            this.Controls.Add(this.lblFriendlyName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAutoDownload";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Download";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.VScrollBar scrSeason;
        private System.Windows.Forms.TextBox txtSeasonNr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEpisodeNr;
        private System.Windows.Forms.VScrollBar scrEpisode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMinSize;
        private System.Windows.Forms.TextBox txtMaxSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ComboBox cboSABcat;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFriendlyName;
        private System.Windows.Forms.TextBox txtFriendlyName;
        private System.Windows.Forms.TextBox txtResultFilter;
        private System.Windows.Forms.Label lblResultFilter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblRegExHelp;
    }
}