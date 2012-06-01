namespace NzbSearcher
{
    partial class frmAbout
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
            this.lblAbout = new System.Windows.Forms.Label();
            this.lblWebsite = new System.Windows.Forms.Label();
            this.lblNZB = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblAbout
            // 
            this.lblAbout.Location = new System.Drawing.Point(11, 45);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(101, 77);
            this.lblAbout.TabIndex = 0;
            this.lblAbout.Text = "lblAbout";
            this.lblAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAbout.Click += new System.EventHandler(this.frmAbout_Click);
            // 
            // lblWebsite
            // 
            this.lblWebsite.AutoSize = true;
            this.lblWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWebsite.ForeColor = System.Drawing.Color.Blue;
            this.lblWebsite.Location = new System.Drawing.Point(12, 130);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(101, 13);
            this.lblWebsite.TabIndex = 1;
            this.lblWebsite.Text = "Goto Website (help)";
            this.lblWebsite.Click += new System.EventHandler(this.lblWebsite_Click);
            // 
            // lblNZB
            // 
            this.lblNZB.Location = new System.Drawing.Point(14, 9);
            this.lblNZB.Name = "lblNZB";
            this.lblNZB.Size = new System.Drawing.Size(98, 36);
            this.lblNZB.TabIndex = 2;
            this.lblNZB.Text = "lblNZB";
            this.lblNZB.Click += new System.EventHandler(this.frmAbout_Click);
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(124, 157);
            this.Controls.Add(this.lblNZB);
            this.Controls.Add(this.lblWebsite);
            this.Controls.Add(this.lblAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            this.Click += new System.EventHandler(this.frmAbout_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.Label lblWebsite;
        private System.Windows.Forms.Label lblNZB;
    }
}