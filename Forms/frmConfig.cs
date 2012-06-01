using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NzbSearcher
{
    public partial class frmConfig : Form
    {
        public string InitialActiveTab { get; set; }

        List<TabConfig> _TabsConfig = new List<TabConfig>();

        public frmConfig()
        {
            InitializeComponent();

            //Fill Tab Config collection

            foreach (ISearchProvider SearchProv in SearchProvidersCollection.Providers)
            {
                _TabsConfig.Add(new TabConfig
                {
                    Icon = ResizedIcon(SearchProv.Icon),
                    Name = SearchProv.ToolTip,
                    Enabled = !SearchProv.Config.Disabled,
                    UseInAll = SearchProv.Config.UseForCollectiveSearch,
                    Config = SearchProv.Config
                });
            }

            //add non search providers
            _TabsConfig.Add(new TabConfig { Icon = ResizedIcon(Properties.Resources.auto), Name = "Auto Downloader", Enabled = !Global.AutoEpisodeDownloader.Config.Disabled, Config = Global.AutoEpisodeDownloader.Config });
            _TabsConfig.Add(new TabConfig { Icon = ResizedIcon(Properties.Resources.SABnzbd), Name = "SABnzbd", Enabled = !Global.SABnzbd.Config.Disabled, Config = Global.SABnzbd.Config });
        }
        
        private void frmConfig_Load(object sender, EventArgs e)
        {
            //General
            chkShowInTray.Checked = Global.Config.ShowInTray;
            chkShowButtonText.Checked = Global.Config.ShowButtonText;
            chkPortableConfig.Checked = Global.Config.PortableConfig;
            txtWatchDir.Text = Global.FolderWatcher.Config.Path;
            chkDeleteNzb.Checked = Global.FolderWatcher.Config.DeleteNzb;

            //Nzb Handling
            txtSaveToDir.Text = Global.Config.NzbSaveToDir;
            txtOpenWith.Text = Global.Config.NzbOpenWith;
            switch (Global.Config.NzbHandling)
            {
                case NzbHandling.UploadNZB: radUploadNZB.Checked = true; break;
                case NzbHandling.SendURL: radSendURL.Checked = true; break;
                case NzbHandling.SaveToDir: radSaveToDir.Checked = true; break;
                case NzbHandling.OpenWith: radOpenWith.Checked = true; break;
                default: radUploadNZB.Checked = true; break;
            }

            //SABnzbd
            txtHost.Text = Global.SABnzbd.Config.HostURL;
            txtAPIkey.Text = Global.SABnzbd.Config.APIkey;
            txtUserName.Text = Global.SABnzbd.Config.UserName;
            txtPassword.Text = Global.SABnzbd.Config.Password;

            //Auto Downloader
            txtAutoInterval.Text = Global.AutoEpisodeDownloader.Config.Interval.ToString();
            chkDontCheckRecentEp.Checked = Global.AutoEpisodeDownloader.Config.DontCheckRecentEp;
            chkDontCheckOldEp.Checked = Global.AutoEpisodeDownloader.Config.DontCheckOldEp;
            chkCheckNextEpisode.Checked = Global.AutoEpisodeDownloader.Config.CheckNextEpisode;
            chkCheckNextSeason.Checked = Global.AutoEpisodeDownloader.Config.CheckNextSeason;

            //Search Engines
            chkSearchInToolbar.Checked = Global.Config.SearchInToolbar;
            txtItemsPerPage.Text = Global.Config.ItemsPerPage.ToString();
            txtNzbMatrix_UserName.Text = Config<NzbMatrixConfig>.Value.UserName;
            txtNzbMatrix_ApiKey.Text = Config<NzbMatrixConfig>.Value.ApiKey;
            txtNZBsu_ApiKey.Text = Config<NZBsuConfig>.Value.ApiKey;
            txtUNS_UserName.Text = Config<UsenetServerConfig>.Value.UserName;
            txtUNS_Password.Text = Config<UsenetServerConfig>.Value.Password;

            //IMDB
            txtImdbUserName.Text = Global.IMDB.Config.UserName;
            txtImdbPassword.Text = Global.IMDB.Config.Password;
            chkDisplayMyMovs.Checked = Global.IMDB.Config.DisplayMyMovs;
            radIMDB_None.Checked = Global.IMDB.Config.MenuDisplayType == IMDBmenuDisplayType.Flat;
            radIMDB_Years.Checked = Global.IMDB.Config.MenuDisplayType == IMDBmenuDisplayType.Years;
            radIMDB_Categories.Checked = Global.IMDB.Config.MenuDisplayType == IMDBmenuDisplayType.Categories;
            radIMDB_Both.Checked = Global.IMDB.Config.MenuDisplayType == IMDBmenuDisplayType.BothYearsAndCategories;
            chkDisplayYear.Checked = Global.IMDB.Config.DisplayYear;
            chkDisplayCategory.Checked = Global.IMDB.Config.DisplayCategory;
            chkIncludeYear.Checked = Global.IMDB.Config.IncludeYearOnSearch;

            //Tabs
            lstTabs.DataSource = _TabsConfig;

            //set some cells invisible
            foreach (DataGridViewRow row in lstTabs.Rows)
            {
                DataGridViewCell UseInAll = row.Cells["UseInAll"];
                if (UseInAll.Value == null || row.Cells["Config"].Value is CollectiveConfig)
                    lstTabs.AddInvisibleCell(UseInAll);
            }
        }

        Image ResizedIcon(Image Img)
        {
            return new Bitmap(Img, new Size(16, 16));
        }

        private void frmConfig_Activated(object sender, EventArgs e)
        {
            if (InitialActiveTab != null && InitialActiveTab.Length > 0)
            {
                tabControl1.SelectTab(InitialActiveTab);
                InitialActiveTab = null;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            //General
            Global.Config.ShowInTray = chkShowInTray.Checked;
            Global.Config.ShowButtonText = chkShowButtonText.Checked;
            Global.Config.PortableConfig = chkPortableConfig.Checked;
            Global.FolderWatcher.Config.Path = txtWatchDir.Text;
            Global.FolderWatcher.Config.DeleteNzb = chkDeleteNzb.Checked;

            //Nzb Handling
            Global.Config.NzbSaveToDir = txtSaveToDir.Text;
            Global.Config.NzbOpenWith = txtOpenWith.Text;

            if (radSendURL.Checked)
                Global.Config.NzbHandling = NzbHandling.SendURL;
            else if (radUploadNZB.Checked)
                Global.Config.NzbHandling = NzbHandling.UploadNZB;
            else if (radSaveToDir.Checked)
                Global.Config.NzbHandling = NzbHandling.SaveToDir;
            else if (radOpenWith.Checked)
                Global.Config.NzbHandling = NzbHandling.OpenWith;
            else
                Global.Config.NzbHandling = NzbHandling.UploadNZB;

            //SABnzbd
            Global.SABnzbd.Config.HostURL = txtHost.Text;
            Global.SABnzbd.Config.APIkey = txtAPIkey.Text;
            Global.SABnzbd.Config.UserName = txtUserName.Text;
            Global.SABnzbd.Config.Password = txtPassword.Text;

            //Auto Downloader
            Global.AutoEpisodeDownloader.Config.Interval = Global.TryParseInt(txtAutoInterval.Text, 60);
            Global.AutoEpisodeDownloader.Config.DontCheckRecentEp = chkDontCheckRecentEp.Checked;
            Global.AutoEpisodeDownloader.Config.DontCheckOldEp = chkDontCheckOldEp.Checked;
            Global.AutoEpisodeDownloader.Config.CheckNextEpisode = chkCheckNextEpisode.Checked;
            Global.AutoEpisodeDownloader.Config.CheckNextSeason = chkCheckNextSeason.Checked;

            //Search Engines
            Global.Config.SearchInToolbar = chkSearchInToolbar.Checked;
            Global.Config.ItemsPerPage = Global.TryParseInt(txtItemsPerPage.Text, 25);
            Config<NzbMatrixConfig>.Value.UserName = txtNzbMatrix_UserName.Text;
            Config<NzbMatrixConfig>.Value.ApiKey = txtNzbMatrix_ApiKey.Text;
            Config<NZBsuConfig>.Value.ApiKey = txtNZBsu_ApiKey.Text;
            Config<UsenetServerConfig>.Value.UserName = txtUNS_UserName.Text;
            Config<UsenetServerConfig>.Value.Password = txtUNS_Password.Text;

            //IMDB
            Global.IMDB.Config.UserName = txtImdbUserName.Text;
            Global.IMDB.Config.Password = txtImdbPassword.Text;
            Global.IMDB.Config.DisplayMyMovs = chkDisplayMyMovs.Checked;
            Global.IMDB.Config.DisplayYear = chkDisplayYear.Checked;
            Global.IMDB.Config.DisplayCategory = chkDisplayCategory.Checked;
            Global.IMDB.Config.IncludeYearOnSearch = chkIncludeYear.Checked;

            if (radIMDB_None.Checked)
                Global.IMDB.Config.MenuDisplayType = IMDBmenuDisplayType.Flat;
            if (radIMDB_Years.Checked)
                Global.IMDB.Config.MenuDisplayType = IMDBmenuDisplayType.Years;
            if (radIMDB_Categories.Checked)
                Global.IMDB.Config.MenuDisplayType = IMDBmenuDisplayType.Categories;
            if (radIMDB_Both.Checked) 
                Global.IMDB.Config.MenuDisplayType = IMDBmenuDisplayType.BothYearsAndCategories;

            //Tabs
            foreach (TabConfig TabCfg in _TabsConfig)
            {
                TabCfg.Config.Disabled = !TabCfg.Enabled;
                if (TabCfg.Config is IProviderConfig && !(TabCfg.Config is CollectiveConfig))
                    ((IProviderConfig)TabCfg.Config).UseForCollectiveSearch = TabCfg.UseInAll.Value;
            }

            if (Global.Config.NzbHandling != NzbHandling.SendURL &&
                Global.Config.NzbHandling != NzbHandling.UploadNZB)
                    Global.SABnzbd.Config.Disabled = true; //disable SABnzbd if handling incompatible

            this.Enabled = false;
            this.Refresh();

            NzbSearcher.Config.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAutoInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 20 && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private void cmdBrowseDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderDlg = new FolderBrowserDialog();
            if (txtSaveToDir.Text.Length > 0)
                FolderDlg.SelectedPath = txtSaveToDir.Text;
            if (FolderDlg.ShowDialog() == DialogResult.OK)
                txtSaveToDir.Text = FolderDlg.SelectedPath;
        }

        private void cmdBrowseApp_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDlg = new OpenFileDialog();
            FileDlg.Filter = "Applications (*.exe)|*.exe";
            if (FileDlg.ShowDialog() == DialogResult.OK)
            {
                txtOpenWith.Text = FileDlg.FileName + " \"%1\"";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderDlg = new FolderBrowserDialog();
            if (txtSaveToDir.Text.Length > 0)
                FolderDlg.SelectedPath = txtSaveToDir.Text;
            if (FolderDlg.ShowDialog() == DialogResult.OK)
                txtWatchDir.Text = FolderDlg.SelectedPath;
        }
    }


    class TabConfig
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool? UseInAll { get; set; }
        public ICanBeDisabled Config { get; set; }
        public Image Icon { get; set; }
    }


    public class DataGridViewWithInvisibleCells : DataGridView
    {
        private List<Point> invisibleColumnRow = new List<Point>();

        public void AddInvisibleCell(DataGridViewCell Cell)
        {
            AddInvisibleCell(Cell.ColumnIndex, Cell.RowIndex);
        }

        public void AddInvisibleCell(int columnIndex, int rowIndex)
        {
            Point p = new Point(columnIndex, rowIndex);
            invisibleColumnRow.Add(p);
            this[p.X, p.Y].ReadOnly = true;
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);
            for (int i = 0; i < invisibleColumnRow.Count; i++)
            {
                this[invisibleColumnRow[i].X, invisibleColumnRow[i].Y].ReadOnly = true;
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);
            Point p = new Point(e.ColumnIndex, e.RowIndex);
            if (invisibleColumnRow.Contains(p))
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackgroundColor), e.CellBounds);
                e.Handled = true;
            }
        }
    }

}
