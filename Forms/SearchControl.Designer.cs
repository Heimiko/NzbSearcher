namespace NzbSearcher
{
    partial class SearchControl
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
            this.components = new System.ComponentModel.Container();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.cboMaxDays = new System.Windows.Forms.ComboBox();
            this.txtMinSize = new System.Windows.Forms.TextBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.AdditionalControlsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMinSize = new System.Windows.Forms.Label();
            this.lblMaxDays = new System.Windows.Forms.Label();
            this.cboSearch = new System.Windows.Forms.ComboBox();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NzbResultList = new System.Windows.Forms.ListView();
            this.mnuFavorites = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmdAddFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditFav = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListSplitter = new System.Windows.Forms.Splitter();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.mnuNzbContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmdAddToQueue = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdIMDB = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdShowNFO = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIMDBmovies = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmdSearchIMDB = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdManageMyMovies = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdRefreshMyMovies = new System.Windows.Forms.ToolStripMenuItem();
            this.txtIMDBfilter = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditMov = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchPanel.SuspendLayout();
            this.mnuFavorites.SuspendLayout();
            this.mnuEditFav.SuspendLayout();
            this.mnuNzbContext.SuspendLayout();
            this.mnuIMDBmovies.SuspendLayout();
            this.mnuEditMov.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchPanel
            // 
            this.SearchPanel.Controls.Add(this.cboMaxDays);
            this.SearchPanel.Controls.Add(this.txtMinSize);
            this.SearchPanel.Controls.Add(this.txtFilter);
            this.SearchPanel.Controls.Add(this.label5);
            this.SearchPanel.Controls.Add(this.AdditionalControlsPanel);
            this.SearchPanel.Controls.Add(this.lblMinSize);
            this.SearchPanel.Controls.Add(this.lblMaxDays);
            this.SearchPanel.Controls.Add(this.cboSearch);
            this.SearchPanel.Controls.Add(this.cmdSearch);
            this.SearchPanel.Controls.Add(this.label1);
            this.SearchPanel.Location = new System.Drawing.Point(62, 173);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(562, 51);
            this.SearchPanel.TabIndex = 5;
            // 
            // cboMaxDays
            // 
            this.cboMaxDays.FormattingEnabled = true;
            this.cboMaxDays.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "5",
            "7",
            "10",
            "15",
            "20",
            "30",
            "50",
            "80",
            "100",
            "200",
            "300",
            "500",
            "800"});
            this.cboMaxDays.Location = new System.Drawing.Point(301, 3);
            this.cboMaxDays.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.cboMaxDays.Name = "cboMaxDays";
            this.cboMaxDays.Size = new System.Drawing.Size(42, 21);
            this.cboMaxDays.TabIndex = 8;
            this.cboMaxDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboMaxDays_KeyPress);
            // 
            // txtMinSize
            // 
            this.txtMinSize.Location = new System.Drawing.Point(418, 3);
            this.txtMinSize.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.txtMinSize.Name = "txtMinSize";
            this.txtMinSize.Size = new System.Drawing.Size(42, 20);
            this.txtMinSize.TabIndex = 11;
            this.txtMinSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinSize_KeyPress);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(66, 28);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(157, 20);
            this.txtFilter.TabIndex = 20;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Result Filter:";
            // 
            // AdditionalControlsPanel
            // 
            this.AdditionalControlsPanel.AutoSize = true;
            this.AdditionalControlsPanel.Location = new System.Drawing.Point(470, 27);
            this.AdditionalControlsPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.AdditionalControlsPanel.Name = "AdditionalControlsPanel";
            this.AdditionalControlsPanel.Size = new System.Drawing.Size(88, 22);
            this.AdditionalControlsPanel.TabIndex = 16;
            this.AdditionalControlsPanel.WrapContents = false;
            // 
            // lblMinSize
            // 
            this.lblMinSize.AutoSize = true;
            this.lblMinSize.Location = new System.Drawing.Point(345, 6);
            this.lblMinSize.Name = "lblMinSize";
            this.lblMinSize.Size = new System.Drawing.Size(75, 13);
            this.lblMinSize.TabIndex = 10;
            this.lblMinSize.Text = "Min.Size (MB):";
            // 
            // lblMaxDays
            // 
            this.lblMaxDays.AutoSize = true;
            this.lblMaxDays.Location = new System.Drawing.Point(246, 6);
            this.lblMaxDays.Name = "lblMaxDays";
            this.lblMaxDays.Size = new System.Drawing.Size(57, 13);
            this.lblMaxDays.TabIndex = 9;
            this.lblMaxDays.Text = "Max.Days:";
            // 
            // cboSearch
            // 
            this.cboSearch.FormattingEnabled = true;
            this.cboSearch.Location = new System.Drawing.Point(66, 3);
            this.cboSearch.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.cboSearch.Name = "cboSearch";
            this.cboSearch.Size = new System.Drawing.Size(157, 21);
            this.cboSearch.TabIndex = 7;
            this.cboSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboSearch_KeyPress);
            // 
            // cmdSearch
            // 
            this.cmdSearch.Location = new System.Drawing.Point(470, 1);
            this.cmdSearch.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(88, 23);
            this.cmdSearch.TabIndex = 6;
            this.cmdSearch.Text = "Search";
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Search:";
            // 
            // NzbResultList
            // 
            this.NzbResultList.Dock = System.Windows.Forms.DockStyle.Top;
            this.NzbResultList.FullRowSelect = true;
            this.NzbResultList.HideSelection = false;
            this.NzbResultList.Location = new System.Drawing.Point(0, 0);
            this.NzbResultList.Name = "NzbResultList";
            this.NzbResultList.Size = new System.Drawing.Size(890, 86);
            this.NzbResultList.TabIndex = 3;
            this.NzbResultList.UseCompatibleStateImageBehavior = false;
            this.NzbResultList.View = System.Windows.Forms.View.Details;
            this.NzbResultList.VirtualMode = true;
            this.NzbResultList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.NzbResultList_ColumnClick);
            this.NzbResultList.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.NzbResultList_RetrieveVirtualItem);
            this.NzbResultList.SelectedIndexChanged += new System.EventHandler(this.NzbResultList_SelectedIndexChanged);
            this.NzbResultList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NzbResultList_KeyPress);
            this.NzbResultList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NzbResultList_MouseDoubleClick);
            this.NzbResultList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NzbResultList_MouseUp);
            // 
            // mnuFavorites
            // 
            this.mnuFavorites.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdAddFavorite,
            this.toolStripSeparator1});
            this.mnuFavorites.Name = "mnuFavorites";
            this.mnuFavorites.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mnuFavorites.Size = new System.Drawing.Size(240, 32);
            this.mnuFavorites.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.mnuFavorites_Closing);
            this.mnuFavorites.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuFavorites_ItemClicked);
            // 
            // cmdAddFavorite
            // 
            this.cmdAddFavorite.Image = global::NzbSearcher.Properties.Resources.favorites;
            this.cmdAddFavorite.Name = "cmdAddFavorite";
            this.cmdAddFavorite.Size = new System.Drawing.Size(239, 22);
            this.cmdAddFavorite.Text = "Add current Search to Favorites";
            this.cmdAddFavorite.Click += new System.EventHandler(this.cmdAddFavorite_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(236, 6);
            // 
            // mnuEditFav
            // 
            this.mnuEditFav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.mnuEditFav.Name = "mnuEditFav";
            this.mnuEditFav.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mnuEditFav.Size = new System.Drawing.Size(118, 92);
            this.mnuEditFav.Opening += new System.ComponentModel.CancelEventHandler(this.mnuEditFav_Opening);
            this.mnuEditFav.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuEditFav_ItemClicked);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.searchToolStripMenuItem.Image = global::NzbSearcher.Properties.Resources.Search;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.MouseEnter += new System.EventHandler(this.ToolStripMenuItem_MouseEnter);
            this.searchToolStripMenuItem.MouseLeave += new System.EventHandler(this.Form_MouseLeave);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.MouseEnter += new System.EventHandler(this.ToolStripMenuItem_MouseEnter);
            this.renameToolStripMenuItem.MouseLeave += new System.EventHandler(this.Form_MouseLeave);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.MouseEnter += new System.EventHandler(this.ToolStripMenuItem_MouseEnter);
            this.updateToolStripMenuItem.MouseLeave += new System.EventHandler(this.Form_MouseLeave);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::NzbSearcher.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.MouseEnter += new System.EventHandler(this.ToolStripMenuItem_MouseEnter);
            this.deleteToolStripMenuItem.MouseLeave += new System.EventHandler(this.Form_MouseLeave);
            // 
            // ListSplitter
            // 
            this.ListSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.ListSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.ListSplitter.Location = new System.Drawing.Point(0, 86);
            this.ListSplitter.Name = "ListSplitter";
            this.ListSplitter.Size = new System.Drawing.Size(890, 10);
            this.ListSplitter.TabIndex = 7;
            this.ListSplitter.TabStop = false;
            this.ListSplitter.Visible = false;
            this.ListSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.ListSplitter_SplitterMoved);
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Control;
            this.txtInfo.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInfo.Location = new System.Drawing.Point(0, 102);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfo.Size = new System.Drawing.Size(175, 50);
            this.txtInfo.TabIndex = 8;
            // 
            // mnuNzbContext
            // 
            this.mnuNzbContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdAddToQueue,
            this.cmdIMDB,
            this.cmdShowNFO});
            this.mnuNzbContext.Name = "mnuNzbContext";
            this.mnuNzbContext.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mnuNzbContext.Size = new System.Drawing.Size(317, 70);
            this.mnuNzbContext.Opening += new System.ComponentModel.CancelEventHandler(this.mnuNzbContext_Opening);
            // 
            // cmdAddToQueue
            // 
            this.cmdAddToQueue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddToQueue.Image = global::NzbSearcher.Properties.Resources.SABnzbd;
            this.cmdAddToQueue.Name = "cmdAddToQueue";
            this.cmdAddToQueue.ShortcutKeyDisplayString = "Enter / Dbl Click";
            this.cmdAddToQueue.Size = new System.Drawing.Size(316, 22);
            this.cmdAddToQueue.Text = "Add to SABnzbd\'s queue...";
            this.cmdAddToQueue.Click += new System.EventHandler(this.cmdAddToQueue_Click);
            // 
            // cmdIMDB
            // 
            this.cmdIMDB.Image = global::NzbSearcher.Properties.Resources.imdb;
            this.cmdIMDB.Name = "cmdIMDB";
            this.cmdIMDB.ShortcutKeyDisplayString = "Ctrl+F10";
            this.cmdIMDB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
            this.cmdIMDB.Size = new System.Drawing.Size(316, 22);
            this.cmdIMDB.Text = "IMDB";
            this.cmdIMDB.Click += new System.EventHandler(this.cmdIMDB_Click);
            // 
            // cmdShowNFO
            // 
            this.cmdShowNFO.Image = global::NzbSearcher.Properties.Resources.notepad;
            this.cmdShowNFO.Name = "cmdShowNFO";
            this.cmdShowNFO.ShortcutKeyDisplayString = "Ctrl+F11";
            this.cmdShowNFO.Size = new System.Drawing.Size(316, 22);
            this.cmdShowNFO.Text = "Show NFO";
            this.cmdShowNFO.Click += new System.EventHandler(this.cmdShowNFO_Click);
            // 
            // mnuIMDBmovies
            // 
            this.mnuIMDBmovies.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdSearchIMDB,
            this.cmdManageMyMovies,
            this.cmdRefreshMyMovies,
            this.txtIMDBfilter,
            this.toolStripSeparator2});
            this.mnuIMDBmovies.Name = "mnuFavorites";
            this.mnuIMDBmovies.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mnuIMDBmovies.Size = new System.Drawing.Size(194, 101);
            this.mnuIMDBmovies.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.mnuIMDBmovies_Closing);
            this.mnuIMDBmovies.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuIMDBmovies_ItemClicked);
            // 
            // cmdSearchIMDB
            // 
            this.cmdSearchIMDB.Image = global::NzbSearcher.Properties.Resources.imdb;
            this.cmdSearchIMDB.Name = "cmdSearchIMDB";
            this.cmdSearchIMDB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
            this.cmdSearchIMDB.Size = new System.Drawing.Size(193, 22);
            this.cmdSearchIMDB.Text = "Search IMDB";
            this.cmdSearchIMDB.Click += new System.EventHandler(this.cmdIMDB_Click);
            // 
            // cmdManageMyMovies
            // 
            this.cmdManageMyMovies.Name = "cmdManageMyMovies";
            this.cmdManageMyMovies.Size = new System.Drawing.Size(193, 22);
            this.cmdManageMyMovies.Text = "Manage \"My Movies\"";
            this.cmdManageMyMovies.Click += new System.EventHandler(this.cmdManageMyMovies_Click);
            // 
            // cmdRefreshMyMovies
            // 
            this.cmdRefreshMyMovies.Name = "cmdRefreshMyMovies";
            this.cmdRefreshMyMovies.Size = new System.Drawing.Size(193, 22);
            this.cmdRefreshMyMovies.Text = "Refresh \"My Movies\"";
            this.cmdRefreshMyMovies.Click += new System.EventHandler(this.cmdRefreshMyMovies_Click);
            // 
            // txtIMDBfilter
            // 
            this.txtIMDBfilter.Name = "txtIMDBfilter";
            this.txtIMDBfilter.Size = new System.Drawing.Size(100, 23);
            this.txtIMDBfilter.Text = "<filter>";
            this.txtIMDBfilter.TextChanged += new System.EventHandler(this.txtIMDBfilter_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(190, 6);
            // 
            // mnuEditMov
            // 
            this.mnuEditMov.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.moveToCategoryToolStripMenuItem,
            this.toolStripMenuItem4});
            this.mnuEditMov.Name = "mnuEditFav";
            this.mnuEditMov.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mnuEditMov.Size = new System.Drawing.Size(170, 92);
            this.mnuEditMov.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuEditMov_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripMenuItem1.Image = global::NzbSearcher.Properties.Resources.Search;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItem1.Text = "Search";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItem2.Text = "Open on IMDB";
            // 
            // moveToCategoryToolStripMenuItem
            // 
            this.moveToCategoryToolStripMenuItem.Name = "moveToCategoryToolStripMenuItem";
            this.moveToCategoryToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.moveToCategoryToolStripMenuItem.Text = "Move to Category";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Image = global::NzbSearcher.Properties.Resources.delete;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItem4.Text = "Delete";
            // 
            // SearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ListSplitter);
            this.Controls.Add(this.NzbResultList);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.SearchPanel);
            this.Name = "SearchControl";
            this.Size = new System.Drawing.Size(890, 285);
            this.Load += new System.EventHandler(this.SearchControl_Load);
            this.Resize += new System.EventHandler(this.SearchControl_Resize);
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            this.mnuFavorites.ResumeLayout(false);
            this.mnuEditFav.ResumeLayout(false);
            this.mnuNzbContext.ResumeLayout(false);
            this.mnuIMDBmovies.ResumeLayout(false);
            this.mnuIMDBmovies.PerformLayout();
            this.mnuEditMov.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView NzbResultList;
        private System.Windows.Forms.Panel SearchPanel;
        private System.Windows.Forms.ComboBox cboSearch;
        private System.Windows.Forms.Button cmdSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMinSize;
        private System.Windows.Forms.Label lblMaxDays;
        private System.Windows.Forms.ComboBox cboMaxDays;
        private System.Windows.Forms.TextBox txtMinSize;
        private System.Windows.Forms.ContextMenuStrip mnuFavorites;
        private System.Windows.Forms.ToolStripMenuItem cmdAddFavorite;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip mnuEditFav;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel AdditionalControlsPanel;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Splitter ListSplitter;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.ContextMenuStrip mnuNzbContext;
        private System.Windows.Forms.ToolStripMenuItem cmdAddToQueue;
        private System.Windows.Forms.ToolStripMenuItem cmdIMDB;
        private System.Windows.Forms.ToolStripMenuItem cmdShowNFO;
        private System.Windows.Forms.ContextMenuStrip mnuIMDBmovies;
        private System.Windows.Forms.ToolStripMenuItem cmdSearchIMDB;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cmdRefreshMyMovies;
        private System.Windows.Forms.ToolStripMenuItem cmdManageMyMovies;
        private System.Windows.Forms.ContextMenuStrip mnuEditMov;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem moveToCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripTextBox txtIMDBfilter;
    }
}
