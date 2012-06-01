namespace NzbSearcher
{
    partial class ImdbWatchList
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
            this.lstWatchList = new System.Windows.Forms.ListView();
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Year = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Rating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cboMyLists = new System.Windows.Forms.ToolStripComboBox();
            this.cmdAddList = new System.Windows.Forms.ToolStripButton();
            this.cmdDeleteList = new System.Windows.Forms.ToolStripButton();
            this.cmdRefresh = new System.Windows.Forms.ToolStripButton();
            this.cmdManageList = new System.Windows.Forms.ToolStripButton();
            this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmdSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdOpenOnIMDB = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdDeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.mnuContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstWatchList
            // 
            this.lstWatchList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Title,
            this.Year,
            this.Rating});
            this.lstWatchList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstWatchList.FullRowSelect = true;
            this.lstWatchList.HideSelection = false;
            this.lstWatchList.Location = new System.Drawing.Point(0, 51);
            this.lstWatchList.MultiSelect = false;
            this.lstWatchList.Name = "lstWatchList";
            this.lstWatchList.Size = new System.Drawing.Size(247, 573);
            this.lstWatchList.TabIndex = 4;
            this.lstWatchList.UseCompatibleStateImageBehavior = false;
            this.lstWatchList.View = System.Windows.Forms.View.Details;
            this.lstWatchList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstWatchList_ColumnClick);
            this.lstWatchList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstWatchList_KeyUp);
            this.lstWatchList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstWatchList_MouseDoubleClick);
            this.lstWatchList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstWatchList_MouseUp);
            // 
            // Title
            // 
            this.Title.Text = "Title";
            this.Title.Width = 142;
            // 
            // Year
            // 
            this.Year.Text = "Year";
            this.Year.Width = 37;
            // 
            // Rating
            // 
            this.Rating.Text = "Rating";
            this.Rating.Width = 43;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cboMyLists,
            this.cmdAddList,
            this.cmdDeleteList,
            this.cmdRefresh,
            this.cmdManageList});
            this.toolStrip1.Location = new System.Drawing.Point(0, 23);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(218, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // cboMyLists
            // 
            this.cboMyLists.AutoSize = false;
            this.cboMyLists.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMyLists.MaxDropDownItems = 50;
            this.cboMyLists.Name = "cboMyLists";
            this.cboMyLists.Size = new System.Drawing.Size(121, 23);
            this.cboMyLists.SelectedIndexChanged += new System.EventHandler(this.cboMyLists_SelectedIndexChanged);
            // 
            // cmdAddList
            // 
            this.cmdAddList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdAddList.Image = global::NzbSearcher.Properties.Resources.add;
            this.cmdAddList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdAddList.Name = "cmdAddList";
            this.cmdAddList.Size = new System.Drawing.Size(23, 22);
            this.cmdAddList.Text = "toolStripButton1";
            this.cmdAddList.ToolTipText = "Add another list...";
            this.cmdAddList.Click += new System.EventHandler(this.cmdAddList_Click);
            // 
            // cmdDeleteList
            // 
            this.cmdDeleteList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdDeleteList.Image = global::NzbSearcher.Properties.Resources.delete;
            this.cmdDeleteList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdDeleteList.Name = "cmdDeleteList";
            this.cmdDeleteList.Size = new System.Drawing.Size(23, 22);
            this.cmdDeleteList.Text = "toolStripButton2";
            this.cmdDeleteList.ToolTipText = "Remove currently selected list";
            this.cmdDeleteList.Click += new System.EventHandler(this.cmdDeleteList_Click);
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdRefresh.Image = global::NzbSearcher.Properties.Resources.refresh;
            this.cmdRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(23, 22);
            this.cmdRefresh.Text = "Refresh List";
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // cmdManageList
            // 
            this.cmdManageList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdManageList.Image = global::NzbSearcher.Properties.Resources.imdb;
            this.cmdManageList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdManageList.Name = "cmdManageList";
            this.cmdManageList.Size = new System.Drawing.Size(23, 22);
            this.cmdManageList.Text = "toolStripButton3";
            this.cmdManageList.ToolTipText = "Manage list on IMDB";
            this.cmdManageList.Click += new System.EventHandler(this.cmdManageList_Click);
            // 
            // mnuContext
            // 
            this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdSearch,
            this.cmdOpenOnIMDB,
            this.cmdDeleteItem});
            this.mnuContext.Name = "mnuNzbContext";
            this.mnuContext.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mnuContext.Size = new System.Drawing.Size(209, 70);
            // 
            // cmdSearch
            // 
            this.cmdSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSearch.Image = global::NzbSearcher.Properties.Resources.Search;
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.ShortcutKeyDisplayString = "Enter / Dbl Click";
            this.cmdSearch.Size = new System.Drawing.Size(208, 22);
            this.cmdSearch.Text = "Search";
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // cmdOpenOnIMDB
            // 
            this.cmdOpenOnIMDB.Image = global::NzbSearcher.Properties.Resources.imdb;
            this.cmdOpenOnIMDB.Name = "cmdOpenOnIMDB";
            this.cmdOpenOnIMDB.ShortcutKeyDisplayString = "Ctrl+F10";
            this.cmdOpenOnIMDB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
            this.cmdOpenOnIMDB.Size = new System.Drawing.Size(208, 22);
            this.cmdOpenOnIMDB.Text = "Open on IMDB";
            this.cmdOpenOnIMDB.Click += new System.EventHandler(this.cmdOpenOnIMDB_Click);
            // 
            // cmdDeleteItem
            // 
            this.cmdDeleteItem.Image = global::NzbSearcher.Properties.Resources.delete;
            this.cmdDeleteItem.Name = "cmdDeleteItem";
            this.cmdDeleteItem.ShortcutKeyDisplayString = "Del";
            this.cmdDeleteItem.Size = new System.Drawing.Size(208, 22);
            this.cmdDeleteItem.Text = "Delete";
            this.cmdDeleteItem.Click += new System.EventHandler(this.cmdDeleteItem_Click);
            // 
            // ImdbWatchList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lstWatchList);
            this.Name = "ImdbWatchList";
            this.Size = new System.Drawing.Size(247, 624);
            this.VisibleChanged += new System.EventHandler(this.ImdbWatchList_VisibleChanged);
            this.Resize += new System.EventHandler(this.ImdbWatchList_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.mnuContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstWatchList;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.ColumnHeader Year;
        private System.Windows.Forms.ColumnHeader Rating;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox cboMyLists;
        private System.Windows.Forms.ToolStripButton cmdAddList;
        private System.Windows.Forms.ToolStripButton cmdDeleteList;
        private System.Windows.Forms.ToolStripButton cmdManageList;
        private System.Windows.Forms.ContextMenuStrip mnuContext;
        private System.Windows.Forms.ToolStripMenuItem cmdSearch;
        private System.Windows.Forms.ToolStripMenuItem cmdOpenOnIMDB;
        private System.Windows.Forms.ToolStripMenuItem cmdDeleteItem;
        private System.Windows.Forms.ToolStripButton cmdRefresh;
    }
}
