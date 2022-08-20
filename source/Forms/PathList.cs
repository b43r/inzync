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
using System.ComponentModel;
using System.Windows.Forms;

namespace InZync.Forms
{
    /// <summary>
    /// Zusammenfassung für PathList.
    /// </summary>
    public class PathList : Form
    {
        private SyncPathList pathList;

        private Panel pnlBottomLeft;
        private Button cmdDestDir;
        private Button cmdSourceDir;
        private Label lblDestination;
        private Label lblSource;
        private TextBox txtDestination;
        private TextBox txtSource;
        private Button cmdAdd;
        private Button cmdDelete;
        private ColumnHeader hdrSource;
        private ColumnHeader hdrDestination;
        private Button cmdOk;
        private Button cmdCancel;
        private Panel pnlBottomRight;
        private ListView lvPaths;
        private FolderBrowserDialog folderBrowser;
        private Button cmdUpdate;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem mnuDelete;
        private IContainer components;

        public PathList()
        {
            //
            // Erforderlich für die Windows Form-Designerunterstützung
            //
            InitializeComponent();
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

        #region Vom Windows Form-Designer generierter Code
        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathList));
            this.pnlBottomLeft = new System.Windows.Forms.Panel();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.pnlBottomRight = new System.Windows.Forms.Panel();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.cmdDestDir = new System.Windows.Forms.Button();
            this.cmdSourceDir = new System.Windows.Forms.Button();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.lvPaths = new System.Windows.Forms.ListView();
            this.hdrSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdrDestination = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlBottomLeft.SuspendLayout();
            this.pnlBottomRight.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottomLeft
            // 
            this.pnlBottomLeft.Controls.Add(this.cmdUpdate);
            this.pnlBottomLeft.Controls.Add(this.pnlBottomRight);
            this.pnlBottomLeft.Controls.Add(this.cmdDelete);
            this.pnlBottomLeft.Controls.Add(this.cmdAdd);
            this.pnlBottomLeft.Controls.Add(this.cmdDestDir);
            this.pnlBottomLeft.Controls.Add(this.cmdSourceDir);
            this.pnlBottomLeft.Controls.Add(this.lblDestination);
            this.pnlBottomLeft.Controls.Add(this.lblSource);
            this.pnlBottomLeft.Controls.Add(this.txtDestination);
            this.pnlBottomLeft.Controls.Add(this.txtSource);
            this.pnlBottomLeft.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottomLeft.Location = new System.Drawing.Point(0, 330);
            this.pnlBottomLeft.Name = "pnlBottomLeft";
            this.pnlBottomLeft.Size = new System.Drawing.Size(704, 84);
            this.pnlBottomLeft.TabIndex = 0;
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Location = new System.Drawing.Point(472, 56);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(56, 20);
            this.cmdUpdate.TabIndex = 22;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // pnlBottomRight
            // 
            this.pnlBottomRight.Controls.Add(this.cmdCancel);
            this.pnlBottomRight.Controls.Add(this.cmdOk);
            this.pnlBottomRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBottomRight.Location = new System.Drawing.Point(534, 0);
            this.pnlBottomRight.Name = "pnlBottomRight";
            this.pnlBottomRight.Size = new System.Drawing.Size(170, 84);
            this.pnlBottomRight.TabIndex = 21;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(86, 52);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(72, 24);
            this.cmdCancel.TabIndex = 20;
            this.cmdCancel.Text = "&Cancel";
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(8, 52);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(72, 24);
            this.cmdOk.TabIndex = 19;
            this.cmdOk.Text = "&Ok";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(472, 32);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(56, 20);
            this.cmdDelete.TabIndex = 18;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // cmdAdd
            // 
            this.cmdAdd.Enabled = false;
            this.cmdAdd.Location = new System.Drawing.Point(472, 8);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(56, 20);
            this.cmdAdd.TabIndex = 17;
            this.cmdAdd.Text = "Add";
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // cmdDestDir
            // 
            this.cmdDestDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDestDir.Location = new System.Drawing.Point(432, 32);
            this.cmdDestDir.Name = "cmdDestDir";
            this.cmdDestDir.Size = new System.Drawing.Size(24, 20);
            this.cmdDestDir.TabIndex = 16;
            this.cmdDestDir.Text = "...";
            this.cmdDestDir.Click += new System.EventHandler(this.cmdDestDir_Click);
            // 
            // cmdSourceDir
            // 
            this.cmdSourceDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSourceDir.Location = new System.Drawing.Point(432, 8);
            this.cmdSourceDir.Name = "cmdSourceDir";
            this.cmdSourceDir.Size = new System.Drawing.Size(24, 20);
            this.cmdSourceDir.TabIndex = 15;
            this.cmdSourceDir.Text = "...";
            this.cmdSourceDir.Click += new System.EventHandler(this.cmdSourceDir_Click);
            // 
            // lblDestination
            // 
            this.lblDestination.Location = new System.Drawing.Point(8, 32);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(112, 23);
            this.lblDestination.TabIndex = 14;
            this.lblDestination.Text = "Destination directory:";
            // 
            // lblSource
            // 
            this.lblSource.Location = new System.Drawing.Point(8, 8);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(100, 23);
            this.lblSource.TabIndex = 13;
            this.lblSource.Text = "Source directory:";
            // 
            // txtDestination
            // 
            this.txtDestination.Location = new System.Drawing.Point(128, 32);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(296, 20);
            this.txtDestination.TabIndex = 12;
            this.txtDestination.TextChanged += new System.EventHandler(this.txtDestination_TextChanged);
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(128, 8);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(296, 20);
            this.txtSource.TabIndex = 11;
            this.txtSource.TextChanged += new System.EventHandler(this.txtSource_TextChanged);
            // 
            // lvPaths
            // 
            this.lvPaths.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdrSource,
            this.hdrDestination});
            this.lvPaths.ContextMenuStrip = this.contextMenuStrip;
            this.lvPaths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPaths.FullRowSelect = true;
            this.lvPaths.GridLines = true;
            this.lvPaths.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPaths.HideSelection = false;
            this.lvPaths.Location = new System.Drawing.Point(0, 0);
            this.lvPaths.MultiSelect = false;
            this.lvPaths.Name = "lvPaths";
            this.lvPaths.Size = new System.Drawing.Size(704, 330);
            this.lvPaths.TabIndex = 2;
            this.lvPaths.UseCompatibleStateImageBehavior = false;
            this.lvPaths.View = System.Windows.Forms.View.Details;
            this.lvPaths.SelectedIndexChanged += new System.EventHandler(this.lvPaths_SelectedIndexChanged);
            // 
            // hdrSource
            // 
            this.hdrSource.Text = "Source path";
            this.hdrSource.Width = 300;
            // 
            // hdrDestination
            // 
            this.hdrDestination.Text = "Destination path";
            this.hdrDestination.Width = 300;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDelete});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(108, 26);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Image = global::InZync.Properties.Resources.DeleteHS;
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(107, 22);
            this.mnuDelete.Text = "&Delete";
            this.mnuDelete.Click += new System.EventHandler(this.mnuDelete_Click);
            // 
            // PathList
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(704, 414);
            this.Controls.Add(this.lvPaths);
            this.Controls.Add(this.pnlBottomLeft);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(712, 448);
            this.Name = "PathList";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Paths to synchronize";
            this.VisibleChanged += new System.EventHandler(this.PathList_VisibleChanged);
            this.Resize += new System.EventHandler(this.PathList_Resize);
            this.pnlBottomLeft.ResumeLayout(false);
            this.pnlBottomLeft.PerformLayout();
            this.pnlBottomRight.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Show the dialog.
        /// </summary>
        /// <param name="owner">owner form</param>
        /// <param name="list">instance of SyncPathList</param>
        /// <returns>DialogResult</returns>
        public DialogResult ShowDialog(IWin32Window owner, SyncPathList list)
        {
            pathList = list;

            // copy items from ArrayList to ListView
            lvPaths.Items.Clear();
            foreach (PathPair pp in pathList)
            {
                ListViewItem newItem = new ListViewItem(new string[] { pp.Source, pp.Destination });
                lvPaths.Items.Add(newItem);
            }

            return base.ShowDialog(owner);
        }

        private void SetControls()
        {
            cmdDelete.Enabled = (lvPaths.SelectedItems.Count > 0);
            cmdAdd.Enabled = (txtSource.Text.Trim() != "") && (txtDestination.Text.Trim() != "");
            cmdUpdate.Enabled = (lvPaths.SelectedItems.Count > 0) &&
                ((lvPaths.SelectedItems[0].SubItems[0].Text.ToLower() != txtSource.Text.Trim().ToLower()) ||
                (lvPaths.SelectedItems[0].SubItems[1].Text.ToLower() != txtDestination.Text.Trim().ToLower()));
        }

        #region Event-handler

        private void PathList_Resize(object sender, EventArgs e)
        {
            lvPaths.Columns[0].Width = (lvPaths.Width / 2) - 2;
            lvPaths.Columns[1].Width = (lvPaths.Width / 2) - 2;
        }

        private void lvPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPaths.SelectedItems.Count > 0)
            {
                txtSource.Text = lvPaths.SelectedItems[0].SubItems[0].Text;
                txtDestination.Text = lvPaths.SelectedItems[0].SubItems[1].Text;
            }
            SetControls();			
        }

        private void PathList_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                lvPaths.Columns[0].Width = (lvPaths.Width / 2) - 2;
                lvPaths.Columns[1].Width = (lvPaths.Width / 2) - 2;
                cmdDelete.Enabled = false;
                cmdUpdate.Enabled = false;
            }
        }

        private void txtSource_TextChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        private void txtDestination_TextChanged(object sender, EventArgs e)
        {
            SetControls();
        }

        /// <summary>
        /// Add a new path to the list, but check if it does not already exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAdd_Click(object sender, EventArgs e)
        {
            string src = txtSource.Text.Trim().ToLower();
            string dest = txtDestination.Text.Trim().ToLower();

            // test if path pair already exists
            foreach (ListViewItem lvi in lvPaths.Items)
            {
                if ((lvi.SubItems[0].Text == src) && (lvi.SubItems[1].Text == dest))
                {
                    lvi.Selected = true;
                    lvPaths.Focus();
                    return;
                }
            }

            ListViewItem newItem = new ListViewItem(new string[] {src, dest});
            lvPaths.Items.Add(newItem);
            txtSource.Text = "";
            txtDestination.Text = "";
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (lvPaths.SelectedItems.Count > 0)
            {
                lvPaths.Items.Remove(lvPaths.SelectedItems[0]);
                SetControls();
            }
        }

        private void cmdSourceDir_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = txtSource.Text;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtSource.Text = folderBrowser.SelectedPath;
            }
        }

        private void cmdDestDir_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = txtDestination.Text;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtDestination.Text = folderBrowser.SelectedPath;
            }
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (lvPaths.Items.Count == 0 && !string.IsNullOrEmpty(txtSource.Text) && !string.IsNullOrEmpty(txtDestination.Text))
            {
                var result = MessageBox.Show($"Would you like to add the following paths to the list of paths to compare?\r\r\r\n{txtSource.Text}\r\n{txtDestination.Text}", "InZync - Paths", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    cmdAdd_Click(sender, e);
                } else if (result == DialogResult.Cancel)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            // copy items from ListView to ArrayList
            pathList.Clear();
            foreach (ListViewItem lvi in lvPaths.Items)
            {
                PathPair pp = new PathPair();
                pp.Source = lvi.SubItems[0].Text;
                if (!pp.Source.EndsWith("\\") && (pp.Source != ""))
                {
                    pp.Source += "\\";
                }
                pp.Destination = lvi.SubItems[1].Text;
                if (!pp.Destination.EndsWith("\\") && (pp.Destination != ""))
                {
                    pp.Destination += "\\";
                }
                pathList.Add(pp);
            }

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Update the selected path pair with the values in the text boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            string src = txtSource.Text.Trim().ToLower();
            if (!src.EndsWith("\\"))
            {
                src += "\\";
            }
            string dest = txtDestination.Text.Trim().ToLower();
            if (!dest.EndsWith("\\"))
            {
                dest += "\\";
            }

            // test if path pair already exists
            foreach (ListViewItem lvi in lvPaths.Items)
            {
                if ((lvi.SubItems[0].Text == src) && (lvi.SubItems[1].Text == dest))
                {
                    lvi.Selected = true;
                    lvPaths.Focus();
                    return;
                }
            }

            // update the selected path pair
            if (lvPaths.SelectedItems.Count > 0)
            {
                lvPaths.SelectedItems[0].SubItems[0].Text = src;
                lvPaths.SelectedItems[0].SubItems[1].Text = dest;
            }
        }

        #endregion

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            if (cmdDelete.Enabled)
            {
                cmdDelete_Click(sender, e);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !cmdDelete.Enabled;
        }
    }
}
