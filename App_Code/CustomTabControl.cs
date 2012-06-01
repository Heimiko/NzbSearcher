using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace NzbSearcher
{

    /// <summary>
    /// interface to a UserControl
    /// add methods/properties when needed
    /// they don't need implementation, just declaration (as defined in UserControl class)
    /// </summary>
    public interface IUserControl
    {
        bool Visible { get; set; }
        DockStyle Dock { get; set; }
        void BringToFront();

        int Width { get; set; }
        int Height { get; set; }
    }

    public interface ICustomTabControl : IUserControl
    {
        Image Icon { get; }

        IGUIelementConfig Config { get; }
        ToolStripButton Button { get; set; }

        string DisplayName { get; }
        string ToolTip { get; }

        void OnTabFocus();

        void OnKeyPreview(object sender, KeyEventArgs e);

        void AddToolbarItems(ToolStripItemCollection col);
    }

    public class CustomTabControlsCollection : List<ICustomTabControl>
    {
        private CustomTabControlsCollection(IEnumerable<ICustomTabControl> collection) : base(collection) { }

        /// <summary>
        /// Here we define all our search providers
        /// </summary>
        static CustomTabControlsCollection _CustomTabControls = new CustomTabControlsCollection(new ICustomTabControl[]
        {
            new AutoDownloader_Control(),
            new SABnzbd_Control()
        });

        public static CustomTabControlsCollection CustomTabControls { get { return _CustomTabControls; } }
    }
}
