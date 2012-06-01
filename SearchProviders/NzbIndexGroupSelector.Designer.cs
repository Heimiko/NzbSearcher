namespace NzbSearcher
{
    partial class NzbIndexGroupSelector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstGroups = new System.Windows.Forms.ListView();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstGroups
            // 
            this.lstGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstGroups.CheckBoxes = true;
            this.lstGroups.FullRowSelect = true;
            this.lstGroups.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstGroups.Location = new System.Drawing.Point(0, 23);
            this.lstGroups.Name = "lstGroups";
            this.lstGroups.Size = new System.Drawing.Size(250, 464);
            this.lstGroups.TabIndex = 0;
            this.lstGroups.UseCompatibleStateImageBehavior = false;
            this.lstGroups.View = System.Windows.Forms.View.Details;
            this.lstGroups.VirtualMode = true;
            this.lstGroups.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lstGroups_RetrieveVirtualItem);
            this.lstGroups.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstGroups_MouseClick);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(0, 0);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(151, 20);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(153, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "(Start typing to filter)";
            // 
            // NzbIndexGroupSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 569);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lstGroups);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NzbIndexGroupSelector";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NzbIndexGroupSelector";
            this.Activated += new System.EventHandler(this.NzbIndexGroupSelector_Activated);
            this.Deactivate += new System.EventHandler(this.NzbIndexGroupSelector_Deactivate);
            this.Load += new System.EventHandler(this.NzbIndexGroupSelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstGroups;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label1;
    }
}