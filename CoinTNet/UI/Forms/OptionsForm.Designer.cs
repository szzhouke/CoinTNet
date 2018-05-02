namespace CoinTNet.UI.Forms
{
    partial class OptionsForm
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
            this.tvSections = new System.Windows.Forms.TreeView();
            this.pnlPlaceHolder = new System.Windows.Forms.Panel();
            this.btnOK = new CoinTNet.UI.Controls.Common.MyButton();
            this.btnCancel = new CoinTNet.UI.Controls.Common.MyButton();
            this.SuspendLayout();
            // 
            // tvSections
            // 
            this.tvSections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvSections.Location = new System.Drawing.Point(18, 18);
            this.tvSections.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tvSections.Name = "tvSections";
            this.tvSections.Size = new System.Drawing.Size(180, 470);
            this.tvSections.TabIndex = 0;
            this.tvSections.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSections_AfterSelect);
            // 
            // pnlPlaceHolder
            // 
            this.pnlPlaceHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPlaceHolder.Location = new System.Drawing.Point(224, 18);
            this.pnlPlaceHolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlPlaceHolder.Name = "pnlPlaceHolder";
            this.pnlPlaceHolder.Size = new System.Drawing.Size(802, 472);
            this.pnlPlaceHolder.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(778, 503);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 38);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(914, 503);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 38);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.ClientSize = new System.Drawing.Size(1044, 560);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlPlaceHolder);
            this.Controls.Add(this.tvSections);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "OptionsForm";
            this.Text = " ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvSections;
        private System.Windows.Forms.Panel pnlPlaceHolder;
        private Controls.Common.MyButton btnOK;
        private Controls.Common.MyButton btnCancel;
    }
}