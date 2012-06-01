namespace NzbSearcher
{
    partial class SABnzbd_Control
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
            this.DownloadList = new System.Windows.Forms.ListView();
            this.ListSplitter = new System.Windows.Forms.Splitter();
            this.HistoryList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // DownloadList
            // 
            this.DownloadList.Dock = System.Windows.Forms.DockStyle.Top;
            this.DownloadList.FullRowSelect = true;
            this.DownloadList.HideSelection = false;
            this.DownloadList.Location = new System.Drawing.Point(0, 0);
            this.DownloadList.Name = "DownloadList";
            this.DownloadList.Size = new System.Drawing.Size(581, 162);
            this.DownloadList.TabIndex = 4;
            this.DownloadList.UseCompatibleStateImageBehavior = false;
            this.DownloadList.View = System.Windows.Forms.View.Details;
            this.DownloadList.VirtualMode = true;
            this.DownloadList.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.DownloadList_ColumnWidthChanged);
            this.DownloadList.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.DownloadList_RetrieveVirtualItem);
            this.DownloadList.SelectedIndexChanged += new System.EventHandler(this.DownloadList_SelectedIndexChanged);
            this.DownloadList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DownloadList_KeyDown);
            // 
            // ListSplitter
            // 
            this.ListSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.ListSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.ListSplitter.Location = new System.Drawing.Point(0, 162);
            this.ListSplitter.Name = "ListSplitter";
            this.ListSplitter.Size = new System.Drawing.Size(581, 10);
            this.ListSplitter.TabIndex = 6;
            this.ListSplitter.TabStop = false;
            this.ListSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.ListSplitter_SplitterMoved);
            // 
            // HistoryList
            // 
            this.HistoryList.FullRowSelect = true;
            this.HistoryList.HideSelection = false;
            this.HistoryList.Location = new System.Drawing.Point(63, 213);
            this.HistoryList.Name = "HistoryList";
            this.HistoryList.Size = new System.Drawing.Size(470, 219);
            this.HistoryList.TabIndex = 7;
            this.HistoryList.UseCompatibleStateImageBehavior = false;
            this.HistoryList.View = System.Windows.Forms.View.Details;
            this.HistoryList.VirtualMode = true;
            this.HistoryList.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.HistoryList_ColumnWidthChanged);
            this.HistoryList.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.HistoryList_RetrieveVirtualItem);
            this.HistoryList.SelectedIndexChanged += new System.EventHandler(this.HistoryList_SelectedIndexChanged);
            this.HistoryList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HistoryList_KeyDown);
            this.HistoryList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HistoryList_MouseDoubleClick);
            // 
            // SABnzbd_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HistoryList);
            this.Controls.Add(this.ListSplitter);
            this.Controls.Add(this.DownloadList);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SABnzbd_Control";
            this.Size = new System.Drawing.Size(581, 503);
            this.Load += new System.EventHandler(this.SABnzbd_Control_Load);
            this.Resize += new System.EventHandler(this.SABnzbd_Control_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView DownloadList;
        private System.Windows.Forms.Splitter ListSplitter;
        private System.Windows.Forms.ListView HistoryList;

    }
}
