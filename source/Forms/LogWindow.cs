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

using System;
using System.Windows.Forms;

namespace InZync.Forms
{
    /// <summary>
    /// This window shows the synchronization log.
    /// </summary>
    public class LogWindow : Form
    {
        private Panel pnlBottomLeft;
        private Button cmdClose;
        private Panel pnlTop;
        private RichTextBox txtLog;
        private Panel pnlBottomRight;
        private Button cmdSave;
        private SaveFileDialog saveFileDialog;

        private System.ComponentModel.Container components = null;

        public LogWindow()
        {
            InitializeComponent();
            txtLog.Text = "";
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

        /// <summary>
        /// Add a text to the log.
        /// </summary>
        /// <param name="logText">text to add</param>
        public void Add(string logText)
        {
            txtLog.Text += "[" + DateTime.Now.ToLongTimeString() + "] ";
            txtLog.Text += logText;
            txtLog.Text += "\r\n";
        }

        /// <summary>
        /// The current log is saved to a file.
        /// </summary>
        /// <param name="path">path and filename to save log</param>
        /// <param name="append">true to append text, false to overwrite file</param>
        public void Save(string path, bool append)
        {
            System.IO.StreamWriter stream;
            if (append && System.IO.File.Exists(path))
            {
                stream = System.IO.File.AppendText(path);
            }
            else
            {
                stream = System.IO.File.CreateText(path);
            }
            stream.Write(txtLog.Text);
            stream.Close();
        }

        #region Vom Windows Form-Designer generierter Code
        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlBottomLeft = new System.Windows.Forms.Panel();
            this.cmdSave = new System.Windows.Forms.Button();
            this.pnlBottomRight = new System.Windows.Forms.Panel();
            this.cmdClose = new System.Windows.Forms.Button();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pnlBottomLeft.SuspendLayout();
            this.pnlBottomRight.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottomLeft
            // 
            this.pnlBottomLeft.Controls.Add(this.cmdSave);
            this.pnlBottomLeft.Controls.Add(this.pnlBottomRight);
            this.pnlBottomLeft.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottomLeft.Location = new System.Drawing.Point(0, 234);
            this.pnlBottomLeft.Name = "pnlBottomLeft";
            this.pnlBottomLeft.Size = new System.Drawing.Size(392, 40);
            this.pnlBottomLeft.TabIndex = 1;
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(8, 8);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(80, 24);
            this.cmdSave.TabIndex = 2;
            this.cmdSave.Text = "Save log...";
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // pnlBottomRight
            // 
            this.pnlBottomRight.Controls.Add(this.cmdClose);
            this.pnlBottomRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBottomRight.Location = new System.Drawing.Point(296, 0);
            this.pnlBottomRight.Name = "pnlBottomRight";
            this.pnlBottomRight.Size = new System.Drawing.Size(96, 40);
            this.pnlBottomRight.TabIndex = 1;
            // 
            // cmdClose
            // 
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Location = new System.Drawing.Point(8, 8);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Text = "Close";
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.txtLog);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(392, 234);
            this.pnlTop.TabIndex = 2;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(392, 234);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Filter = "Textfiles (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // LogWindow
            // 
            this.AcceptButton = this.cmdClose;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdClose;
            this.ClientSize = new System.Drawing.Size(392, 274);
            this.ControlBox = false;
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottomLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "LogWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Log";
            this.pnlBottomLeft.ResumeLayout(false);
            this.pnlBottomRight.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Ask for a filename and save log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSave_Click(object sender, System.EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                txtLog.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
            }
        }
    }
}
