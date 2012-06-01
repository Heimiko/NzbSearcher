using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace NzbSearcher
{
    /// <summary>
    /// This class wraps the list items in the combo box
    /// </summary>
    public class CheckComboBoxItem
    {
        /// <summary>
        /// C'tor - creates a CheckComboBoxItem
        /// </summary>
        /// <param name="text">Label of the check box in the drop down list</param>
        /// <param name="initialCheckState">Initial value for the check box (true=checked)</param>
        public CheckComboBoxItem( string text, bool initialCheckState )
        {
            _checkState = initialCheckState;
            _text = text;
        }

        private bool _checkState = false;
        /// <summary>
        /// Gets the check value (true=checked)
        /// </summary>
        public bool CheckState
        {
            get { return _checkState; }
            set { _checkState = value; }
        }

        private string _text = "";
        /// <summary>
        /// Gets the label of the check box
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private object _tag = null;
        /// <summary>
        /// User defined data
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// This is used to keep the edit control portion of the combo box consistent
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Search Specific Groups";
        }

    }

    /// <summary>
    /// Inherits from ComboBox and handles DrawItem and SelectedIndexChanged events to create an
    /// owner drawn combo box drop-down.  The contents of the dropdown are rendered using the
    /// CheckBoxRenderer class.
    /// </summary>
    public partial class CheckComboBox : ComboBox
    {
        /// <summary>
        /// C'tor
        /// </summary>
        public CheckComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += new DrawItemEventHandler(CheckComboBox_DrawItem);
            this.SelectedIndexChanged += new EventHandler( CheckComboBox_SelectedIndexChanged );
            SelectedText = "Search Specific Groups";
        }

        /// <summary>
        /// Invoked when the selected index is changed on the dropdown.  This sets the check state
        /// of the CheckComboBoxItem and fires the public event CheckStateChanged using the 
        /// CheckComboBoxItem as the event sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CheckComboBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            CheckComboBoxItem item = (CheckComboBoxItem)SelectedItem;
            item.CheckState = !item.CheckState;
            if (CheckStateChanged != null)
                CheckStateChanged(item, e);
        }

        /// <summary>
        /// Invoked when the ComboBox has to render the drop-down items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CheckComboBox_DrawItem( object sender, DrawItemEventArgs e )
        {
			// make sure the index is valid (sanity check)
			if( e.Index == -1 )
			{
				return;
			}

			// test the item to see if its a CheckComboBoxItem
			if( !( Items[ e.Index ] is CheckComboBoxItem ) )
			{
				// it's not, so just render it as a default string
				e.Graphics.DrawString(
					Items[ e.Index ].ToString(),
					this.Font,
					Brushes.Black,
					new Point( e.Bounds.X, e.Bounds.Y ) );
				return;
			}

			// get the CheckComboBoxItem from the collection
			CheckComboBoxItem box = (CheckComboBoxItem)Items[ e.Index ];

			// render it
			CheckBoxRenderer.RenderMatchingApplicationState = true;
			CheckBoxRenderer.DrawCheckBox(
				e.Graphics,
				new Point( e.Bounds.X, e.Bounds.Y ),
				e.Bounds,
				box.Text,
				this.Font,
				( e.State & DrawItemState.Focus ) != 0,
				box.CheckState ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal );
		}

        /// <summary>
        /// Fired when the user clicks a check box item in the drop-down list
        /// </summary>
        public event EventHandler CheckStateChanged;

    }
}


