using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NzbSearcher
{
    public partial class InputBox : Form
    {
        public string NewValue { get; set; }

        public InputBox()
        {
            InitializeComponent();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {

        }

        public static DialogResult Show(string Caption, string Question, ref string Value)
        {
            InputBox boxy = new InputBox();
            boxy.Text = Caption;
            boxy.lblQuestion.Text = Question;
            boxy.txtValue.Text = Value;

            DialogResult res = boxy.ShowDialog();
            if (boxy.NewValue != null)
            {
                Value = boxy.NewValue;
                return DialogResult.OK;
            }
            return DialogResult.Cancel;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            NewValue = this.txtValue.Text;
            this.Close();
        }

        private void InputBox_Activated(object sender, EventArgs e)
        {
            txtValue.Focus();

            //Select all text, but rather with the cursor on the START of the text
            // will solve the issue that if its a very long text, it shows the beginning instead of the end
            SendKeys.Send("{END}+{HOME}"); // send END, then SHIFT+HOME
        }
    }
}
