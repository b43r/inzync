/*
 * InZync
 * 
 * Copyright (C) 2020 by Simon Baer
 *
 * This program is free software; you can redistribute it and/or modify it under the terms
 * of the GNU General Public License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program;
 * If not, see http://www.gnu.org/licenses/.
 * 
 */

using System.Windows.Forms;

namespace InZync.Forms
{
    /// <summary>
    /// This is the about dialog.
    /// </summary>
    public class About : Form
    {
        private Button cmdOk;
        private PictureBox imgLeft;
        private Label lblTitle;
        private Label lblCredits;
        private Label lblVersion;
        private PictureBox imgIcon;
        private System.ComponentModel.Container components = null;

        public About()
        {
            InitializeComponent();

            // set the label containing the program version
            lblVersion.Text = $"Version: {Application.ProductVersion} (2020/01/10)";
        }

        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose( disposing );
        }

        #region Vom Windows Form-Designer generierter Code
        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.cmdOk = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.imgLeft = new System.Windows.Forms.PictureBox();
            this.lblCredits = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.imgIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(361, 257);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 0;
            this.cmdOk.Text = "OK";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(216, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(136, 32);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "InZync";
            // 
            // imgLeft
            // 
            this.imgLeft.Image = ((System.Drawing.Image)(resources.GetObject("imgLeft.Image")));
            this.imgLeft.Location = new System.Drawing.Point(12, 56);
            this.imgLeft.Name = "imgLeft";
            this.imgLeft.Size = new System.Drawing.Size(197, 224);
            this.imgLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgLeft.TabIndex = 2;
            this.imgLeft.TabStop = false;
            // 
            // lblCredits
            // 
            this.lblCredits.Location = new System.Drawing.Point(218, 48);
            this.lblCredits.Name = "lblCredits";
            this.lblCredits.Size = new System.Drawing.Size(152, 23);
            this.lblCredits.TabIndex = 3;
            this.lblCredits.Text = "(c) 2020 by Simon Baer,";
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(218, 86);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(218, 23);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "lblVersion";
            // 
            // imgIcon
            // 
            this.imgIcon.Image = ((System.Drawing.Image)(resources.GetObject("imgIcon.Image")));
            this.imgIcon.Location = new System.Drawing.Point(404, 16);
            this.imgIcon.Name = "imgIcon";
            this.imgIcon.Size = new System.Drawing.Size(32, 32);
            this.imgIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgIcon.TabIndex = 5;
            this.imgIcon.TabStop = false;
            // 
            // About
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdOk;
            this.ClientSize = new System.Drawing.Size(448, 296);
            this.ControlBox = false;
            this.Controls.Add(this.imgIcon);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblCredits);
            this.Controls.Add(this.imgLeft);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About InZync";
            ((System.ComponentModel.ISupportInitialize)(this.imgLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}
