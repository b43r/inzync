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
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace InZync.Forms
{
    /// <summary>
    /// This is the main form of the project.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private IContainer components;

        private SyncPathList syncPathList = new SyncPathList();

        private string sourcePath;
        private string destPath;
        private readonly Settings settings;
        private string jobFile;
        private bool jobChanged;
        private readonly bool silentFlag;
        private readonly SyncList syncList;
        private int sortingColumn = 0;
        private bool sortingOrderAsc = true;
        private readonly int initialContextMenuItemsCount;

        private const string RegistryPath = "Software\\SimonBaer\\InZync";

        // all possible columns
        private enum ColumnNumbers
        {
            UNKNOWN = -1,
            SRCPATH = 0,
            DESTPATH = 1,
            SIZE = 2,
            DATE = 3,
            STATUS = 4,
            ACTION = 5
        }

        private const int NO_OF_COLUMNS = 6;

        // Mapping from logical columns to physical columns in ListView. A value of -1 means the column
        // is currently not visible.
        private int[] colListViewPos = new int[NO_OF_COLUMNS];

        // if this flag is set to true, no files will be copied or deleted.
        private const bool TESTDRIVE = false;

        private string WINDOW_CAPTION = "InZync debug " + Application.ProductVersion;

        //private ListViewSortManager m_ListViewSortManager;

        private const string STATUS_UNKNOWN = "Unknown";
        private const string STATUS_EQUAL = "Equal";
        private const string STATUS_SOURCE_NEWER = "Source newer";
        private const string STATUS_DESTINATION_NEWER = "Destination newer";
        private const string STATUS_SOURCE_MISSING = "Source missing";
        private const string STATUS_DESTINATION_MISSING = "Destination missing";
        private const string STATUS_ACCESS_DENIED = "Access denied";

        private const string ACTION_OVERWRITE_SOURCE = "Overwrite source";
        private const string ACTION_OVERWRITE_DESTINATION = "Overwrite destination";
        private const string ACTION_CREATE_SOURCE = "Create source";
        private const string ACTION_CREATE_DESTINATION = "Create destination";
        private const string ACTION_DELETE_SOURCE = "Delete source";
        private const string ACTION_DELETE_DESTINATION = "Delete destination";
        private const string ACTION_NONE = "None";
        private const string ACTION_IGNORE = "Ignore";
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem mnuActionNone;
        private ToolStripMenuItem mnuActionOwSource;
        private ToolStripMenuItem mnuActionOwDestination;
        private ToolStripMenuItem mnuActionCrSource;
        private ToolStripMenuItem mnuActionCrDestination;
        private ToolStripMenuItem mnuActionDelSource;
        private ToolStripMenuItem mnuActionDelDestination;
        private Panel pnlMain;
        private Panel pnlLower;
        private Button cmdSync;
        private Button cmdCompare;
        private Panel pnlUpper;
        private SyncListView lvSource;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private StatusBar statusBar;
        private StatusBarPanel statusBarPanel;
        private Panel panel1;
        private Button cmdPaths;
        private Button cmdSettings;
        private ContextMenuStrip contextMenuColumns;
        private ToolStripMenuItem mnuContextColSrc;
        private ToolStripMenuItem mnuContextColDest;
        private ToolStripMenuItem mnuContextColSize;
        private ToolStripMenuItem mnuContextColDate;
        private MenuStrip mnuMain;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuOptions;
        private ToolStripMenuItem mnuFilter;
        private ToolStripMenuItem mnuFilterOnlyDifferent;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuHelpAbout;
        private ToolStripMenuItem mnuHelpLink;
        private ToolStripMenuItem mnuOptionsSettings;
        private ToolStripMenuItem mnuOptionsClear;
        private ToolStripMenuItem mnuOptionsColumns;
        private ToolStripMenuItem mnuColumnsSrc;
        private ToolStripMenuItem mnuColumnsDest;
        private ToolStripMenuItem mnuColumnsMod;
        private ToolStripMenuItem mnuColumnsSize;
        private ToolStripMenuItem mnuFileCompare;
        private ToolStripMenuItem mnuFileSync;
        private ToolStripMenuItem mnuFilePaths;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuFileLoad;
        private ToolStripMenuItem mnuFileSave;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mnuFileExit;
        private ToolStripMenuItem mnuFileSaveAs;


        /// <summary>
        /// Constructor of main form. Will be called from the Main() method
        /// after evaluating the command line.
        /// </summary>
        /// <param name="jobFile">jobfile to load</param>
        /// <param name="runFlag">true if the given jobfile should be run</param>
        /// <param name="silentFlag">true if the /silent cmd line switch has been specified</param>
        public MainForm(string jobFile, bool runFlag, bool silentFlag)
        {
            InitializeComponent();

            this.jobFile = jobFile;
            this.jobChanged = false;
            this.silentFlag = silentFlag;

            initialContextMenuItemsCount = contextMenu.Items.Count;

            settings = new Settings();
            cmdSync.Enabled = false;
            mnuFileSync.Enabled = false;
            mnuOptionsClear.Enabled = false;

            syncList = new SyncList();

            if (!string.IsNullOrEmpty(jobFile))
            {
                LoadJob(jobFile);
                if (runFlag)
                {
                    if (settings.RunSilent)
                    {
                        silentFlag = true;
                    }
                }
            }

            if (!silentFlag)
            {
                this.Show();
            }
            
            if (runFlag)
            {
                CommandCompare();
                CommandSync();
                if (settings.TerminateApp)
                {
                    this.Close();
                }
            }

            // read column definition from registry
            ReadColumns(lvSource);
            lvSource.Sort(sortingColumn, sortingOrderAsc ? SortOrder.Ascending : SortOrder.Descending, ColNr2Idx(ColumnNumbers.SIZE));

            SetContextMenu();
        }

        /// <summary>
        /// All columns are removed from the ListView control and then the state of all columns
        /// is read from the registry. Columns currently visible are added to the ListView control again.
        /// </summary>
        /// <param name="lv">ListView control</param>
        private void ReadColumns(ListView lv)
        {
            lvSource.Columns.Clear();
            RebuildColIndex();

            using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath + "\\Columns", false))
            {
                if (reg != null)
                {
                    for (int col = 0; col < NO_OF_COLUMNS; col++)
                    {
                        int displayIndex = (int)reg.GetValue("Pos" + col.ToString(), col);
                        if (displayIndex >= 0)
                        {
                            InsertCol((ColumnNumbers)col, displayIndex);
                        }
                    }

                    // read sort column and order
                    sortingColumn = (int)reg.GetValue("SortColumn", 0);
                    sortingOrderAsc = (int)reg.GetValue("SortAscending", 1) == 1;
                }
                else
                {
                    for (int col = 0; col < NO_OF_COLUMNS; col++)
                    {
                        InsertCol((ColumnNumbers)col, col);
                    }
                    sortingColumn = 0;
                    sortingOrderAsc = true;
                }
            }

            // set menus to checked state if column is visible
            mnuColumnsSrc.Checked = ColVisible(ColumnNumbers.SRCPATH);
            mnuContextColSrc.Checked = mnuColumnsSrc.Checked;
            mnuColumnsDest.Checked = ColVisible(ColumnNumbers.DESTPATH);
            mnuContextColDest.Checked = mnuColumnsDest.Checked;
            mnuColumnsSize.Checked = ColVisible(ColumnNumbers.SIZE);
            mnuContextColSize.Checked = mnuColumnsSize.Checked;
            mnuColumnsMod.Checked = ColVisible(ColumnNumbers.DATE);
            mnuContextColDate.Checked = mnuColumnsMod.Checked;
        }

        /// <summary>
        /// Save the width and position of all currently visible columns to the registry.
        /// </summary>
        /// <param name="lv"></param>
        private void SaveColumns(ListView lv)
        {
            using (var reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath + "\\Columns"))
            {
                for (int col = 0; col < NO_OF_COLUMNS; col++)
                {
                    if (colListViewPos[col] >= 0)
                    {
                        reg.SetValue("Witdh" + col.ToString(), lv.Columns[colListViewPos[col]].Width, Microsoft.Win32.RegistryValueKind.DWord);
                        reg.SetValue("Pos" + col.ToString(), lv.Columns[colListViewPos[col]].DisplayIndex, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                    else
                    {
                        reg.SetValue("Pos" + col.ToString(), -1, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                }

                // save sort columns and order
                reg.SetValue("SortColumn", sortingColumn, Microsoft.Win32.RegistryValueKind.DWord);
                reg.SetValue("SortAscending", sortingOrderAsc ? 1 : 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuActionNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionOwSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionOwDestination = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionCrSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionCrDestination = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionDelSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionDelDestination = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlUpper = new System.Windows.Forms.Panel();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileCompare = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSync = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePaths = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptionsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptionsClear = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptionsColumns = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColumnsSrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColumnsDest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColumnsMod = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColumnsSize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilterOnlyDifferent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpLink = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlLower = new System.Windows.Forms.Panel();
            this.cmdSettings = new System.Windows.Forms.Button();
            this.cmdPaths = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdCompare = new System.Windows.Forms.Button();
            this.cmdSync = new System.Windows.Forms.Button();
            this.contextMenuColumns = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuContextColSrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextColDest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextColSize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextColDate = new System.Windows.Forms.ToolStripMenuItem();
            this.lvSource = new InZync.SyncListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.pnlUpper.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.pnlLower.SuspendLayout();
            this.contextMenuColumns.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "syncjob";
            this.openFileDialog.Filter = "InZync job files (*.syncjob)|*.syncjob|All files (*.*)|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "syncjob";
            this.saveFileDialog.Filter = "InZync job files (*.syncjob)|*.syncjob|All files (*.*)|*.*";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuActionNone,
            this.mnuActionOwSource,
            this.mnuActionOwDestination,
            this.mnuActionCrSource,
            this.mnuActionCrDestination,
            this.mnuActionDelSource,
            this.mnuActionDelDestination});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(188, 158);
            // 
            // mnuActionNone
            // 
            this.mnuActionNone.Image = global::InZync.Properties.Resources.none;
            this.mnuActionNone.Name = "mnuActionNone";
            this.mnuActionNone.Size = new System.Drawing.Size(187, 22);
            this.mnuActionNone.Text = "None";
            this.mnuActionNone.Click += new System.EventHandler(this.mnuActionNone_Click);
            // 
            // mnuActionOwSource
            // 
            this.mnuActionOwSource.Image = global::InZync.Properties.Resources.overwrite_src;
            this.mnuActionOwSource.Name = "mnuActionOwSource";
            this.mnuActionOwSource.Size = new System.Drawing.Size(187, 22);
            this.mnuActionOwSource.Text = "Overwrite source";
            this.mnuActionOwSource.Click += new System.EventHandler(this.mnuActionOwSource_Click);
            // 
            // mnuActionOwDestination
            // 
            this.mnuActionOwDestination.Image = global::InZync.Properties.Resources.overwrite_dst;
            this.mnuActionOwDestination.Name = "mnuActionOwDestination";
            this.mnuActionOwDestination.Size = new System.Drawing.Size(187, 22);
            this.mnuActionOwDestination.Text = "Overwrite destination";
            this.mnuActionOwDestination.Click += new System.EventHandler(this.mnuActionOwDestination_Click);
            // 
            // mnuActionCrSource
            // 
            this.mnuActionCrSource.Image = global::InZync.Properties.Resources.create_src;
            this.mnuActionCrSource.Name = "mnuActionCrSource";
            this.mnuActionCrSource.Size = new System.Drawing.Size(187, 22);
            this.mnuActionCrSource.Text = "Create source";
            this.mnuActionCrSource.Click += new System.EventHandler(this.mnuActionCrSource_Click);
            // 
            // mnuActionCrDestination
            // 
            this.mnuActionCrDestination.Image = global::InZync.Properties.Resources.create_dst;
            this.mnuActionCrDestination.Name = "mnuActionCrDestination";
            this.mnuActionCrDestination.Size = new System.Drawing.Size(187, 22);
            this.mnuActionCrDestination.Text = "Create destination";
            this.mnuActionCrDestination.Click += new System.EventHandler(this.mnuActionCrDestination_Click);
            // 
            // mnuActionDelSource
            // 
            this.mnuActionDelSource.Image = global::InZync.Properties.Resources.del_src;
            this.mnuActionDelSource.Name = "mnuActionDelSource";
            this.mnuActionDelSource.Size = new System.Drawing.Size(187, 22);
            this.mnuActionDelSource.Text = "Delete source";
            this.mnuActionDelSource.Click += new System.EventHandler(this.mnuActionDelSource_Click);
            // 
            // mnuActionDelDestination
            // 
            this.mnuActionDelDestination.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.mnuActionDelDestination.Image = global::InZync.Properties.Resources.del_dst;
            this.mnuActionDelDestination.Name = "mnuActionDelDestination";
            this.mnuActionDelDestination.Size = new System.Drawing.Size(187, 22);
            this.mnuActionDelDestination.Text = "Delete destination";
            this.mnuActionDelDestination.Click += new System.EventHandler(this.mnuActionDelDestination_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 407);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(840, 22);
            this.statusBar.TabIndex = 6;
            // 
            // statusBarPanel
            // 
            this.statusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel.Name = "statusBarPanel";
            this.statusBarPanel.Width = 823;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlUpper);
            this.pnlMain.Controls.Add(this.pnlLower);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(840, 407);
            this.pnlMain.TabIndex = 7;
            // 
            // pnlUpper
            // 
            this.pnlUpper.Controls.Add(this.lvSource);
            this.pnlUpper.Controls.Add(this.mnuMain);
            this.pnlUpper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlUpper.Location = new System.Drawing.Point(0, 0);
            this.pnlUpper.Name = "pnlUpper";
            this.pnlUpper.Size = new System.Drawing.Size(840, 367);
            this.pnlUpper.TabIndex = 6;
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuOptions,
            this.mnuFilter,
            this.mnuHelp});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(840, 24);
            this.mnuMain.TabIndex = 2;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileCompare,
            this.mnuFileSync,
            this.mnuFilePaths,
            this.toolStripMenuItem1,
            this.mnuFileLoad,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.toolStripMenuItem2,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileCompare
            // 
            this.mnuFileCompare.Image = global::InZync.Properties.Resources.PrintPreviewHS;
            this.mnuFileCompare.Name = "mnuFileCompare";
            this.mnuFileCompare.Size = new System.Drawing.Size(252, 22);
            this.mnuFileCompare.Text = "&Compare";
            this.mnuFileCompare.Click += new System.EventHandler(this.mnuFileCompare_Click);
            // 
            // mnuFileSync
            // 
            this.mnuFileSync.Image = global::InZync.Properties.Resources.SychronizeListHS;
            this.mnuFileSync.Name = "mnuFileSync";
            this.mnuFileSync.Size = new System.Drawing.Size(252, 22);
            this.mnuFileSync.Text = "&Sync";
            this.mnuFileSync.Click += new System.EventHandler(this.mnuFileSync_Click);
            // 
            // mnuFilePaths
            // 
            this.mnuFilePaths.Name = "mnuFilePaths";
            this.mnuFilePaths.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuFilePaths.Size = new System.Drawing.Size(252, 22);
            this.mnuFilePaths.Text = "Set &paths to synchronize...";
            this.mnuFilePaths.Click += new System.EventHandler(this.mnuFilePaths_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(249, 6);
            // 
            // mnuFileLoad
            // 
            this.mnuFileLoad.Image = global::InZync.Properties.Resources.openHS;
            this.mnuFileLoad.Name = "mnuFileLoad";
            this.mnuFileLoad.Size = new System.Drawing.Size(252, 22);
            this.mnuFileLoad.Text = "&Load job...";
            this.mnuFileLoad.Click += new System.EventHandler(this.mnuFileLoad_Click);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Image = global::InZync.Properties.Resources.saveHS;
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuFileSave.Size = new System.Drawing.Size(252, 22);
            this.mnuFileSave.Text = "Sa&ve job";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            this.mnuFileSaveAs.Size = new System.Drawing.Size(252, 22);
            this.mnuFileSaveAs.Text = "Save job &as...";
            this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(249, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mnuFileExit.Size = new System.Drawing.Size(252, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOptionsSettings,
            this.mnuOptionsClear,
            this.mnuOptionsColumns});
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.Size = new System.Drawing.Size(61, 20);
            this.mnuOptions.Text = "&Options";
            // 
            // mnuOptionsSettings
            // 
            this.mnuOptionsSettings.Image = global::InZync.Properties.Resources.OptionsHS;
            this.mnuOptionsSettings.Name = "mnuOptionsSettings";
            this.mnuOptionsSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOptionsSettings.Size = new System.Drawing.Size(180, 22);
            this.mnuOptionsSettings.Text = "&Settings...";
            this.mnuOptionsSettings.Click += new System.EventHandler(this.mnuOptionsSettings_Click);
            // 
            // mnuOptionsClear
            // 
            this.mnuOptionsClear.Image = global::InZync.Properties.Resources.DeleteHS;
            this.mnuOptionsClear.Name = "mnuOptionsClear";
            this.mnuOptionsClear.Size = new System.Drawing.Size(180, 22);
            this.mnuOptionsClear.Text = "&Clear list";
            this.mnuOptionsClear.Click += new System.EventHandler(this.mnuOptionsClear_Click);
            // 
            // mnuOptionsColumns
            // 
            this.mnuOptionsColumns.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuColumnsSrc,
            this.mnuColumnsDest,
            this.mnuColumnsMod,
            this.mnuColumnsSize});
            this.mnuOptionsColumns.Name = "mnuOptionsColumns";
            this.mnuOptionsColumns.Size = new System.Drawing.Size(180, 22);
            this.mnuOptionsColumns.Text = "Co&lumns";
            // 
            // mnuColumnsSrc
            // 
            this.mnuColumnsSrc.Name = "mnuColumnsSrc";
            this.mnuColumnsSrc.Size = new System.Drawing.Size(161, 22);
            this.mnuColumnsSrc.Text = "&Source path";
            this.mnuColumnsSrc.Click += new System.EventHandler(this.mnuColumnsSrc_Click);
            // 
            // mnuColumnsDest
            // 
            this.mnuColumnsDest.Name = "mnuColumnsDest";
            this.mnuColumnsDest.Size = new System.Drawing.Size(161, 22);
            this.mnuColumnsDest.Text = "&Destination path";
            this.mnuColumnsDest.Click += new System.EventHandler(this.mnuColumnsDest_Click);
            // 
            // mnuColumnsMod
            // 
            this.mnuColumnsMod.Name = "mnuColumnsMod";
            this.mnuColumnsMod.Size = new System.Drawing.Size(161, 22);
            this.mnuColumnsMod.Text = "&Last modified";
            this.mnuColumnsMod.Click += new System.EventHandler(this.mnuColumnsMod_Click);
            // 
            // mnuColumnsSize
            // 
            this.mnuColumnsSize.Name = "mnuColumnsSize";
            this.mnuColumnsSize.Size = new System.Drawing.Size(161, 22);
            this.mnuColumnsSize.Text = "&Size";
            this.mnuColumnsSize.Click += new System.EventHandler(this.mnuColumnsSize_Click);
            // 
            // mnuFilter
            // 
            this.mnuFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFilterOnlyDifferent});
            this.mnuFilter.Name = "mnuFilter";
            this.mnuFilter.Size = new System.Drawing.Size(45, 20);
            this.mnuFilter.Text = "&Filter";
            // 
            // mnuFilterOnlyDifferent
            // 
            this.mnuFilterOnlyDifferent.Image = global::InZync.Properties.Resources.Filter2HS;
            this.mnuFilterOnlyDifferent.Name = "mnuFilterOnlyDifferent";
            this.mnuFilterOnlyDifferent.ShortcutKeyDisplayString = "";
            this.mnuFilterOnlyDifferent.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mnuFilterOnlyDifferent.Size = new System.Drawing.Size(241, 22);
            this.mnuFilterOnlyDifferent.Text = "Show only &different files";
            this.mnuFilterOnlyDifferent.Click += new System.EventHandler(this.mnuFilterOnlyDifferent_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpLink,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpLink
            // 
            this.mnuHelpLink.Image = global::InZync.Properties.Resources.HomeHS;
            this.mnuHelpLink.Name = "mnuHelpLink";
            this.mnuHelpLink.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuHelpLink.Size = new System.Drawing.Size(196, 22);
            this.mnuHelpLink.Text = "InZync on &GitHub...";
            this.mnuHelpLink.Click += new System.EventHandler(this.mnuHelpLink_Click);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(196, 22);
            this.mnuHelpAbout.Text = "&About...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // pnlLower
            // 
            this.pnlLower.Controls.Add(this.cmdSettings);
            this.pnlLower.Controls.Add(this.cmdPaths);
            this.pnlLower.Controls.Add(this.panel1);
            this.pnlLower.Controls.Add(this.cmdCompare);
            this.pnlLower.Controls.Add(this.cmdSync);
            this.pnlLower.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLower.Location = new System.Drawing.Point(0, 367);
            this.pnlLower.Name = "pnlLower";
            this.pnlLower.Size = new System.Drawing.Size(840, 40);
            this.pnlLower.TabIndex = 5;
            // 
            // cmdSettings
            // 
            this.cmdSettings.Location = new System.Drawing.Point(72, 8);
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.Size = new System.Drawing.Size(72, 24);
            this.cmdSettings.TabIndex = 13;
            this.cmdSettings.Text = "Settings...";
            this.cmdSettings.Click += new System.EventHandler(this.cmdSettings_Click);
            // 
            // cmdPaths
            // 
            this.cmdPaths.Location = new System.Drawing.Point(8, 8);
            this.cmdPaths.Name = "cmdPaths";
            this.cmdPaths.Size = new System.Drawing.Size(56, 24);
            this.cmdPaths.TabIndex = 12;
            this.cmdPaths.Text = "Paths...";
            this.cmdPaths.Click += new System.EventHandler(this.cmdPaths_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(664, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(176, 40);
            this.panel1.TabIndex = 11;
            // 
            // cmdCompare
            // 
            this.cmdCompare.Location = new System.Drawing.Point(160, 8);
            this.cmdCompare.Name = "cmdCompare";
            this.cmdCompare.Size = new System.Drawing.Size(75, 23);
            this.cmdCompare.TabIndex = 3;
            this.cmdCompare.Text = "Compare";
            this.cmdCompare.Click += new System.EventHandler(this.cmdCompare_Click);
            // 
            // cmdSync
            // 
            this.cmdSync.Location = new System.Drawing.Point(240, 8);
            this.cmdSync.Name = "cmdSync";
            this.cmdSync.Size = new System.Drawing.Size(75, 23);
            this.cmdSync.TabIndex = 4;
            this.cmdSync.Text = "Sync";
            this.cmdSync.Click += new System.EventHandler(this.cmdSync_Click);
            // 
            // contextMenuColumns
            // 
            this.contextMenuColumns.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContextColSrc,
            this.mnuContextColDest,
            this.mnuContextColSize,
            this.mnuContextColDate});
            this.contextMenuColumns.Name = "contextMenuColumns";
            this.contextMenuColumns.Size = new System.Drawing.Size(162, 92);
            // 
            // mnuContextColSrc
            // 
            this.mnuContextColSrc.Name = "mnuContextColSrc";
            this.mnuContextColSrc.Size = new System.Drawing.Size(161, 22);
            this.mnuContextColSrc.Text = "Source path";
            this.mnuContextColSrc.Click += new System.EventHandler(this.mnuColumnsSrc_Click);
            // 
            // mnuContextColDest
            // 
            this.mnuContextColDest.Name = "mnuContextColDest";
            this.mnuContextColDest.Size = new System.Drawing.Size(161, 22);
            this.mnuContextColDest.Text = "Destination path";
            this.mnuContextColDest.Click += new System.EventHandler(this.mnuColumnsDest_Click);
            // 
            // mnuContextColSize
            // 
            this.mnuContextColSize.Name = "mnuContextColSize";
            this.mnuContextColSize.Size = new System.Drawing.Size(161, 22);
            this.mnuContextColSize.Text = "Size";
            this.mnuContextColSize.Click += new System.EventHandler(this.mnuColumnsSize_Click);
            // 
            // mnuContextColDate
            // 
            this.mnuContextColDate.Name = "mnuContextColDate";
            this.mnuContextColDate.Size = new System.Drawing.Size(161, 22);
            this.mnuContextColDate.Text = "Last modified";
            this.mnuContextColDate.Click += new System.EventHandler(this.mnuColumnsMod_Click);
            // 
            // lvSource
            // 
            this.lvSource.AllowColumnReorder = true;
            this.lvSource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSource.FullRowSelect = true;
            this.lvSource.GridLines = true;
            this.lvSource.HideSelection = false;
            this.lvSource.Location = new System.Drawing.Point(0, 24);
            this.lvSource.Name = "lvSource";
            this.lvSource.Size = new System.Drawing.Size(840, 343);
            this.lvSource.TabIndex = 1;
            this.lvSource.UseCompatibleStateImageBehavior = false;
            this.lvSource.View = System.Windows.Forms.View.Details;
            this.lvSource.ShowPopupMenu += new InZync.ShowPopupMenuEventHandler(this.lvSource_PopupMenuEvent);
            this.lvSource.HeaderMouseDown += new InZync.HeaderMouseDownEventHandler(this.lvSource_HeaderMouseDown);
            this.lvSource.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvSource_ColumnClick);
            this.lvSource.DoubleClick += new System.EventHandler(this.lvSource_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Source path";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Modified";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Action";
            this.columnHeader5.Width = 116;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(840, 429);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.statusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MinimumSize = new System.Drawing.Size(680, 300);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "InZync";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
            this.Closed += new System.EventHandler(this.frmMain_Closed);
            this.VisibleChanged += new System.EventHandler(this.frmMain_VisibleChanged);
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlUpper.ResumeLayout(false);
            this.pnlUpper.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.pnlLower.ResumeLayout(false);
            this.contextMenuColumns.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main() 
        {
            string jobFile = "";
            bool jobFlag = false;
            bool runFlag = false;
            bool silentFlag = false;
            string[] args = Environment.GetCommandLineArgs();

            Application.EnableVisualStyles();

            // parse command line arguments
            for (int i = 1; i < args.Length; i++)
            {
                string cmd = args[i].ToLower();
                if (cmd.StartsWith("/") || cmd.StartsWith("-"))
                {
                    jobFlag = false;
                    cmd = cmd.Substring(1);
                    switch (cmd)
                    {
                        case "help":
                            MessageBox.Show(null, "InZync command line switches:\r\r" +
                                "/help\t\tShow this message box.\r" +
                                "/job \"filename\"\tLoad a job from given file.\r" +
                                "/run\t\tiIf a job has been loaded using /job, it is executed.\r" +
                                "/silent\t\tDo not display any output while running the job.", "InZync help");
                            return;

                        case "run":
                            runFlag = true;
                            break;

                        case "silent":
                            silentFlag = true;
                            break;

                        case "job":
                            jobFlag = true;
                            jobFile = "-";
                            break;

                        case "setfileextension:1":
                            Settings.SetFileAssociation(true, true);
                            return;

                        case "setfileextension:0":
                            Settings.SetFileAssociation(true, false);
                            return;

                        case "removefileextension":
                            Settings.RemoveFileAssociation(true);
                            return;
                    }
                } 
                else 
                {
                    if (jobFlag)
                    {
                        jobFile = cmd.Replace("\"", "");
                        jobFlag = false;
                    }
                }
            }

            //
            // check command line arguments
            //

            if (jobFile == "-")
            {
                jobFile = "";
                runFlag = false;
                silentFlag = false;
                MessageBox.Show(null, "A filename must follow the /job command line switch.\r\rExample: /job \"SyncMyLaptop.syncjob\"", "InZync - Invalid command line");
            }

            if (!string.IsNullOrEmpty(jobFile))
            {
                if (!File.Exists(jobFile))
                {
                    jobFile = Path.GetFileNameWithoutExtension(jobFile) + ".syncjob";
                    if (!File.Exists(jobFile))
                    {
                        MessageBox.Show(null, $"The given job file does not exist: \"{jobFile}\"", "InZync - Invalid command line");
                        jobFile = "";
                        runFlag = false;
                        silentFlag = false;
                    }
                }
            }

            if (runFlag && string.IsNullOrEmpty(jobFile))
            {
                runFlag = false;
                silentFlag = false;
                MessageBox.Show(null, "The /run command line switch can only be used if a job file is specified using the /job parameter.", "InZync - Invalid command line");
            }

            if ((silentFlag) && (!runFlag))
            {
                MessageBox.Show(null, "The /silent command line switch can only be used together with the /run switch.", "InZync - Invalid command line");
                silentFlag = false;
            }

            MainForm frm = new MainForm(jobFile, runFlag, silentFlag);
            if (frm.Visible)
            {
                Application.Run(frm);
            }
        }

        /// <summary>
        /// Method called for file extension filtering while comparing files.
        /// Must return true if files with the given extension should be compared or false if not.
        /// </summary>
        /// <param name="ext">file extension including leading dot</param>
        /// <returns>true if file should be processed</returns>
        private bool ProcessExtension(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                // remove leading dot and convert to lower case
                ext = ext.Substring(1).ToLower();
            }
            if ((settings.Extensions.Length == 1) && (settings.Extensions[0] == "*"))
            {
                // "*" = process everything, check excluded extensions now
                foreach (string e in settings.ExcludedExtensions)
                {
                    if (!string.IsNullOrEmpty(e))
                    {
                        if (Wildcard(e, ext))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                foreach (string e in settings.Extensions)
                {
                    if (Wildcard(e, ext))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a given string matches a given wildcard.
        /// Code based on http://www.codeproject.com/string/wildcmp.asp
        /// </summary>
        /// <param name="wild">wildcard</param>
        /// <param name="str">string to check</param>
        /// <returns>true if string matches wildcard</returns>
        private bool Wildcard(string wild, string str) 
        {
            int cp = 0;
            int mp = 0;
            int i = 0;
            int j = 0;

            while ((i < str.Length) && (j < wild.Length) && (wild[j] != '*'))
            {
                if ((wild[j] != str[i]) && (wild[j] != '?')) 
                {
                    return false;
                }
                i++;
                j++;
            }
            
            while (i < str.Length) 
            {
                if ((j < wild.Length) && (wild[j] == '*'))
                {
                    if ((j++) >= wild.Length) 
                    {
                        return true;
                    }
                    mp = j;
                    cp = i + 1;
                } 
                else if ((j < wild.Length) && ((wild[j] == str[i]) || (wild[j] == '?'))) 
                {
                    j++;
                    i++;
                } 
                else 
                {
                    j = mp;
                    i = cp++;
                }
            }
            
            while ((j < wild.Length) && (wild[j] == '*'))
            {
                j++;
            }
            return (j >= wild.Length);
        }
        
        /// <summary>
        ///  The ListView control is filled with the data in the SyncList.
        /// </summary>
        private void FillGrid()
        {
            int fileCount = 0;
            int diffCount = 0;
            long diffSize = 0;

            statusBar.Panels[0].Text = " Please wait...";
            Application.DoEvents();

            lvSource.Items.Clear();
            lvSource.SuspendLayout();
            this.lvSource.ListViewItemSorter = null;

            for (int i = 0; i < syncList.Count; i++)
            {
                SyncItem si = syncList[i];

                if (!string.IsNullOrEmpty(si.Size))
                {
                    // otherwise its a directory
                    fileCount++;
                    if (si.Status != STATUS_EQUAL)
                    {
                        diffCount++;
                        diffSize += si.NumSize;
                    }
                }

                if ((!settings.ShowOnlyDifferentFiles) || (si.Status != STATUS_EQUAL))
                {
                    ListViewItem item = CreateListViewItem(si);
                    item.Tag = i;
                    this.lvSource.Items.Add(item);
                }
            }
            lvSource.ResumeLayout();

            lvSource.Sort(sortingColumn, sortingOrderAsc ? SortOrder.Ascending : SortOrder.Descending, ColNr2Idx(ColumnNumbers.SIZE));
            string sizeStr = FormatSize(diffSize);
            statusBar.Panels[0].Text = string.Format(" {0:#,0} files compared, {1:#,0} different ({2})", fileCount, diffCount, sizeStr == string.Empty ? "0 Bytes" : sizeStr);
        }

        /// <summary>
        /// Create a ListViewItem from the given SyncItem.
        /// </summary>
        /// <param name="si">SyncItem</param>
        /// <returns>ListViewItem representing the SyncItem</returns>
        private ListViewItem CreateListViewItem(SyncItem si)
        {
            // sort all visible columns by their position and get the value to display
            SortedList<int, string> sl = new SortedList<int, string>();
            for (int col = 0; col < NO_OF_COLUMNS; col++)
            {
                if (colListViewPos[col] >= 0)
                {
                    string data = "?";
                    switch ((ColumnNumbers)col)
                    {
                        case ColumnNumbers.SRCPATH:
                            data = si.SourcePath + si.RelPath;
                            break;
                        case ColumnNumbers.DESTPATH:
                            data = si.DestPath + si.RelPath;
                            break;
                        case ColumnNumbers.SIZE:
                            data = FormatSize(si.NumSize);
                            break;
                        case ColumnNumbers.DATE:
                            data = si.Date;
                            break;
                        case ColumnNumbers.STATUS:
                            data = si.Status;
                            break;
                        case ColumnNumbers.ACTION:
                            data = si.Action;
                            break;
                    }
                    sl.Add(colListViewPos[col], data);
                }
            }

            // add items
            ListViewItem item = null;
            foreach (string obj in sl.Values)
            {
                if (item == null)
                {
                    item = new ListViewItem(obj);
                } 
                else 
                {
                    item.SubItems.Add(obj);
                }
            }
            item.ForeColor = si.Color;
            return item;
        }

        /// <summary>
        /// All files/subdirectories in the destination directory are parsed.
        /// </summary>
        /// <param name="frm">reference to the progress bar form</param>
        /// <param name="dir">directory to parse</param>
        private void ParseDestinationDir(Forms.Progress frm, string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            Application.DoEvents();
            if (frm.Cancel)
                return;

            // add empty directory
            if (((!IsEmptyDirectory(dirInfo)) ||
                (settings.DirectoriesLikeFiles)) && (dir != destPath))
            {
                if ((((dirInfo.Attributes & FileAttributes.Hidden) == 0) || (settings.ProcessHiddenFiles)) &&
                    (((dirInfo.Attributes & FileAttributes.System) == 0) || (settings.ProcessSystemFiles)))
                {
                    string relPath = dirInfo.FullName.Substring(destPath.Length);
                    if (!Directory.Exists(sourcePath + relPath))
                    {
                        SyncItem si = new SyncItem();
                        si.SourcePath = sourcePath;
                        si.DestPath = destPath;
                        si.RelPath = relPath;
                        si.Size = "";
                        si.NumSize = 0;
                        si.Date = dirInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
                        si.Status = STATUS_SOURCE_MISSING;
                        switch (settings.SourceMissing)
                        {
                            case 1:
                                si.Action = ACTION_CREATE_SOURCE;
                                si.Color = Color.Blue;
                                break;
                            case 2:
                                si.Action = ACTION_DELETE_DESTINATION;
                                si.Color = Color.Blue;
                                break;
                            case 3:
                                si.Action = ACTION_NONE;
                                si.Color = Color.Black;
                                break;
                        }
                        si.Processed = true;
                        syncList.Add(si);
                        if (TESTDRIVE)
                        {
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                }
            }

            // parse all subdirectories
            if (settings.SubDirectories)
            {
                try
                {
                    foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
                    {
                        if ((((subDirInfo.Attributes & FileAttributes.Hidden) == 0) || (settings.ProcessHiddenFiles)) &&
                            (((subDirInfo.Attributes & FileAttributes.System) == 0) || (settings.ProcessSystemFiles)))
                        {
                            ParseDestinationDir(frm, subDirInfo.FullName);
                        }
                    }
                }
                catch (System.UnauthorizedAccessException)
                {
                    string relPath = dirInfo.FullName.Substring(destPath.Length);

                    SyncItem si = new SyncItem();
                    si.SourcePath = sourcePath;
                    si.DestPath = destPath;
                    si.RelPath = relPath;
                    si.Size = "";
                    si.NumSize = 0;
                    si.Date = dirInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
                    si.Status = STATUS_ACCESS_DENIED;
                    si.Action = ACTION_IGNORE;
                    si.Color = Color.Gray;
                    si.Processed = true;
                    syncList.Add(si);
                    if (TESTDRIVE)
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }

            // parse all files
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.FullName.ToLower() != settings.LogFile.ToLower())
                {
                    if (ProcessExtension(fileInfo.Extension))
                    {
                        if ((((fileInfo.Attributes & FileAttributes.Hidden) == 0) || (settings.ProcessHiddenFiles)) &&
                            (((fileInfo.Attributes & FileAttributes.System) == 0) || (settings.ProcessSystemFiles)))
                        {
                            frm.Value++;
                            string relPath = fileInfo.FullName.Substring(destPath.Length);
                            if (!File.Exists(sourcePath + relPath))
                            {
                                SyncItem si = new SyncItem();
                                si.SourcePath = sourcePath;
                                si.DestPath = destPath;
                                si.RelPath = relPath;
                                si.Size = fileInfo.Length.ToString();
                                si.NumSize = fileInfo.Length;
                                si.Date = fileInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
                                si.Status = STATUS_SOURCE_MISSING;
                                switch (settings.SourceMissing)
                                {
                                    case 1:
                                        si.Action = ACTION_CREATE_SOURCE;
                                        si.Color = Color.Red;
                                        break;
                                    case 2:
                                        si.Action = ACTION_DELETE_DESTINATION;
                                        si.Color = Color.Red;
                                        break;
                                    case 3:
                                        si.Action = ACTION_NONE;
                                        si.Color = Color.Black;
                                        break;
                                }
                                si.Processed = true;
                                syncList.Add(si);
                                if (TESTDRIVE)
                                {
                                    System.Threading.Thread.Sleep(50);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// All files/subdirectories in the source directory are parsed.
        /// </summary>
        /// <param name="dir">directory to parse</param>
        /// <returns>number of files found</returns>
        private int ParseSourceDir(string dir)
        {
            int fileCounter = 0;
            int latestDirIdx = -1;

            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            Application.DoEvents();

            // add empty directory
            if (dir != sourcePath)
            {
                if ((!IsEmptyDirectory(dirInfo)) ||
                    (settings.DirectoriesLikeFiles))
                {
                    if ((((dirInfo.Attributes & FileAttributes.Hidden) == 0) || (settings.ProcessHiddenFiles)) &&
                        (((dirInfo.Attributes & FileAttributes.System) == 0) || (settings.ProcessSystemFiles)))
                    {
                        AddDirectory(dirInfo, false);
                        latestDirIdx = syncList.Count - 1;
                    }
                }
            }

            // parse all subdirectories
            if (settings.SubDirectories)
            {
                try
                {
                    foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
                    {
                        if ((((subDirInfo.Attributes & FileAttributes.Hidden) == 0) || (settings.ProcessHiddenFiles)) &&
                            (((subDirInfo.Attributes & FileAttributes.System) == 0) || (settings.ProcessSystemFiles)))
                        {
                            fileCounter += ParseSourceDir(subDirInfo.FullName);
                        }
                    }
                }
                catch (System.UnauthorizedAccessException)
                {
                    AddDirectory(dirInfo, true);
                }
            }

            // parse all files
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.FullName.ToLower() != settings.LogFile.ToLower())
                {
                    if (ProcessExtension(fileInfo.Extension))
                    {
                        if ((((fileInfo.Attributes & FileAttributes.Hidden) == 0) || (settings.ProcessHiddenFiles)) &&
                            (((fileInfo.Attributes & FileAttributes.System) == 0) || (settings.ProcessSystemFiles)) &&
                            (((fileInfo.Attributes & FileAttributes.Archive) != 0) || (!settings.BackupMode)))
                        {
                            AddFile(fileInfo);
                            fileCounter++;
                        }
                    }
                }
            }

            // remove directory if no subdirectory contains any files
            if ((fileCounter == 0) && (latestDirIdx != -1) && (!settings.DirectoriesLikeFiles))
            {
                syncList.RemoveAt(latestDirIdx);
            }

            return fileCounter;
        }

        /// <summary>
        /// Test if a directory is empty. "Empty" means it does not contain any file
        /// but it may contain empty subdirectories.
        /// </summary>
        /// <param name="dirInfo">DirectoryInfo of directory to check</param>
        /// <returns>true if empty</returns>
        private bool IsEmptyDirectory(DirectoryInfo dirInfo)
        {
            if (dirInfo.GetFiles().Length > 0)
            {
                return false;
            }
            foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
            {
                if (!IsEmptyDirectory(subDirInfo))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Add a file to the syncList.
        /// </summary>
        /// <param name="fileInfo">FileInfo of file to add</param>
        private void AddFile(FileInfo fileInfo) 
        {
            string relPath = fileInfo.FullName.Substring(sourcePath.Length);
            SyncItem si = new SyncItem();
            si.SourcePath = sourcePath;
            si.DestPath = destPath;
            si.RelPath = relPath;
            si.Size = fileInfo.Length.ToString();
            si.NumSize = fileInfo.Length;
            si.Date = fileInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
            si.Status = STATUS_UNKNOWN;
            si.Action = "";
            si.Color = Color.Black;
            si.Processed = false;
            syncList.Add(si);
        }

        /// <summary>
        /// Add a directory to the syncList.
        /// </summary>
        /// <param name="dirInfo">DirectoryInfo of directory to add</param>
        /// <param name="failed">true if the directory could not be accessed</param>
        private void AddDirectory(DirectoryInfo dirInfo, bool failed)
        {
            string relPath = dirInfo.FullName.Substring(sourcePath.Length);
            SyncItem si = new SyncItem();
            si.SourcePath = sourcePath;
            si.DestPath = destPath;
            si.RelPath = relPath;
            si.Size = "";
            si.NumSize = 0;
            si.Date = dirInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
            if (failed)
            {
                si.Status = STATUS_ACCESS_DENIED;
                si.Action = ACTION_IGNORE;
                si.Color = Color.Gray;
            } 
            else 
            {
                si.Status = STATUS_UNKNOWN;
                si.Action = "";
                si.Color = Color.Black;
            }
            si.Processed = false;
            syncList.Add(si);
        }

        /// <summary>
        /// Compares all files in syncList.
        /// </summary>
        /// <param name="frm">reference to progress bar form</param>
        private void Compare(Forms.Progress frm)
        {
            int counter = 0;
            for (int i = 0; i < syncList.Count; i++)
            {
                if (TESTDRIVE)
                {
                    System.Threading.Thread.Sleep(50);
                }

                if (frm.Cancel)
                {
                    break;
                }

                SyncItem item = syncList[i];

                frm.Value = ++counter;
                if ((item.Status != STATUS_ACCESS_DENIED) && (!item.Processed))
                {
                    string destFile = item.DestPath + item.RelPath;
                    string sourceFile = item.SourcePath + item.RelPath;
                    bool exists;
                    Color changedCol;
                    bool isDirectory;

                    item.Processed = true;

                    if (string.IsNullOrEmpty(item.Size))
                    {
                        // it's a directory!
                        exists = Directory.Exists(destFile);
                        changedCol = Color.Blue;
                        isDirectory = true;
                    }
                    else
                    {
                        // it's a file
                        exists = File.Exists(destFile);
                        changedCol = Color.Red;
                        isDirectory = false;
                    }

                    if (exists)
                    {
                        if (isDirectory)
                        {
                            item.Status = STATUS_EQUAL;
                            item.Action = ACTION_NONE;
                        }
                        else 
                        {
                            FileInfo destInfo = new FileInfo(destFile);
                            FileInfo sourceInfo = new FileInfo(sourceFile);
                            if (sourceInfo.LastWriteTime > destInfo.LastWriteTime)
                            {
                                item.Status = STATUS_SOURCE_NEWER;
                                switch (settings.SourceNewer)
                                {
                                    case 1:
                                        item.Action = ACTION_OVERWRITE_DESTINATION;
                                        item.Color = changedCol;
                                        break;
                                    case 2:
                                        item.Action = ACTION_OVERWRITE_SOURCE;
                                        item.Color = changedCol;
                                        break;
                                    case 3:
                                        item.Action = ACTION_NONE;
                                        break;
                                }
                            }
                            else if (sourceInfo.LastWriteTime < destInfo.LastWriteTime)
                            {
                                item.Status = STATUS_DESTINATION_NEWER;
                                switch (settings.DestNewer)
                                {
                                    case 1:
                                        item.Action = ACTION_OVERWRITE_SOURCE;
                                        item.Color = changedCol;
                                        break;
                                    case 2:
                                        item.Action = ACTION_OVERWRITE_DESTINATION;
                                        item.Color = changedCol;
                                        break;
                                    case 3:
                                        item.Action = ACTION_NONE;
                                        break;
                                }
                            }
                            else 
                            {
                                item.Status = STATUS_EQUAL;
                                item.Action = ACTION_NONE;
                            }
                        }
                    }
                    else 
                    {
                        item.Status = STATUS_DESTINATION_MISSING;
                        switch (settings.DestMissing)
                        {
                            case 1:
                                item.Action = ACTION_CREATE_DESTINATION;
                                item.Color = changedCol;
                                break;
                            case 2:
                                item.Action = ACTION_DELETE_SOURCE;
                                item.Color = changedCol;
                                break;
                            case 3:
                                item.Action = ACTION_NONE;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is called before the comparing starts and checks if all paths exist.
        /// </summary>
        /// <returns>true if all paths are ok</returns>
        private bool CheckAllPaths()
        {
            foreach (PathPair pp in syncPathList)
            {
                if (!Directory.Exists(pp.Source))
                {
                    MessageBox.Show(this, "Source directory does not exist: " + pp.Source, this.Text);
                    return false;
                }
                if  (!Directory.Exists(pp.Destination))
                {
                    MessageBox.Show(this, "Destination directory does not exist: " + pp.Destination, this.Text);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Starts the comparing.
        /// </summary>
        private void CommandCompare()
        {
            if (!CheckAllPaths())
            {
                return;
            }

            EnableAllCommands(false);
            this.Cursor = Cursors.WaitCursor;
            syncList.Clear();

            Forms.Progress frm = new Forms.Progress();
            frm.Owner = this;
            frm.Left = this.Left + (this.Width - frm.Width) / 2;
            frm.Top = this.Top + (this.Height - frm.Height) / 2;
            frm.Text = "Comparing directories...";
            frm.Label1 = "Directories: 1 of " + syncPathList.Count.ToString();
            frm.TotalDirectories = syncPathList.Count;
            statusBar.Panels[0].Text = " " + frm.Text;
            if (!silentFlag)
            {
                frm.Show();
            }

            // iterate over all paths...
            foreach (PathPair pp in syncPathList)
            {
                sourcePath = pp.Source;
                destPath = pp.Destination;

                frm.Label2 = "Comparing " + sourcePath + "...";

                // fill directories and files from source into SyncList
                ParseSourceDir(sourcePath);
                if (frm.Cancel)
                {
                    break;
                }

                frm.Maximum = syncList.Count * 2;

                // compare objects in SyncList with destination
                Compare(frm);
                if (frm.Cancel)
                {
                    break;
                }

                ParseDestinationDir(frm, destPath);
                if (frm.Cancel)
                {
                    break;
                }

                frm.CurrentDirectory++;
            }

            frm.Value = frm.Maximum;
            System.Threading.Thread.Sleep(100);
            frm.Close();

            if (frm.Cancel)
            {
                this.Cursor = Cursors.Default;
                syncList.Clear();
                statusBar.Panels[0].Text = " Cancelled.";
            } 
            else 
            {
                FillGrid();
                this.Cursor = Cursors.Default;
            }

            EnableAllCommands(true);
        }

        /// <summary>
        /// Actually copy files.
        /// </summary>
        private void CommandSync()
        {
            // statistic...
            int createCount = 0;
            int copyCount = 0;
            int deleteCount = 0;

            EnableAllCommands(false);

            // create log-window
            Forms.LogWindow log = new Forms.LogWindow();
            log.Add("Synchronisation started.");

            // show progressbar
            Forms.Progress frm = new Forms.Progress();
            frm.Owner = this;
            frm.Left = this.Left + (this.Width - frm.Width) / 2;
            frm.Top = this.Top + (this.Height - frm.Height) / 2;
            frm.Text = "Synchronizing...";
            statusBar.Panels[0].Text = " " + frm.Text;
            frm.Maximum = syncList.Count * 2;
            frm.Label2 = "Synchronizing " + syncList.Count.ToString() + " files...";
            frm.TwoProgressBars = false;
            if (!silentFlag)
            {
                frm.Show();
            }

            /// Each row in the listview represents a file. Traverse the list top-down and
            /// copy/create all necessary files/directories. Then traverse the list again
            /// bottom-up and delete all necessary files/directories.
            
            // 1st pass: top-down
            for (int i = 0; i < syncList.Count; i++)
            {
                if (frm.Cancel)
                {
                    break;
                }

                SyncItem item = syncList[i];

                string sourceFile = item.SourcePath + item.RelPath;
                string destFile = item.DestPath + item.RelPath;
                bool isDirectory = string.IsNullOrEmpty(item.Size);

                if (TESTDRIVE)
                {
                    System.Threading.Thread.Sleep(50);
                } 
                else 
                {
                    try
                    {
                        switch (item.Action)
                        {
                            case ACTION_OVERWRITE_SOURCE:
                                CheckReadOnlyAttribute(sourceFile);
                                File.Copy(destFile, sourceFile, true);
                                log.Add($"File \"{destFile}\" copied to \"{sourceFile}\".");
                                copyCount++;
                                break;

                            case ACTION_OVERWRITE_DESTINATION:
                                CheckReadOnlyAttribute(destFile);
                                File.Copy(sourceFile, destFile, true);
                                log.Add($"File \"{sourceFile}\" copied to \"{destFile}\".");
                                copyCount++;
                                break;

                            case ACTION_CREATE_SOURCE:
                                if (isDirectory)
                                {
                                    Directory.CreateDirectory(sourceFile);
                                    log.Add($"Directory \"{sourceFile}\" created.");
                                } 
                                else 
                                {
                                    File.Copy(destFile, sourceFile, true);
                                    log.Add($"File \"{destFile}\" copied to \"{sourceFile}\".");
                                    createCount++;
                                }
                                break;

                            case ACTION_CREATE_DESTINATION:
                                if (isDirectory)
                                {
                                    Directory.CreateDirectory(destFile);
                                    log.Add($"Directory \"{destFile}\" created.");
                                } 
                                else 
                                {
                                    File.Copy(sourceFile, destFile, true);
                                    log.Add($"File \"{sourceFile}\" copied to \"{destFile}\".");
                                    createCount++;
                                }
                                break;
                        }

                        // remove archiv attribute of source file in backup-mode
                        if (settings.BackupMode)
                        {
                            FileAttributes fa = File.GetAttributes(sourceFile);
                            if ((fa & FileAttributes.Archive) != 0)
                            {
                                File.SetAttributes(sourceFile, fa & ~FileAttributes.Archive);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add($"Exception: {ex.Message}");
                        log.Add($"  *** source file: \"{sourceFile}\"");
                        log.Add($"  *** destination file: \"{destFile}\"");
                        log.Add($"  *** action: {item.Action}");
                    }
                }

                frm.Value++;
                Application.DoEvents();
            }

            // 2nd pass: bottom-up
            for (int i = syncList.Count - 1; i >= 0; i--)
            {
                if (frm.Cancel)
                {
                    break;
                }

                SyncItem item = syncList[i];

                string sourceFile = item.SourcePath + item.RelPath;
                string destFile = item.DestPath + item.RelPath;
                bool isDirectory = string.IsNullOrEmpty(item.Size);

                if (TESTDRIVE)
                {
                    System.Threading.Thread.Sleep(50);
                } 
                else 
                {
                    try
                    {
                        switch (item.Action)
                        {
                            case ACTION_DELETE_SOURCE:
                                CheckReadOnlyAttribute(sourceFile);
                                if (isDirectory)
                                {
                                    Directory.Delete(sourceFile, false);
                                    log.Add($"Directory \"{sourceFile}\" deleted.");
                                } 
                                else 
                                {
                                    File.Delete(sourceFile);
                                    log.Add($"File \"{sourceFile}\" deleted.");
                                    deleteCount++;
                                }
                                break;

                            case ACTION_DELETE_DESTINATION:
                                CheckReadOnlyAttribute(destFile);
                                if (isDirectory)
                                {
                                    Directory.Delete(destFile, false);
                                    log.Add($"Directory \"{destFile}\" deleted.");
                                }
                                else
                                {
                                    File.Delete(destFile);
                                    log.Add($"File \"{destFile}\" deleted.");
                                    deleteCount++;
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add($"Exception: {ex.Message}");
                        log.Add($"  *** source file: \"{sourceFile}\"");
                        log.Add($"  *** destination file: \"{destFile}\"");
                        log.Add($"  *** action: {item.Action}");
                    }
                }

                frm.Value++;
                Application.DoEvents();
                if (frm.Cancel)
                {
                    break;
                }
            }

            frm.Close();

            if (frm.Cancel)
            {
                log.Add("Synchronisation cancelled.");
                statusBar.Panels[0].Text = " Cancelled.";
            } 
            else 
            {
                log.Add("Synchronisation finished.");
                statusBar.Panels[0].Text = $" {copyCount} files overwritten, {createCount} files created and {deleteCount} files deleted.";
            }

            EnableAllCommands(true);

            if (settings.SaveLog)
            {
                log.Save(settings.LogFile, settings.AppendLog);
            }
            if ((settings.ShowLogWindow) && (!silentFlag))
            {
                log.ShowDialog(this);
            }
        }
        
        /// <summary>
        /// If the read-only attribute of the given file is set it is removed.
        /// </summary>
        /// <param name="file"></param>
        private void CheckReadOnlyAttribute(string file)
        {
            FileAttributes fa = File.GetAttributes(file);
            if ((fa & FileAttributes.ReadOnly) != 0)
            {
                File.SetAttributes(file, fa & ~FileAttributes.ReadOnly);
            }
        }	

        #region All event-handler

        #region Event-handler for buttons

        private void cmdPaths_Click(object sender, System.EventArgs e)
        {
            CommandSetPaths();
        }

        private void cmdSettings_Click(object sender, System.EventArgs e)
        {
            CommandSettings();
        }

        private void cmdCompare_Click(object sender, System.EventArgs e)
        {
            CommandCompare();
        }

        private void cmdSync_Click(object sender, System.EventArgs e)
        {
            CommandSync();
        }

        #endregion

        #region Event-handler for menus

        //
        // Menu "File"
        //

        private void mnuFileCompare_Click(object sender, EventArgs e)
        {
            CommandCompare();
        }

        private void mnuFileSync_Click(object sender, EventArgs e)
        {
            CommandSync();
        }

        private void mnuFilePaths_Click(object sender, EventArgs e)
        {
            CommandSetPaths();
        }

        private void mnuFileLoad_Click(object sender, EventArgs e)
        {
            // test if a currently loaded job has been changed and should be saved
            if (!TestForUnsavedJob())
            {
                return;
            }

            // get initial path from registry
            openFileDialog.InitialDirectory = GetInitialPath();
            openFileDialog.FileName = "";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // save selected path to registry
                UpdateInitialPath(Path.GetDirectoryName(openFileDialog.FileName));
                LoadJob(openFileDialog.FileName);
            }
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(jobFile) && File.Exists(jobFile))
            {
                SaveJob(jobFile, false);
            }
            else
            {
                mnuFileSaveAs_Click(sender, e);
            }
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            // get initial path from registry
            saveFileDialog.InitialDirectory = GetInitialPath();
            saveFileDialog.FileName = jobFile;

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // save selected path to registry
                UpdateInitialPath(Path.GetDirectoryName(saveFileDialog.FileName));
                SaveJob(saveFileDialog.FileName, false);
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //
        // Menu "Options"
        //

        private void mnuOptionsSettings_Click(object sender, System.EventArgs e)
        {
            CommandSettings();
        }

        private void mnuOptionsClear_Click(object sender, EventArgs e)
        {
            lvSource.Items.Clear();
            syncList.Clear();
            cmdSync.Enabled = false;
            mnuFileSync.Enabled = false;
            mnuOptionsClear.Enabled = false;
        }

        //
        // Menu "Filter"
        //

        private void mnuFilterOnlyDifferent_Click(object sender, System.EventArgs e)
        {
            mnuFilterOnlyDifferent.Checked = !mnuFilterOnlyDifferent.Checked;
            settings.ShowOnlyDifferentFiles = mnuFilterOnlyDifferent.Checked;
            jobChanged = true;
            MakeWindowCaption();
            FillGrid();
        }

        //
        // Menu "Help"
        //

        private void mnuHelpLink_Click(object sender, System.EventArgs e)
        {
            string commandText = "https://github.com/b43r/inzync";
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = commandText;
            proc.StartInfo.Verb = "open";
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }

        private void mnuHelpAbout_Click(object sender, System.EventArgs e)
        {
            Forms.About frm = new Forms.About();
            frm.ShowDialog();
        }

        #endregion

        #region Event handler for switching columns on/off

        private void mnuColumnsSrc_Click(object sender, System.EventArgs e)
        {
            mnuColumnsSrc.Checked = !mnuColumnsSrc.Checked;
            mnuContextColSrc.Checked = mnuColumnsSrc.Checked;
            if (mnuColumnsSrc.Checked)
            {
                InsertCol(ColumnNumbers.SRCPATH);
            }
            else
            {
                RemoveCol(ColumnNumbers.SRCPATH);
            }
            FillGrid();
        }

        private void mnuColumnsDest_Click(object sender, System.EventArgs e)
        {
            mnuColumnsDest.Checked = !mnuColumnsDest.Checked;
            mnuContextColDest.Checked = mnuColumnsDest.Checked;
            if (mnuColumnsDest.Checked)
            {
                InsertCol(ColumnNumbers.DESTPATH);
            }
            else
            {
                RemoveCol(ColumnNumbers.DESTPATH);
            }
            FillGrid();
        }

        private void mnuColumnsSize_Click(object sender, System.EventArgs e)
        {
            mnuColumnsSize.Checked = !mnuColumnsSize.Checked;
            mnuContextColSize.Checked = mnuColumnsSize.Checked;
            if (mnuColumnsSize.Checked)
            {
                InsertCol(ColumnNumbers.SIZE);
            }
            else
            {
                RemoveCol(ColumnNumbers.SIZE);
            }
            FillGrid();
        }

        private void mnuColumnsMod_Click(object sender, System.EventArgs e)
        {
            mnuColumnsMod.Checked = !mnuColumnsMod.Checked;
            mnuContextColDate.Checked = mnuColumnsMod.Checked;
            if (mnuColumnsMod.Checked)
            {
                InsertCol(ColumnNumbers.DATE);
            }
            else
            {
                RemoveCol(ColumnNumbers.DATE);
            }
            FillGrid();
        }

        #endregion

        #region Event-handler for main form

        private void frmMain_Closed(object sender, System.EventArgs e)
        {
            using (var reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath + "\\Window", true))
            {
                if (reg != null)
                {
                    reg.SetValue("Left", this.Left, Microsoft.Win32.RegistryValueKind.DWord);
                    reg.SetValue("Top", this.Top, Microsoft.Win32.RegistryValueKind.DWord);
                    reg.SetValue("Width", this.Width, Microsoft.Win32.RegistryValueKind.DWord);
                    reg.SetValue("Height", this.Height, Microsoft.Win32.RegistryValueKind.DWord);
                }
            }

            // save column widths to registry
            SaveColumns(lvSource);

            SaveJob();
        }

        private void frmMain_VisibleChanged(object sender, System.EventArgs e)
        {
            if (this.Visible == true)
            {
                if (string.IsNullOrEmpty(jobFile))
                {
                    LoadJob();
                }				

                using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath + "\\Window", false))
                {
                    if (reg != null)
                    {
                        this.Left = (int)reg.GetValue("Left", this.Left);
                        this.Top = (int)reg.GetValue("Top", this.Top);
                        this.Width = (int)reg.GetValue("Width", this.Width);
                        this.Height = (int)reg.GetValue("Height", this.Height);
                    }
                    else
                    {
                        this.Left = Screen.PrimaryScreen.Bounds.X + (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                        this.Top = Screen.PrimaryScreen.Bounds.Y + (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
                    }
                }
            }
        }

        private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!TestForUnsavedJob())
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Event-handler for ListView

        private void lvSource_PopupMenuEvent(object sender, MouseEventArgs e)
        {
            ShowActionPopupMenu(e.X, e.Y);
        }

        private void lvSource_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("(ColumnClick); Idx: " + e.Column.ToString() + ", nr: " + ColIdx2Nr(e.Column));
            if (e.Column == sortingColumn)
            {
                sortingOrderAsc = !sortingOrderAsc;
            }
            sortingColumn = e.Column;        
            lvSource.Sort(sortingColumn, sortingOrderAsc ? SortOrder.Ascending : SortOrder.Descending, ColNr2Idx(ColumnNumbers.SIZE));
        }


        private void lvSource_HeaderMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            contextMenuColumns.Show(lvSource, new Point(e.X, e.Y));
        }

        #endregion

        #region Event-handler for "action" context menu

        private void mnuActionNone_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_NONE);
        }

        private void mnuActionOwSource_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_OVERWRITE_SOURCE);
        }

        private void mnuActionOwDestination_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_OVERWRITE_DESTINATION);
        }

        private void mnuActionCrSource_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_CREATE_SOURCE);
        }

        private void mnuActionCrDestination_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_CREATE_DESTINATION);
        }

        private void mnuActionDelSource_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_DELETE_SOURCE);
        }

        private void mnuActionDelDestination_Click(object sender, System.EventArgs e)
        {
            SetActionForSelection(ACTION_DELETE_DESTINATION);
        }

        #endregion

        #endregion

        /// <summary>
        /// The default job is saved.
        /// </summary>
        void SaveJob()
        {
            string jobFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (Directory.Exists(jobFile))
            {
                jobFile = Path.Combine(jobFile, "InZync");
                if (!Directory.Exists(jobFile))
                {
                    Directory.CreateDirectory(jobFile);
                }
                SaveJob(Path.Combine(jobFile, "defaults.xml"), true);
            }
        }

        /// <summary>
        /// The default job is loaded.
        /// </summary>
        void LoadJob()
        {
            string job = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InZync", "defaults.xml");
            if (File.Exists(job))
            {
                LoadJob(job, true);
            }
        }

        /// <summary>
        /// The job from the given file is loaded.
        /// </summary>
        /// <param name="fileName">filename of job to load</param>
        void LoadJob(string fileName)
        {
            LoadJob(fileName, false);
        }

        /// <summary>
        /// The job from the given file is loaded.
        /// </summary>
        /// <param name="fileName">filename of job to load</param>
        /// <param name="defaultSetting">if true then the filename is not saved</param>
        void LoadJob(string fileName, bool defaultSetting)
        {
            if (defaultSetting)
            {
                jobFile = "";
            }
            else 
            {
                jobFile = fileName;
            }
            jobChanged = false;

            // load file into Settings class
            try
            {
                settings.LoadJob(fileName, syncPathList);
            }
            catch
            {
                MessageBox.Show(this, $"The file \"{fileName}\" is no valid InZync job.", this.Text);
            }

            MakeWindowCaption();
            cmdCompare.Enabled = (syncPathList.Count > 0);
            mnuFileCompare.Enabled = cmdCompare.Enabled;
            mnuFilterOnlyDifferent.Checked = settings.ShowOnlyDifferentFiles;
        }

        /// <summary>
        /// Save current settings in a jobfile with the given name.
        /// </summary>
        /// <param name="fileName">filename of jobfile to save</param>
        /// <param name="isDefaultJob">true if the default job is saved</param>
        void SaveJob(string fileName, bool isDefaultJob)
        {
            if (!isDefaultJob)
            {
                this.jobFile = fileName;
                this.jobChanged = false;
                MakeWindowCaption();
            }

            // save Settings class to file
            settings.SaveJob(fileName, syncPathList);
        }

        /// <summary>
        /// Show the "action" popup menu at the given coordinates.
        /// </summary>
        /// <param name="x">x coordinate to show popup menu</param>
        /// <param name="y">y coordinate to show popup menu</param>
        private void ShowActionPopupMenu(int x, int y)
        {
            if (lvSource.SelectedItems.Count == 0)
            {
                return;
            }

            bool sourceMissing = false;
            bool destMissing = false;
            bool sourceNewer = false;
            bool destNewer = false;
            bool equal = false;

            // test if all selected items have the same status
            string status = "";
            foreach (ListViewItem lvi in lvSource.SelectedItems)
            {
                status = lvi.SubItems[ColNr2Idx(ColumnNumbers.STATUS)].Text;
                if (status == STATUS_SOURCE_NEWER)
                {
                    sourceNewer = true;
                }
                else if (status == STATUS_DESTINATION_NEWER)
                {
                    destNewer = true;
                }
                else if (status == STATUS_SOURCE_MISSING)
                {
                    sourceMissing = true;
                }
                else if (status == STATUS_DESTINATION_MISSING)
                {
                    destMissing = true;
                }
                else if (status == STATUS_EQUAL)
                {
                    equal = true;
                }
            }

            // enable everything per default
            mnuActionNone.Visible = true;
            mnuActionOwSource.Visible = true;
            mnuActionOwDestination.Visible = true;
            mnuActionCrSource.Visible = true;
            mnuActionCrDestination.Visible = true;
            mnuActionDelSource.Visible = true;
            mnuActionDelDestination.Visible = true;

            // disable actions depending on selection
            if (sourceNewer)
            {
                mnuActionCrSource.Visible = false;
                mnuActionCrDestination.Visible = false;
                mnuActionDelSource.Visible = false;
                mnuActionDelDestination.Visible = false;
            }
            if (destNewer)
            {
                mnuActionCrSource.Visible = false;
                mnuActionCrDestination.Visible = false;
                mnuActionDelSource.Visible = false;
                mnuActionDelDestination.Visible = false;
            }
            if (sourceMissing)
            {
                mnuActionOwSource.Visible = false;
                mnuActionOwDestination.Visible = false;
                mnuActionCrDestination.Visible = false;
                mnuActionDelSource.Visible = false;
            }
            if (destMissing)
            {
                mnuActionOwSource.Visible = false;
                mnuActionOwDestination.Visible = false;
                mnuActionCrSource.Visible = false;
                mnuActionDelDestination.Visible = false;
            }

            for (int i = initialContextMenuItemsCount; i < contextMenu.Items.Count; i++)
            {
                contextMenu.Items[i].Visible = ((lvSource.SelectedItems.Count == 1) && (!equal) && (!sourceMissing) && (!destMissing));
            }

            contextMenu.Show(lvSource, new Point(x, y));
        }

        /// <summary>
        /// If double-clicked in two different files, the default context menu
        /// entry is executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSource_DoubleClick(object sender, EventArgs e)
        {
            if ((settings.ContextDblClickIdx != -1) &&
                (settings.ContextDblClickIdx < settings.ContextMenu.Count) &&
                (lvSource.SelectedItems.Count == 1))
            {
                string cmd = settings.ContextMenu[settings.ContextDblClickIdx].Split('\t')[1];
                ListViewItem lvi = lvSource.SelectedItems[0];
                string status = lvi.SubItems[ColNr2Idx(ColumnNumbers.STATUS)].Text;
                if (File.Exists(cmd) && ((status == STATUS_SOURCE_NEWER) || (status == STATUS_DESTINATION_NEWER)))
                {					
                    string source = syncList[(int)lvi.Tag].SourcePath + syncList[(int)lvi.Tag].RelPath;
                    string destination = syncList[(int)lvi.Tag].DestPath + syncList[(int)lvi.Tag].RelPath;

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = cmd;
                    proc.StartInfo.Verb = "open";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.Arguments = $"\"{source} \" \"{destination}\"";
                    proc.Start();
                }
            }
        }

        /// <summary>
        /// Set the title of the main window. The title includes the program version
        /// and optionally the name of a loaded job.
        /// </summary>
        private void MakeWindowCaption()
        {
            string caption = WINDOW_CAPTION;
            if (!string.IsNullOrEmpty(jobFile))
            {
                caption += " [" + jobFile;
                if (jobChanged)
                {
                    caption += "*";
                }
                caption += "]";
            }
            if (settings.ShowOnlyDifferentFiles)
            {
                caption += " - only different files";
            }
            this.Text = caption;
        }

        /// <summary>
        /// Show the dialog with the paths to synchronize.
        /// </summary>
        private void CommandSetPaths()
        {
            Forms.PathList pl = new Forms.PathList();
            var savedList = syncPathList.ToArray();
            if (pl.ShowDialog(this, syncPathList) == DialogResult.OK)
            {
                jobChanged |= (savedList.Length != syncPathList.Count || savedList.Except(syncPathList).Any());
                cmdCompare.Enabled = (syncPathList.Count > 0);
                mnuFileCompare.Enabled = cmdCompare.Enabled;
                MakeWindowCaption();
            }
        }

        /// <summary>
        /// Show the configuration dialog.
        /// </summary>
        private void CommandSettings()
        {
            using (SettingsForm frm = new Forms.SettingsForm(settings))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SetContextMenu();
                    if (frm.Changed)
                    {
                        if (!string.IsNullOrEmpty(jobFile))
                        {
                            jobChanged = true;
                            MakeWindowCaption();
                        }
                        if (syncList.Count > 0)
                        {
                            DialogResult dr = MessageBox.Show(this, "You have changed some compare/sync options. Do you wish to recompare?", "Options changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                CommandCompare();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is called when the application is closed or if a new jobfile
        /// is opened. If a jobfile has been loaded an modified a message box is displayed asking
        /// whether to save the current job or not.
        /// </summary>
        /// <returns>false if the action should be aborted</returns>
        private bool TestForUnsavedJob()
        {
            if (!string.IsNullOrEmpty(jobFile) && jobChanged)
            {
                DialogResult dr = MessageBox.Show(this, $"The current job has been edited, do you wish to save \"{jobFile}\"?", "InZync - Save changed job?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Yes)
                {
                    SaveJob(jobFile, false);
                    // save old job and load new one
                    return true;
                }
                if (dr == DialogResult.No)
                {
                    // load new job
                    return true;
                }
                // cancel
                return false;
            }
            return true;
        }

        /// <summary>
        /// Enable or disable all controls.
        /// </summary>
        /// <param name="enable">true to enable controls</param>
        private void EnableAllCommands(bool enable)
        {
            cmdCompare.Enabled = enable;
            cmdPaths.Enabled = enable;
            cmdSettings.Enabled = enable;
            mnuFile.Enabled = enable;
            mnuOptions.Enabled = enable;
            mnuFilter.Enabled = enable;
            mnuHelp.Enabled = enable;
            lvSource.Enabled = enable;

            cmdSync.Enabled = (syncList.Count > 0) && enable;
            mnuFileSync.Enabled = (syncList.Count > 0) && enable;
            mnuOptionsClear.Enabled = (syncList.Count > 0) && enable;
        }

        /// <summary>
        /// Format a file size expression.
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>formatted size</returns>
        private string FormatSize(long size)
        {
            if (size == 0)
            {
                return "";
            } 
            else 
            {
                if (size < 1024)
                {
                    if (size == 0)
                    {
                        return "0 Bytes";
                    }
                    return string.Format("{0:#,#} Bytes", size);
                }
                size /= 1024;
                if (size < 1024)
                {
                    return string.Format("{0:#,# KB}", size);
                }
                size /= 1024;
                return string.Format("{0:#,# MB}", size);
            }
        }

        /// <summary>
        /// Set the action string for all selected items in lvSource.
        /// </summary>
        /// <param name="action">action string</param>
        private void SetActionForSelection(string action)
        {
            foreach (ListViewItem lvi in lvSource.SelectedItems)
            {
                lvi.SubItems[ColNr2Idx(ColumnNumbers.ACTION)].Text = action;
                syncList[(int)lvi.Tag].Action = action;
            }
        }

        /// <summary>
        /// Adds all custom context menu entries.
        /// </summary>
        private void SetContextMenu()
        {
            while (contextMenu.Items.Count > initialContextMenuItemsCount)
            {
                contextMenu.Items.RemoveAt(contextMenu.Items.Count - 1);
            }

            if (settings.ContextMenu.Count > 0)
            {
                contextMenu.Items.Add("-");
                for (int i = 0; i < settings.ContextMenu.Count; i++)
                {
                    string[] itemArr = settings.ContextMenu[i].Split('\t');
                    if (itemArr.Length == 2)
                    {
                        IconHandler icon = new IconHandler(itemArr[1]);
                        ToolStripItem tsi = contextMenu.Items.Add(itemArr[0]);
                        if (i == settings.ContextDblClickIdx)
                        {
                            tsi.Font = new Font(tsi.Font.FontFamily.Name, tsi.Font.Size, tsi.Font.Style | FontStyle.Bold);
                        }
                        tsi.Image = icon.FileBitmap;
                        tsi.Tag = itemArr[1];
                        tsi.Click += new EventHandler(tsi_Click);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler that is called if a custom context menu entry is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tsi_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = sender as ToolStripItem;
            if (tsi != null)
            {
                string cmd = tsi.Tag.ToString();
                if (File.Exists(cmd))
                {
                    if (lvSource.SelectedItems.Count > 0)
                    {
                        ListViewItem lvi = lvSource.SelectedItems[0];
                        string source = syncList[(int)lvi.Tag].SourcePath + syncList[(int)lvi.Tag].RelPath;
                        string destination = syncList[(int)lvi.Tag].DestPath + syncList[(int)lvi.Tag].RelPath;

                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = cmd;
                        proc.StartInfo.Verb = "open";
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.Arguments = $"\"{source}\" \"{destination}\"";
                        proc.Start();
                    }
                }
            }
        }

        #region Helper methods for ListView columns

        /// <summary>
        /// Convert the column index in lvSource.Columns[] into the column number.
        /// </summary>
        /// <param name="colIdx">column index</param>
        /// <returns>column number</returns>
        private ColumnNumbers ColIdx2Nr(int colIdx)
        {
            for (int i = 0; i < NO_OF_COLUMNS; i++)
            {
                if (colListViewPos[i] == colIdx)
                {
                    return (ColumnNumbers)i;
                }
            }
            return ColumnNumbers.UNKNOWN;
        }

        /// <summary>
        /// Convert the column number to a column index in lvSource.Columns[].
        /// </summary>
        /// <param name="colNr">column number</param>
        /// <returns>column index</returns>
        private int ColNr2Idx(ColumnNumbers colNr)
        {
            return colListViewPos[(int)colNr];
        }

        /// <summary>
        /// Checks whether a given column is visible.
        /// </summary>
        /// <param name="colNr">column number</param>
        /// <returns>true if column is visible</returns>
        private bool ColVisible(ColumnNumbers colNr)
        {
            if (colNr != ColumnNumbers.UNKNOWN)
            {
                return (colListViewPos[(int)colNr] >= 0);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the column with the given number.
        /// </summary>
        /// <param name="colNr">column number to remove</param>
        /// <returns>true if successful</returns>
        private bool RemoveCol(ColumnNumbers colNr)
        {
            int colIdx = ColNr2Idx(colNr);
            if (colIdx >= 0)
            {
                if ((colIdx >= 0) && (colIdx < lvSource.Columns.Count))
                {
                    // save sort column
                    bool newSortCol = (colIdx == sortingColumn);
                    ColumnNumbers sortNr = ColIdx2Nr(sortingColumn);

                    // remove column and rebuild index
                    lvSource.Columns.RemoveAt(colIdx);
                    RebuildColIndex();

                    // set new sort column index
                    if (newSortCol)
                    {
                        sortingColumn = 0;
                    }
                    else
                    {
                        sortingColumn = ColNr2Idx(sortNr);
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Insert a new column in lvSource at its default position.
        /// </summary>
        /// <param name="colNr">column to insert</param>
        private void InsertCol(ColumnNumbers colNr)
        {
            InsertCol(colNr, (int)colNr);
        }

        /// <summary>
        /// Insert a new column in lvSource at the given position.
        /// </summary>
        /// <param name="colNr">column to insert</param>
        /// <param name="displayIndex">where to insert the column</param>
        private void InsertCol(ColumnNumbers colNr, int displayIndex)
        {
            if (ColVisible(colNr))
            {
                return;
            }

            // create new column
            ColumnHeader newHeader = new ColumnHeader();
            newHeader.Tag = colNr;
            newHeader.Text = "?";
            newHeader.Width = 100;
            newHeader.TextAlign = HorizontalAlignment.Left;            

            // set column header caption and default width
            switch (colNr)
            {
                case ColumnNumbers.SRCPATH:
                    newHeader.Text = "Source path";
                    newHeader.Width = 150;
                    break;
                case ColumnNumbers.DESTPATH:
                    newHeader.Text = "Destination path";
                    newHeader.Width = 150;
                    break;
                case ColumnNumbers.SIZE:
                    newHeader.Text = "Size";
                    newHeader.Width = 60;
                    newHeader.TextAlign = HorizontalAlignment.Right;
                    break;
                case ColumnNumbers.DATE:
                    newHeader.Text = "Last modified";
                    break;
                case ColumnNumbers.STATUS:
                    newHeader.Text = "Status";
                    break;
                case ColumnNumbers.ACTION:
                    newHeader.Text = "Action";
                    break;
            }

            // read width from registry
            using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath + "\\Columns", false))
            {
                if (reg != null)
                {
                    newHeader.Width = (int)reg.GetValue("Witdh" + ((int)colNr).ToString(), newHeader.Width);
                }
            }

            // insert column and rebuild index
            lvSource.Columns.Add(newHeader);
            if ((displayIndex >= 0) && (displayIndex < lvSource.Columns.Count))
            {
                newHeader.DisplayIndex = displayIndex;
            }
            RebuildColIndex();
        }

        /// <summary>
        /// The column index in colListViewPos is rebuilt.
        /// </summary>
        private void RebuildColIndex()
        {
            for (int i = 0; i < NO_OF_COLUMNS; i++)
            {
                colListViewPos[i] = -1;
            }
            foreach (ColumnHeader header in lvSource.Columns)
            {
                colListViewPos[(int)header.Tag] = header.Index;
            }
        }

        #endregion

        /// <summary>
        /// Get initial path from registry.
        /// </summary>
        /// <returns></returns>
        private string GetInitialPath()
        {
            using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath, false))
            {
                if (reg != null)
                {
                    return reg.GetValue("JobFilePath", Environment.GetFolderPath(Environment.SpecialFolder.Personal)).ToString();
                }
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// Save the initial path in the registry.
        /// </summary>
        /// <param name="path">path</param>
        private void UpdateInitialPath(string path)
        {
            using (var reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath, true))
            {
                if (reg != null)
                {
                    reg.SetValue("JobFilePath", path);
                }
            }
        }
    }
}
