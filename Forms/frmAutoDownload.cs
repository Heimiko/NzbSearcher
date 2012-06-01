using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NzbSearcher
{
    public partial class frmAutoDownload : Form
    {
        public AutoEpisode NewEpisode { get; private set; }

        public delegate void EpisodeSavedEvent(frmAutoDownload frm, AutoEpisode ep);
        public event EpisodeSavedEvent EpisodeSaved;

        ISearchProvider _SearchProv = null;
        bool _Initializing = true;

        public frmAutoDownload()
        {
            InitializeComponent();

            //initialize defaults
            _SearchProv = new NzbIndex();

            NewEpisode = new AutoEpisode();
            NewEpisode.EpisodeNr = 1;
            NewEpisode.EpisodeNrMinDigits = 2;
            NewEpisode.SeasonNr = 1;
            NewEpisode.SeasonNrMinDigits = 2;
            NewEpisode.FriendlyName = "%N S%SE%E %T";
            NewEpisode.SearchProviderType = _SearchProv.GetType().FullName;
            NewEpisode.SearchText = "%N S%SE%E";
            NewEpisode.SABcat = "series"; //default category

            InitScreen();
        }

        public frmAutoDownload(AutoEpisode ep)
        {
            InitializeComponent();
            NewEpisode = ep.Clone();
            _SearchProv = (ISearchProvider) Activator.CreateInstance(Type.GetType(ep.SearchProviderType));
            InitScreen();
        }

        void InitScreen()
        {
            foreach (string cat in Global.SABnzbd.Status.Categories)
                cboSABcat.Items.Add(cat);
            
            txtName.Text = NewEpisode.DisplayName;
            txtSearch.Text = NewEpisode.SearchText;
            txtMinSize.Text = NewEpisode.MinSize > 0 ? NewEpisode.MinSize.ToString() : string.Empty;
            txtMaxSize.Text = NewEpisode.MaxSize > 0 ? NewEpisode.MaxSize.ToString() : string.Empty;
            txtFriendlyName.Text = NewEpisode.FriendlyName;
            txtSeasonNr.Text = NewEpisode.SeasonNr.ToString();
            txtEpisodeNr.Text = NewEpisode.EpisodeNr.ToString();
            cboSABcat.Text = NewEpisode.SABcat;
            txtResultFilter.Text = NewEpisode.ResultFilter;

            if (cboSABcat.SelectedIndex < 0 && cboSABcat.Items.Count > 0)
                cboSABcat.SelectedIndex = 0;

            _Initializing = false;
            UpdateControls();
        }

        void FromScreen()
        {
            if (_Initializing)
                return;

            NewEpisode.DisplayName = txtName.Text;
            NewEpisode.SearchText = txtSearch.Text;
            NewEpisode.MinSize = txtMinSize.Text.Length > 0 ? int.Parse(txtMinSize.Text) : 0;
            NewEpisode.MaxSize = txtMaxSize.Text.Length > 0 ? int.Parse(txtMaxSize.Text) : 0;
            NewEpisode.FriendlyName = txtFriendlyName.Text;
            NewEpisode.SABcat = cboSABcat.Text;
            NewEpisode.ResultFilter = txtResultFilter.Text;
            
            if (txtSeasonNr.Text.Length > 0)
                NewEpisode.SeasonNr = int.Parse(txtSeasonNr.Text);
            if (txtEpisodeNr.Text.Length > 0)
                NewEpisode.EpisodeNr = int.Parse(txtEpisodeNr.Text);
        }

        void UpdateControls()
        {
            if (_Initializing)
                return;

            FromScreen();
            lblSearch.Text = NewEpisode.ReplaceVars(txtSearch.Text);
            lblFriendlyName.Text = NewEpisode.ReplaceVars(txtFriendlyName.Text);

            cmdOK.Enabled = txtName.Text.Length > 0;
        }

        private void txtSeasonNr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Nr = 100 - int.Parse(txtSeasonNr.Text);
                if (Nr != scrSeason.Value)
                    scrSeason.Value = Nr;

                UpdateControls();
            }
            catch (Exception) { }
        }

        private void txtEpisodeNr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Nr = 100 - int.Parse(txtEpisodeNr.Text);
                if (Nr != scrEpisode.Value)
                    scrEpisode.Value = Nr;

                UpdateControls();
            }
            catch (Exception) { }
        }


        private void scrSeason_Scroll(object sender, ScrollEventArgs e)
        {
            string Nr = (100 - scrSeason.Value).ToString();
            if (txtSeasonNr.Text != Nr)
                txtSeasonNr.Text = Nr;
        }

        private void scrEpisode_Scroll(object sender, ScrollEventArgs e)
        {
            string Nr = (100 - scrEpisode.Value).ToString();
            if (txtEpisodeNr.Text != Nr)
                txtEpisodeNr.Text = Nr;
        }

        private void txtSeasonNr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 20 && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private void txtEpisodeNr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 20 && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void txtFriendlyName_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            FromScreen();
            this.DialogResult = DialogResult.OK;
            this.Close();

            if (EpisodeSaved != null)
                EpisodeSaved(this, NewEpisode);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblRegExHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://msdn.microsoft.com/en-us/library/az24scfc.aspx");
        }

    }
}
