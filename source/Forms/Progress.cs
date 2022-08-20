/*
 * InZync
 * 
 * Copyright (C) 2022 by Simon Baer
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

namespace InZync.Forms
{
    /// <summary>
    /// This window shows the progressbar.
    /// </summary>
    public class Progress : System.Windows.Forms.Form
    {
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button cmdCancel;
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Label lblStatus1;
        private System.Windows.Forms.Label lblStatus2;
        private System.Windows.Forms.ProgressBar pgDir;

        public Progress()
        {
            //
            // Erforderlich für die Windows Form-Designerunterstützung
            //
            InitializeComponent();
            Cancel = false;
        }

        /// <summary>
        /// Die verwendeten Ressourcen bereinigen.
        /// </summary>
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

        /// <summary>
        /// Set the total number of directories to sync.
        /// </summary>
        public int TotalDirectories
        {
            get { return pgDir.Maximum; }
            set { pgDir.Maximum = value; }
        }

        /// <summary>
        /// Set the index of the directory which is currently synced.
        /// </summary>
        public int CurrentDirectory
        {
            get { return pgDir.Value; }
            set
            {
                if ((value >= pgDir.Minimum) &&
                    (value <= pgDir.Maximum))
                {
                    pgDir.Value = value;
                }
            }
        }

        /// <summary>
        /// Set the caption of the upper status label.
        /// </summary>
        public string Label1
        {
            set { lblStatus1.Text = value; }
        }

        /// <summary>
        /// Set the caption of the lower status label.
        /// </summary>
        public string Label2
        {
            set { lblStatus2.Text = value; }
        }

        /// <summary>
        /// Set or get a property indicating whether two status bars are displayed.
        /// </summary>
        public bool TwoProgressBars
        {
            get { return lblStatus1.Visible; }
            set 
            {
                lblStatus1.Visible = value;
                pgDir.Visible = value;
                if (value)
                {
                    lblStatus2.Top = 64;
                    progressBar.Top = 80;
                    cmdCancel.Top = 112;
                    this.Height = 208;
                } 
                else 
                {
                    lblStatus2.Top = 20;
                    progressBar.Top = 36;
                    cmdCancel.Top = 78;
                    this.Height = 146;
                }
            }
        }

        /// <summary>
        /// Get or set the progrss bar's maximum value.
        /// </summary>
        public int Maximum
        {
            get { return progressBar.Maximum; }
            set { progressBar.Maximum = value; }
        }

        /// <summary>
        /// Get a flag indicating that the "Cancel" button has been pressed.
        /// </summary>
        public bool Cancel { get; private set; } = false;

        /// <summary>
        /// Get or set the current progress bar value.
        /// </summary>
        public int Value
        {
            get { return progressBar.Value; }
            set
            {
                if ((value >= progressBar.Minimum) &&
                    (value <= progressBar.Maximum))
                {
                    progressBar.Value = value;
                }
            }
        }

        #region Vom Windows Form-Designer generierter Code
        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.pgDir = new System.Windows.Forms.ProgressBar();
            this.lblStatus1 = new System.Windows.Forms.Label();
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 80);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(328, 23);
            this.progressBar.TabIndex = 0;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(144, 112);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(88, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // pgDir
            // 
            this.pgDir.Location = new System.Drawing.Point(16, 32);
            this.pgDir.Name = "pgDir";
            this.pgDir.Size = new System.Drawing.Size(328, 23);
            this.pgDir.TabIndex = 2;
            // 
            // lblStatus1
            // 
            this.lblStatus1.Location = new System.Drawing.Point(16, 16);
            this.lblStatus1.Name = "lblStatus1";
            this.lblStatus1.Size = new System.Drawing.Size(328, 16);
            this.lblStatus1.TabIndex = 3;
            this.lblStatus1.Text = "Directories:";
            // 
            // lblStatus2
            // 
            this.lblStatus2.Location = new System.Drawing.Point(16, 64);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(328, 16);
            this.lblStatus2.TabIndex = 4;
            this.lblStatus2.Text = "source => destination";
            // 
            // Progress
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(360, 176);
            this.ControlBox = false;
            this.Controls.Add(this.lblStatus2);
            this.Controls.Add(this.lblStatus1);
            this.Controls.Add(this.pgDir);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Progress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Progress";
            this.ResumeLayout(false);

        }
        #endregion

        private void cmdCancel_Click(object sender, System.EventArgs e)
        {
            Cancel = true;
        }
    }
}
