namespace NzbSearcher
{
    partial class AutoDownloader_Control
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstAutoDownloads = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lstAutoDownloads
            // 
            this.lstAutoDownloads.FullRowSelect = true;
            this.lstAutoDownloads.HideSelection = false;
            this.lstAutoDownloads.Location = new System.Drawing.Point(0, 0);
            this.lstAutoDownloads.Name = "lstAutoDownloads";
            this.lstAutoDownloads.Size = new System.Drawing.Size(325, 282);
            this.lstAutoDownloads.TabIndex = 11;
            this.lstAutoDownloads.UseCompatibleStateImageBehavior = false;
            this.lstAutoDownloads.View = System.Windows.Forms.View.Details;
            this.lstAutoDownloads.VirtualMode = true;
            this.lstAutoDownloads.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lstAutoDownloads_RetrieveVirtualItem);
            this.lstAutoDownloads.SelectedIndexChanged += new System.EventHandler(this.lstAutoDownloads_SelectedIndexChanged);
            this.lstAutoDownloads.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstAutoDownloads_KeyDown);
            this.lstAutoDownloads.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstAutoDownloads_MouseDoubleClick);
            // 
            // AutoDownloader_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstAutoDownloads);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AutoDownloader_Control";
            this.Size = new System.Drawing.Size(581, 503);
            this.Load += new System.EventHandler(this.AutoDownloader_Control_Load);
            this.Resize += new System.EventHandler(this.AutoDownloader_Control_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstAutoDownloads;


    }
}
