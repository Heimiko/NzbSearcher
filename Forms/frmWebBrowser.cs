using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace NzbSearcher
{
    public partial class frmWebBrowser : Form
    {
        public WebBrowser WebBrowser { get { return webBrowser1; } }

        static frmWebBrowser() //one-time constructor
        {
            DisableClickSounds(); // disable click sounds for this process
        }

        public frmWebBrowser()
        {
            InitializeComponent();
        }


        const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        const int SET_FEATURE_ON_THREAD = 0x00000001;
        const int SET_FEATURE_ON_PROCESS = 0x00000002;
        const int SET_FEATURE_IN_REGISTRY = 0x00000004;
        const int SET_FEATURE_ON_THREAD_LOCALMACHINE = 0x00000008;
        const int SET_FEATURE_ON_THREAD_INTRANET = 0x00000010;
        const int SET_FEATURE_ON_THREAD_TRUSTED = 0x00000020;
        const int SET_FEATURE_ON_THREAD_INTERNET = 0x00000040;
        const int SET_FEATURE_ON_THREAD_RESTRICTED = 0x00000080;

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(
            int FeatureEntry,
            [MarshalAs(UnmanagedType.U4)] int dwFlags,
            bool fEnable);

        static void DisableClickSounds()
        {
            try
            {
                CoInternetSetFeatureEnabled(
                    FEATURE_DISABLE_NAVIGATION_SOUNDS,
                    SET_FEATURE_ON_PROCESS,
                    true);
            }
            catch (Exception) { /* something went wrong - unsupported? */ }
        }


    }
}
