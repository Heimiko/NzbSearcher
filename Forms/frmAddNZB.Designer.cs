namespace NzbSearcher
{
    partial class frmAddNZB
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
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNZB = new System.Windows.Forms.TextBox();
            this.txtFriendlyName = new System.Windows.Forms.TextBox();
            this.cboCategories = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboFriendly = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDownloading = new System.Windows.Forms.Label();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(281, 104);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(2);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(56, 20);
            this.cmdCancel.TabIndex = 0;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(340, 104);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(2);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(56, 20);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Download NZB:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Friendly Name:";
            // 
            // txtNZB
            // 
            this.txtNZB.Enabled = false;
            this.txtNZB.Location = new System.Drawing.Point(9, 27);
            this.txtNZB.Margin = new System.Windows.Forms.Padding(2);
            this.txtNZB.Name = "txtNZB";
            this.txtNZB.Size = new System.Drawing.Size(265, 20);
            this.txtNZB.TabIndex = 5;
            // 
            // txtFriendlyName
            // 
            this.txtFriendlyName.Location = new System.Drawing.Point(9, 67);
            this.txtFriendlyName.Margin = new System.Windows.Forms.Padding(2);
            this.txtFriendlyName.Name = "txtFriendlyName";
            this.txtFriendlyName.Size = new System.Drawing.Size(265, 20);
            this.txtFriendlyName.TabIndex = 6;
            // 
            // cboCategories
            // 
            this.cboCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategories.FormattingEnabled = true;
            this.cboCategories.Location = new System.Drawing.Point(278, 67);
            this.cboCategories.Margin = new System.Windows.Forms.Padding(2);
            this.cboCategories.Name = "cboCategories";
            this.cboCategories.Size = new System.Drawing.Size(118, 21);
            this.cboCategories.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(275, 50);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Category:";
            // 
            // cboFriendly
            // 
            this.cboFriendly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFriendly.FormattingEnabled = true;
            this.cboFriendly.Items.AddRange(new object[] {
            "None",
            "Replace Chars",
            "Search for first occurance",
            "Search for last occurance"});
            this.cboFriendly.Location = new System.Drawing.Point(278, 27);
            this.cboFriendly.Margin = new System.Windows.Forms.Padding(2);
            this.cboFriendly.Name = "cboFriendly";
            this.cboFriendly.Size = new System.Drawing.Size(118, 21);
            this.cboFriendly.TabIndex = 9;
            this.cboFriendly.SelectedIndexChanged += new System.EventHandler(this.cboFriendly_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(278, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Friendly Processing:";
            // 
            // lblDownloading
            // 
            this.lblDownloading.AutoSize = true;
            this.lblDownloading.ForeColor = System.Drawing.Color.Red;
            this.lblDownloading.Location = new System.Drawing.Point(7, 104);
            this.lblDownloading.Name = "lblDownloading";
            this.lblDownloading.Size = new System.Drawing.Size(79, 13);
            this.lblDownloading.TabIndex = 11;
            this.lblDownloading.Text = "lblDownloading";
            this.lblDownloading.Visible = false;
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // frmAddNZB
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(404, 135);
            this.Controls.Add(this.lblDownloading);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboFriendly);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboCategories);
            this.Controls.Add(this.txtFriendlyName);
            this.Controls.Add(this.txtNZB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddNZB";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add NZB to SABnzbd\'s Queue";
            this.Activated += new System.EventHandler(this.frmAddNZB_Activated);
            this.Load += new System.EventHandler(this.frmAddNZB_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNZB;
        private System.Windows.Forms.TextBox txtFriendlyName;
        private System.Windows.Forms.ComboBox cboCategories;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboFriendly;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblDownloading;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
    }
}