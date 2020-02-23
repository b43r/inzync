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

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace InZync.Forms
{
	/// <summary>
	/// This class represents the settings dialog.
	/// </summary>
	public class SettingsForm : Form
    {
		private Button cmdOk;
		private Button cmdCancel;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private GroupBox groupBox4;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private GroupBox groupBox5;
		private RadioButton rbSourceNewer1;
		private RadioButton rbSourceNewer2;
		private RadioButton rbSourceNewer3;
		private RadioButton rbDestNewer3;
		private RadioButton rbDestNewer2;
		private RadioButton rbDestNewer1;
		private RadioButton rbSourceMissing1;
		private RadioButton rbSourceMissing2;
		private RadioButton rbDestMissing2;
		private RadioButton rbDestMissing1;
		private CheckBox chkSubDirectories;
		private RadioButton rbSourceMissing3;
		private RadioButton rbDestMissing3;
		private CheckBox chkDirectoriesLikeFiles;
		private GroupBox groupBox7;
		private ComboBox cbTemplates;
		private TabPage tabPage3;
		private GroupBox groupBox8;
		private CheckBox chkShowLogWindow;
		private CheckBox chkSaveLog;
		private ComboBox cbLogFile;
		private IContainer components;
		private GroupBox groupBox9;
		private CheckBox chkSystemFiles;
		private CheckBox chkHiddenFiles;
		private GroupBox groupBox10;
		private CheckBox chkReadOnly;
		private RadioButton optAppend;
		private RadioButton optOverwrite;
		private CheckBox chkBackup;
		private TabPage tabPage4;
		private Label lblExtensions;
		private TreeView tvExt;
		private TextBox txtExt;
		private Button cmdAddExt;
		private Button cmdRemoveExt;
		private ImageList imageList;
		private GroupBox groupBox11;
		private CheckBox chkFileAss;
		private Button cmdRemoveAll;
		private CheckBox chkTerminateApp;
		private GroupBox groupBox6;
		private CheckBox chkSilent;
		private RadioButton optDblClickRun;
		private RadioButton optDblClickOpen;
        private Label lblWildcards;
		private Button cmdAddAll;
		private TextBox txtExcludeExt;
		private Label lblExclude;
		private Button cmdExcludeRemove;
		private TabPage tabPage5;
		private Button cmdContextUpdate;
		private Button cmdContextRemove;
		private Button cmdContextAdd;
		private Label label3;
		private Button cmdBrowse;
		private TextBox txtContextFile;
		private TextBox txtContextCaption;
		private Label label2;
		private Label label1;
		private ListBox lstContext;
		private OpenFileDialog openFileDialog;
		private Button cmdDefaults;
		private CheckBox chkExecAtDblClick;
		private Settings settings;
		private int contextDblClickIdx;

        private readonly bool isAdmin;

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);

        const UInt32 BCM_SETSHIELD = 0x160C;

        /// <summary>
        /// Constructor, show the settings dialog based on the properties in the Settings class.
        /// </summary>
        /// <param name="settings">Settings class</param>
        public SettingsForm(Settings settings)
		{
			InitializeComponent();

			this.settings = settings;
            this.settings.LoadRegistrySettings();

			// default logfile locations
			cbLogFile.Items.Add(Path.Combine(Application.StartupPath, "InZync.log"));
			cbLogFile.Items.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "InZync.log"));

			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal(identity);
			isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

        /// <summary>
        /// Returns true if something has been changed.
        /// </summary>
        public bool Changed { get; private set; }

        protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSourceNewer3 = new System.Windows.Forms.RadioButton();
            this.rbSourceNewer2 = new System.Windows.Forms.RadioButton();
            this.rbSourceNewer1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbDestNewer3 = new System.Windows.Forms.RadioButton();
            this.rbDestNewer2 = new System.Windows.Forms.RadioButton();
            this.rbDestNewer1 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbSourceMissing3 = new System.Windows.Forms.RadioButton();
            this.rbSourceMissing2 = new System.Windows.Forms.RadioButton();
            this.rbSourceMissing1 = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbDestMissing3 = new System.Windows.Forms.RadioButton();
            this.rbDestMissing2 = new System.Windows.Forms.RadioButton();
            this.rbDestMissing1 = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkSilent = new System.Windows.Forms.CheckBox();
            this.chkTerminateApp = new System.Windows.Forms.CheckBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.optDblClickOpen = new System.Windows.Forms.RadioButton();
            this.optDblClickRun = new System.Windows.Forms.RadioButton();
            this.chkFileAss = new System.Windows.Forms.CheckBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.optOverwrite = new System.Windows.Forms.RadioButton();
            this.optAppend = new System.Windows.Forms.RadioButton();
            this.cbLogFile = new System.Windows.Forms.ComboBox();
            this.chkSaveLog = new System.Windows.Forms.CheckBox();
            this.chkShowLogWindow = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.chkHiddenFiles = new System.Windows.Forms.CheckBox();
            this.chkSystemFiles = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkSubDirectories = new System.Windows.Forms.CheckBox();
            this.chkDirectoriesLikeFiles = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.cmdExcludeRemove = new System.Windows.Forms.Button();
            this.txtExcludeExt = new System.Windows.Forms.TextBox();
            this.lblExclude = new System.Windows.Forms.Label();
            this.cmdAddAll = new System.Windows.Forms.Button();
            this.lblWildcards = new System.Windows.Forms.Label();
            this.cmdRemoveAll = new System.Windows.Forms.Button();
            this.cmdRemoveExt = new System.Windows.Forms.Button();
            this.cmdAddExt = new System.Windows.Forms.Button();
            this.tvExt = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.txtExt = new System.Windows.Forms.TextBox();
            this.lblExtensions = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkBackup = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cbTemplates = new System.Windows.Forms.ComboBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.chkExecAtDblClick = new System.Windows.Forms.CheckBox();
            this.cmdDefaults = new System.Windows.Forms.Button();
            this.cmdContextUpdate = new System.Windows.Forms.Button();
            this.cmdContextRemove = new System.Windows.Forms.Button();
            this.cmdContextAdd = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.txtContextFile = new System.Windows.Forms.TextBox();
            this.txtContextCaption = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstContext = new System.Windows.Forms.ListBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(392, 432);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 0;
            this.cmdOk.Text = "&Ok";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(472, 432);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSourceNewer3);
            this.groupBox1.Controls.Add(this.rbSourceNewer2);
            this.groupBox1.Controls.Add(this.rbSourceNewer1);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 104);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "If source is newer:";
            // 
            // rbSourceNewer3
            // 
            this.rbSourceNewer3.Location = new System.Drawing.Point(24, 72);
            this.rbSourceNewer3.Name = "rbSourceNewer3";
            this.rbSourceNewer3.Size = new System.Drawing.Size(168, 24);
            this.rbSourceNewer3.TabIndex = 2;
            this.rbSourceNewer3.Text = "do nothing";
            this.rbSourceNewer3.CheckedChanged += new System.EventHandler(this.rbSourceNewer3_CheckedChanged);
            // 
            // rbSourceNewer2
            // 
            this.rbSourceNewer2.Location = new System.Drawing.Point(24, 48);
            this.rbSourceNewer2.Name = "rbSourceNewer2";
            this.rbSourceNewer2.Size = new System.Drawing.Size(168, 24);
            this.rbSourceNewer2.TabIndex = 1;
            this.rbSourceNewer2.Text = "overwrite source";
            this.rbSourceNewer2.CheckedChanged += new System.EventHandler(this.rbSourceNewer2_CheckedChanged);
            // 
            // rbSourceNewer1
            // 
            this.rbSourceNewer1.Location = new System.Drawing.Point(24, 24);
            this.rbSourceNewer1.Name = "rbSourceNewer1";
            this.rbSourceNewer1.Size = new System.Drawing.Size(168, 24);
            this.rbSourceNewer1.TabIndex = 0;
            this.rbSourceNewer1.Text = "overwrite destination";
            this.rbSourceNewer1.CheckedChanged += new System.EventHandler(this.rbSourceNewer1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbDestNewer3);
            this.groupBox2.Controls.Add(this.rbDestNewer2);
            this.groupBox2.Controls.Add(this.rbDestNewer1);
            this.groupBox2.Location = new System.Drawing.Point(272, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(248, 104);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "If destination is newer:";
            // 
            // rbDestNewer3
            // 
            this.rbDestNewer3.Location = new System.Drawing.Point(24, 72);
            this.rbDestNewer3.Name = "rbDestNewer3";
            this.rbDestNewer3.Size = new System.Drawing.Size(168, 24);
            this.rbDestNewer3.TabIndex = 2;
            this.rbDestNewer3.Text = "do nothing";
            this.rbDestNewer3.CheckedChanged += new System.EventHandler(this.rbDestNewer3_CheckedChanged);
            // 
            // rbDestNewer2
            // 
            this.rbDestNewer2.Location = new System.Drawing.Point(24, 48);
            this.rbDestNewer2.Name = "rbDestNewer2";
            this.rbDestNewer2.Size = new System.Drawing.Size(168, 24);
            this.rbDestNewer2.TabIndex = 1;
            this.rbDestNewer2.Text = "overwrite destination";
            this.rbDestNewer2.CheckedChanged += new System.EventHandler(this.rbDestNewer2_CheckedChanged);
            // 
            // rbDestNewer1
            // 
            this.rbDestNewer1.Location = new System.Drawing.Point(24, 24);
            this.rbDestNewer1.Name = "rbDestNewer1";
            this.rbDestNewer1.Size = new System.Drawing.Size(168, 24);
            this.rbDestNewer1.TabIndex = 0;
            this.rbDestNewer1.Text = "overwrite source";
            this.rbDestNewer1.CheckedChanged += new System.EventHandler(this.rbDestNewer1_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbSourceMissing3);
            this.groupBox3.Controls.Add(this.rbSourceMissing2);
            this.groupBox3.Controls.Add(this.rbSourceMissing1);
            this.groupBox3.Location = new System.Drawing.Point(16, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(248, 104);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "If source is missing:";
            // 
            // rbSourceMissing3
            // 
            this.rbSourceMissing3.Location = new System.Drawing.Point(24, 72);
            this.rbSourceMissing3.Name = "rbSourceMissing3";
            this.rbSourceMissing3.Size = new System.Drawing.Size(168, 24);
            this.rbSourceMissing3.TabIndex = 2;
            this.rbSourceMissing3.Text = "do nothing";
            this.rbSourceMissing3.CheckedChanged += new System.EventHandler(this.rbSourceMissing3_CheckedChanged);
            // 
            // rbSourceMissing2
            // 
            this.rbSourceMissing2.Location = new System.Drawing.Point(24, 48);
            this.rbSourceMissing2.Name = "rbSourceMissing2";
            this.rbSourceMissing2.Size = new System.Drawing.Size(168, 24);
            this.rbSourceMissing2.TabIndex = 1;
            this.rbSourceMissing2.Text = "delete destination";
            this.rbSourceMissing2.CheckedChanged += new System.EventHandler(this.rbSourceMissing2_CheckedChanged);
            // 
            // rbSourceMissing1
            // 
            this.rbSourceMissing1.Location = new System.Drawing.Point(24, 24);
            this.rbSourceMissing1.Name = "rbSourceMissing1";
            this.rbSourceMissing1.Size = new System.Drawing.Size(168, 24);
            this.rbSourceMissing1.TabIndex = 0;
            this.rbSourceMissing1.Text = "create source";
            this.rbSourceMissing1.CheckedChanged += new System.EventHandler(this.rbSourceMissing1_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbDestMissing3);
            this.groupBox4.Controls.Add(this.rbDestMissing2);
            this.groupBox4.Controls.Add(this.rbDestMissing1);
            this.groupBox4.Location = new System.Drawing.Point(272, 128);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(248, 104);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "If destination is missing:";
            // 
            // rbDestMissing3
            // 
            this.rbDestMissing3.Location = new System.Drawing.Point(24, 72);
            this.rbDestMissing3.Name = "rbDestMissing3";
            this.rbDestMissing3.Size = new System.Drawing.Size(168, 24);
            this.rbDestMissing3.TabIndex = 3;
            this.rbDestMissing3.Text = "do nothing";
            this.rbDestMissing3.CheckedChanged += new System.EventHandler(this.rbDestMissing3_CheckedChanged);
            // 
            // rbDestMissing2
            // 
            this.rbDestMissing2.Location = new System.Drawing.Point(24, 48);
            this.rbDestMissing2.Name = "rbDestMissing2";
            this.rbDestMissing2.Size = new System.Drawing.Size(168, 24);
            this.rbDestMissing2.TabIndex = 1;
            this.rbDestMissing2.Text = "delete source";
            this.rbDestMissing2.CheckedChanged += new System.EventHandler(this.rbDestMissing2_CheckedChanged);
            // 
            // rbDestMissing1
            // 
            this.rbDestMissing1.Location = new System.Drawing.Point(24, 24);
            this.rbDestMissing1.Name = "rbDestMissing1";
            this.rbDestMissing1.Size = new System.Drawing.Size(168, 24);
            this.rbDestMissing1.TabIndex = 0;
            this.rbDestMissing1.Text = "create destination";
            this.rbDestMissing1.CheckedChanged += new System.EventHandler(this.rbDestMissing1_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(544, 416);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.groupBox11);
            this.tabPage3.Controls.Add(this.groupBox10);
            this.tabPage3.Controls.Add(this.groupBox8);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(536, 390);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "General";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkSilent);
            this.groupBox6.Controls.Add(this.chkTerminateApp);
            this.groupBox6.Location = new System.Drawing.Point(16, 304);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(504, 72);
            this.groupBox6.TabIndex = 3;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Automated job execution";
            // 
            // chkSilent
            // 
            this.chkSilent.Location = new System.Drawing.Point(24, 48);
            this.chkSilent.Name = "chkSilent";
            this.chkSilent.Size = new System.Drawing.Size(440, 16);
            this.chkSilent.TabIndex = 2;
            this.chkSilent.Text = "Do not show user interface";
            // 
            // chkTerminateApp
            // 
            this.chkTerminateApp.Location = new System.Drawing.Point(24, 24);
            this.chkTerminateApp.Name = "chkTerminateApp";
            this.chkTerminateApp.Size = new System.Drawing.Size(440, 24);
            this.chkTerminateApp.TabIndex = 1;
            this.chkTerminateApp.Text = "Terminate application after a jobfile has been executed and the job has finished";
            this.chkTerminateApp.CheckedChanged += new System.EventHandler(this.chkTerminateApp_CheckedChanged);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.optDblClickOpen);
            this.groupBox11.Controls.Add(this.optDblClickRun);
            this.groupBox11.Controls.Add(this.chkFileAss);
            this.groupBox11.Location = new System.Drawing.Point(16, 208);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(504, 88);
            this.groupBox11.TabIndex = 2;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "File association";
            // 
            // optDblClickOpen
            // 
            this.optDblClickOpen.Location = new System.Drawing.Point(40, 64);
            this.optDblClickOpen.Name = "optDblClickOpen";
            this.optDblClickOpen.Size = new System.Drawing.Size(208, 16);
            this.optDblClickOpen.TabIndex = 2;
            this.optDblClickOpen.Text = "Doubleclick opens job in InZync";
            // 
            // optDblClickRun
            // 
            this.optDblClickRun.Location = new System.Drawing.Point(40, 48);
            this.optDblClickRun.Name = "optDblClickRun";
            this.optDblClickRun.Size = new System.Drawing.Size(136, 16);
            this.optDblClickRun.TabIndex = 1;
            this.optDblClickRun.Text = "Doubleclick runs job";
            this.optDblClickRun.CheckedChanged += new System.EventHandler(this.optDblClickRun_CheckedChanged);
            // 
            // chkFileAss
            // 
            this.chkFileAss.Location = new System.Drawing.Point(24, 24);
            this.chkFileAss.Name = "chkFileAss";
            this.chkFileAss.Size = new System.Drawing.Size(392, 16);
            this.chkFileAss.TabIndex = 0;
            this.chkFileAss.Text = "Associate the file extension .syncjob with InZync";
            this.chkFileAss.CheckedChanged += new System.EventHandler(this.chkFileAss_CheckedChanged);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.chkReadOnly);
            this.groupBox10.Location = new System.Drawing.Point(16, 152);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(504, 48);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Read-only attribute";
            // 
            // chkReadOnly
            // 
            this.chkReadOnly.Location = new System.Drawing.Point(24, 24);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(376, 16);
            this.chkReadOnly.TabIndex = 0;
            this.chkReadOnly.Text = "Remove read-only attribute if necessary";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.optOverwrite);
            this.groupBox8.Controls.Add(this.optAppend);
            this.groupBox8.Controls.Add(this.cbLogFile);
            this.groupBox8.Controls.Add(this.chkSaveLog);
            this.groupBox8.Controls.Add(this.chkShowLogWindow);
            this.groupBox8.Location = new System.Drawing.Point(16, 16);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(504, 128);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Log";
            // 
            // optOverwrite
            // 
            this.optOverwrite.Location = new System.Drawing.Point(112, 104);
            this.optOverwrite.Name = "optOverwrite";
            this.optOverwrite.Size = new System.Drawing.Size(72, 16);
            this.optOverwrite.TabIndex = 4;
            this.optOverwrite.Text = "overwrite";
            // 
            // optAppend
            // 
            this.optAppend.Location = new System.Drawing.Point(40, 104);
            this.optAppend.Name = "optAppend";
            this.optAppend.Size = new System.Drawing.Size(64, 16);
            this.optAppend.TabIndex = 3;
            this.optAppend.Text = "append";
            // 
            // cbLogFile
            // 
            this.cbLogFile.Location = new System.Drawing.Point(40, 80);
            this.cbLogFile.Name = "cbLogFile";
            this.cbLogFile.Size = new System.Drawing.Size(448, 21);
            this.cbLogFile.TabIndex = 2;
            // 
            // chkSaveLog
            // 
            this.chkSaveLog.Location = new System.Drawing.Point(24, 56);
            this.chkSaveLog.Name = "chkSaveLog";
            this.chkSaveLog.Size = new System.Drawing.Size(376, 16);
            this.chkSaveLog.TabIndex = 1;
            this.chkSaveLog.Text = "Save log to file";
            this.chkSaveLog.CheckedChanged += new System.EventHandler(this.chkSaveLog_CheckedChanged);
            // 
            // chkShowLogWindow
            // 
            this.chkShowLogWindow.Location = new System.Drawing.Point(24, 24);
            this.chkShowLogWindow.Name = "chkShowLogWindow";
            this.chkShowLogWindow.Size = new System.Drawing.Size(376, 24);
            this.chkShowLogWindow.TabIndex = 0;
            this.chkShowLogWindow.Text = "Show log window";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(536, 390);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Filter options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.chkHiddenFiles);
            this.groupBox9.Controls.Add(this.chkSystemFiles);
            this.groupBox9.Location = new System.Drawing.Point(16, 112);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(504, 80);
            this.groupBox9.TabIndex = 2;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Attributes:";
            // 
            // chkHiddenFiles
            // 
            this.chkHiddenFiles.Location = new System.Drawing.Point(24, 24);
            this.chkHiddenFiles.Name = "chkHiddenFiles";
            this.chkHiddenFiles.Size = new System.Drawing.Size(192, 16);
            this.chkHiddenFiles.TabIndex = 1;
            this.chkHiddenFiles.Text = "Process hidden files/directories";
            this.chkHiddenFiles.CheckedChanged += new System.EventHandler(this.chkHiddenFiles_CheckedChanged);
            // 
            // chkSystemFiles
            // 
            this.chkSystemFiles.Location = new System.Drawing.Point(24, 48);
            this.chkSystemFiles.Name = "chkSystemFiles";
            this.chkSystemFiles.Size = new System.Drawing.Size(192, 16);
            this.chkSystemFiles.TabIndex = 0;
            this.chkSystemFiles.Text = "Process system files/directories";
            this.chkSystemFiles.CheckedChanged += new System.EventHandler(this.chkSystemFiles_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkSubDirectories);
            this.groupBox5.Controls.Add(this.chkDirectoriesLikeFiles);
            this.groupBox5.Location = new System.Drawing.Point(16, 16);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(504, 88);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Directories:";
            // 
            // chkSubDirectories
            // 
            this.chkSubDirectories.Location = new System.Drawing.Point(24, 48);
            this.chkSubDirectories.Name = "chkSubDirectories";
            this.chkSubDirectories.Size = new System.Drawing.Size(184, 24);
            this.chkSubDirectories.TabIndex = 1;
            this.chkSubDirectories.Text = "Compare/sync subdirectories";
            this.chkSubDirectories.CheckedChanged += new System.EventHandler(this.chkSubDirectories_CheckedChanged);
            // 
            // chkDirectoriesLikeFiles
            // 
            this.chkDirectoriesLikeFiles.Location = new System.Drawing.Point(24, 24);
            this.chkDirectoriesLikeFiles.Name = "chkDirectoriesLikeFiles";
            this.chkDirectoriesLikeFiles.Size = new System.Drawing.Size(192, 24);
            this.chkDirectoriesLikeFiles.TabIndex = 0;
            this.chkDirectoriesLikeFiles.Text = "Treat empty directories like files";
            this.chkDirectoriesLikeFiles.CheckedChanged += new System.EventHandler(this.chkDirectoriesLikeFiles_CheckedChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.cmdExcludeRemove);
            this.tabPage4.Controls.Add(this.txtExcludeExt);
            this.tabPage4.Controls.Add(this.lblExclude);
            this.tabPage4.Controls.Add(this.cmdAddAll);
            this.tabPage4.Controls.Add(this.lblWildcards);
            this.tabPage4.Controls.Add(this.cmdRemoveAll);
            this.tabPage4.Controls.Add(this.cmdRemoveExt);
            this.tabPage4.Controls.Add(this.cmdAddExt);
            this.tabPage4.Controls.Add(this.tvExt);
            this.tabPage4.Controls.Add(this.txtExt);
            this.tabPage4.Controls.Add(this.lblExtensions);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(536, 390);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "File extensions";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // cmdExcludeRemove
            // 
            this.cmdExcludeRemove.Location = new System.Drawing.Point(8, 350);
            this.cmdExcludeRemove.Name = "cmdExcludeRemove";
            this.cmdExcludeRemove.Size = new System.Drawing.Size(80, 20);
            this.cmdExcludeRemove.TabIndex = 11;
            this.cmdExcludeRemove.Text = "Remove all";
            this.cmdExcludeRemove.Click += new System.EventHandler(this.cmdExcludeRemove_Click);
            // 
            // txtExcludeExt
            // 
            this.txtExcludeExt.Location = new System.Drawing.Point(8, 268);
            this.txtExcludeExt.Multiline = true;
            this.txtExcludeExt.Name = "txtExcludeExt";
            this.txtExcludeExt.Size = new System.Drawing.Size(296, 76);
            this.txtExcludeExt.TabIndex = 10;
            this.txtExcludeExt.TextChanged += new System.EventHandler(this.txtExcludeExt_TextChanged);
            this.txtExcludeExt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExcludeExt_KeyPress);
            // 
            // lblExclude
            // 
            this.lblExclude.AutoSize = true;
            this.lblExclude.Enabled = false;
            this.lblExclude.Location = new System.Drawing.Point(8, 252);
            this.lblExclude.Name = "lblExclude";
            this.lblExclude.Size = new System.Drawing.Size(173, 13);
            this.lblExclude.TabIndex = 9;
            this.lblExclude.Text = "Exclude files with these extensions:";
            // 
            // cmdAddAll
            // 
            this.cmdAddAll.Location = new System.Drawing.Point(94, 195);
            this.cmdAddAll.Name = "cmdAddAll";
            this.cmdAddAll.Size = new System.Drawing.Size(80, 20);
            this.cmdAddAll.TabIndex = 8;
            this.cmdAddAll.Text = "Add all (*)";
            this.cmdAddAll.Click += new System.EventHandler(this.cmdAddAll_Click);
            // 
            // lblWildcards
            // 
            this.lblWildcards.AutoSize = true;
            this.lblWildcards.Location = new System.Drawing.Point(8, 179);
            this.lblWildcards.Name = "lblWildcards";
            this.lblWildcards.Size = new System.Drawing.Size(192, 13);
            this.lblWildcards.TabIndex = 7;
            this.lblWildcards.Text = "Hint: * and ? can be used as wildcards.";
            // 
            // cmdRemoveAll
            // 
            this.cmdRemoveAll.Location = new System.Drawing.Point(8, 195);
            this.cmdRemoveAll.Name = "cmdRemoveAll";
            this.cmdRemoveAll.Size = new System.Drawing.Size(80, 20);
            this.cmdRemoveAll.TabIndex = 6;
            this.cmdRemoveAll.Text = "Remove all";
            this.cmdRemoveAll.Click += new System.EventHandler(this.cmdRemoveAll_Click);
            // 
            // cmdRemoveExt
            // 
            this.cmdRemoveExt.Location = new System.Drawing.Point(472, 352);
            this.cmdRemoveExt.Name = "cmdRemoveExt";
            this.cmdRemoveExt.Size = new System.Drawing.Size(56, 20);
            this.cmdRemoveExt.TabIndex = 5;
            this.cmdRemoveExt.Text = "Remove";
            this.cmdRemoveExt.Click += new System.EventHandler(this.cmdRemoveExt_Click);
            // 
            // cmdAddExt
            // 
            this.cmdAddExt.Location = new System.Drawing.Point(424, 352);
            this.cmdAddExt.Name = "cmdAddExt";
            this.cmdAddExt.Size = new System.Drawing.Size(40, 20);
            this.cmdAddExt.TabIndex = 4;
            this.cmdAddExt.Text = "Add";
            this.cmdAddExt.Click += new System.EventHandler(this.cmdAddExt_Click);
            // 
            // tvExt
            // 
            this.tvExt.ImageIndex = 0;
            this.tvExt.ImageList = this.imageList;
            this.tvExt.Location = new System.Drawing.Point(336, 48);
            this.tvExt.Name = "tvExt";
            this.tvExt.SelectedImageIndex = 0;
            this.tvExt.Size = new System.Drawing.Size(192, 296);
            this.tvExt.TabIndex = 3;
            this.tvExt.DoubleClick += new System.EventHandler(this.tvExt_DoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "");
            this.imageList.Images.SetKeyName(3, "");
            // 
            // txtExt
            // 
            this.txtExt.Location = new System.Drawing.Point(8, 48);
            this.txtExt.Multiline = true;
            this.txtExt.Name = "txtExt";
            this.txtExt.Size = new System.Drawing.Size(296, 128);
            this.txtExt.TabIndex = 2;
            this.txtExt.TextChanged += new System.EventHandler(this.txtExt_TextChanged);
            this.txtExt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExt_KeyPress);
            // 
            // lblExtensions
            // 
            this.lblExtensions.Location = new System.Drawing.Point(8, 16);
            this.lblExtensions.Name = "lblExtensions";
            this.lblExtensions.Size = new System.Drawing.Size(248, 32);
            this.lblExtensions.TabIndex = 1;
            this.lblExtensions.Text = "Compare/sync files with these extensions only: (e.g.: txt; doc; xls; ppt)";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkBackup);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(536, 390);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Actions";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkBackup
            // 
            this.chkBackup.Location = new System.Drawing.Point(16, 240);
            this.chkBackup.Name = "chkBackup";
            this.chkBackup.Size = new System.Drawing.Size(504, 32);
            this.chkBackup.TabIndex = 5;
            this.chkBackup.Text = "Only process source files with archive flag set and reset archiv flag after proce" +
    "ssing.";
            this.chkBackup.CheckedChanged += new System.EventHandler(this.chkBackup_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cbTemplates);
            this.groupBox7.Location = new System.Drawing.Point(16, 280);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(504, 56);
            this.groupBox7.TabIndex = 4;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Templates:";
            // 
            // cbTemplates
            // 
            this.cbTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTemplates.Items.AddRange(new object[] {
            "custom",
            "Mirror source to destination",
            "Mirror destination to source",
            "Source <=> Destination",
            "Backup source to destination"});
            this.cbTemplates.Location = new System.Drawing.Point(24, 24);
            this.cbTemplates.Name = "cbTemplates";
            this.cbTemplates.Size = new System.Drawing.Size(464, 21);
            this.cbTemplates.TabIndex = 0;
            this.cbTemplates.SelectedIndexChanged += new System.EventHandler(this.cbTemplates_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.chkExecAtDblClick);
            this.tabPage5.Controls.Add(this.cmdDefaults);
            this.tabPage5.Controls.Add(this.cmdContextUpdate);
            this.tabPage5.Controls.Add(this.cmdContextRemove);
            this.tabPage5.Controls.Add(this.cmdContextAdd);
            this.tabPage5.Controls.Add(this.label3);
            this.tabPage5.Controls.Add(this.cmdBrowse);
            this.tabPage5.Controls.Add(this.txtContextFile);
            this.tabPage5.Controls.Add(this.txtContextCaption);
            this.tabPage5.Controls.Add(this.label2);
            this.tabPage5.Controls.Add(this.label1);
            this.tabPage5.Controls.Add(this.lstContext);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(536, 390);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Context menu";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // chkExecAtDblClick
            // 
            this.chkExecAtDblClick.AutoSize = true;
            this.chkExecAtDblClick.Location = new System.Drawing.Point(19, 266);
            this.chkExecAtDblClick.Name = "chkExecAtDblClick";
            this.chkExecAtDblClick.Size = new System.Drawing.Size(243, 17);
            this.chkExecAtDblClick.TabIndex = 11;
            this.chkExecAtDblClick.Text = "Start executable when files are double-clicked";
            this.chkExecAtDblClick.UseVisualStyleBackColor = true;
            this.chkExecAtDblClick.CheckedChanged += new System.EventHandler(this.chkExecAtDblClick_CheckedChanged);
            // 
            // cmdDefaults
            // 
            this.cmdDefaults.Location = new System.Drawing.Point(439, 150);
            this.cmdDefaults.Name = "cmdDefaults";
            this.cmdDefaults.Size = new System.Drawing.Size(75, 23);
            this.cmdDefaults.TabIndex = 10;
            this.cmdDefaults.Text = "add defaults";
            this.cmdDefaults.UseVisualStyleBackColor = true;
            this.cmdDefaults.Click += new System.EventHandler(this.cmdDefaults_Click);
            // 
            // cmdContextUpdate
            // 
            this.cmdContextUpdate.Location = new System.Drawing.Point(181, 305);
            this.cmdContextUpdate.Name = "cmdContextUpdate";
            this.cmdContextUpdate.Size = new System.Drawing.Size(75, 23);
            this.cmdContextUpdate.TabIndex = 9;
            this.cmdContextUpdate.Text = "Update";
            this.cmdContextUpdate.UseVisualStyleBackColor = true;
            this.cmdContextUpdate.Click += new System.EventHandler(this.cmdContextUpdate_Click);
            // 
            // cmdContextRemove
            // 
            this.cmdContextRemove.Location = new System.Drawing.Point(100, 305);
            this.cmdContextRemove.Name = "cmdContextRemove";
            this.cmdContextRemove.Size = new System.Drawing.Size(75, 23);
            this.cmdContextRemove.TabIndex = 8;
            this.cmdContextRemove.Text = "Remove";
            this.cmdContextRemove.UseVisualStyleBackColor = true;
            this.cmdContextRemove.Click += new System.EventHandler(this.cmdContextRemove_Click);
            // 
            // cmdContextAdd
            // 
            this.cmdContextAdd.Location = new System.Drawing.Point(19, 305);
            this.cmdContextAdd.Name = "cmdContextAdd";
            this.cmdContextAdd.Size = new System.Drawing.Size(75, 23);
            this.cmdContextAdd.TabIndex = 7;
            this.cmdContextAdd.Text = "Add";
            this.cmdContextAdd.UseVisualStyleBackColor = true;
            this.cmdContextAdd.Click += new System.EventHandler(this.cmdContextAdd_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(302, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "These entries are added to the context menu of files that differ:";
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(481, 229);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(33, 20);
            this.cmdBrowse.TabIndex = 5;
            this.cmdBrowse.Text = "...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // txtContextFile
            // 
            this.txtContextFile.Location = new System.Drawing.Point(19, 229);
            this.txtContextFile.Name = "txtContextFile";
            this.txtContextFile.Size = new System.Drawing.Size(456, 20);
            this.txtContextFile.TabIndex = 4;
            this.txtContextFile.TextChanged += new System.EventHandler(this.txtContextFile_TextChanged);
            // 
            // txtContextCaption
            // 
            this.txtContextCaption.Location = new System.Drawing.Point(19, 174);
            this.txtContextCaption.Name = "txtContextCaption";
            this.txtContextCaption.Size = new System.Drawing.Size(191, 20);
            this.txtContextCaption.TabIndex = 3;
            this.txtContextCaption.TextChanged += new System.EventHandler(this.txtContextCaption_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path to executable:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Caption:";
            // 
            // lstContext
            // 
            this.lstContext.FormattingEnabled = true;
            this.lstContext.Location = new System.Drawing.Point(19, 36);
            this.lstContext.Name = "lstContext";
            this.lstContext.Size = new System.Drawing.Size(495, 108);
            this.lstContext.TabIndex = 0;
            this.lstContext.SelectedIndexChanged += new System.EventHandler(this.lstContext_SelectedIndexChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "exe";
            this.openFileDialog.Filter = "Executable files (*.exe)|*.exe|all files (*.*)|*.*";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(562, 464);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SettingsForm_HelpButtonClicked);
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Set all controls in the configuration dialog to reflect the current settings.
		/// </summary>
		private void SetControls()
		{
			switch (settings.SourceNewer)
			{
				case 1:
					rbSourceNewer1.Checked = true;
					break;
				case 2:
					rbSourceNewer2.Checked = true;
					break;
				case 3:
					rbSourceNewer3.Checked = true;
					break;
			}
			
			switch (settings.DestNewer)
			{
				case 1:
					rbDestNewer1.Checked = true;
					break;
				case 2:
					rbDestNewer2.Checked = true;
					break;
				case 3:
					rbDestNewer3.Checked = true;
					break;
			}

			switch (settings.SourceMissing)
			{
				case 1:
					rbSourceMissing1.Checked = true;
					break;
				case 2:
					rbSourceMissing2.Checked = true;
					break;
				case 3:
					rbSourceMissing3.Checked = true;
					break;
			}

			switch (settings.DestMissing)
			{
				case 1:
					rbDestMissing1.Checked = true;
					break;
				case 2:
					rbDestMissing2.Checked = true;
					break;
				case 3:
					rbDestMissing3.Checked = true;
					break;
			}

			chkBackup.Checked = settings.BackupMode;
			cbTemplates.SelectedIndex = settings.Template;

			chkDirectoriesLikeFiles.Checked = settings.DirectoriesLikeFiles;
			chkSubDirectories.Checked = settings.SubDirectories;
			chkHiddenFiles.Checked = settings.ProcessHiddenFiles;
			chkSystemFiles.Checked = settings.ProcessSystemFiles;

			txtExt.Text = settings.ExtensionList;
			txtExcludeExt.Text = settings.ExcludedExtensionList;

			chkShowLogWindow.Checked = settings.ShowLogWindow;
			chkSaveLog.Checked = settings.SaveLog;
			cbLogFile.Enabled = chkSaveLog.Checked;
			optAppend.Checked = settings.AppendLog;
			optOverwrite.Checked = !settings.AppendLog;
			optAppend.Enabled = chkSaveLog.Checked;
			optOverwrite.Enabled = chkSaveLog.Checked;

			if (settings.LogFile == "1")
			{
				cbLogFile.SelectedIndex = 0;
			}
			else if (settings.LogFile == "2")
			{
				cbLogFile.SelectedIndex = 1;
			}
			else
			{
				cbLogFile.Text = settings.LogFile;
			}

			chkReadOnly.Checked = settings.RemoveReadOnlyFlag;
			chkFileAss.Checked = settings.FileAssociation;
			optDblClickRun.Checked = settings.DoubleClickRunsJob;
			optDblClickOpen.Checked = !settings.DoubleClickRunsJob;
			optDblClickRun.Enabled = chkFileAss.Checked;
			optDblClickOpen.Enabled = chkFileAss.Checked;
			chkTerminateApp.Checked = settings.TerminateApp;
			chkTerminateApp.Enabled = chkFileAss.Checked;
			chkSilent.Checked = settings.RunSilent;
			chkSilent.Enabled = chkTerminateApp.Checked && chkTerminateApp.Enabled;

			lstContext.Items.Clear();
			foreach (string cme in settings.ContextMenu)
			{
				lstContext.Items.Add(cme);
			}

			// enable/disable controls in "file extension" tab
			SetExtensionControls();

			// enable/disable controls in "context menu" tab
			contextDblClickIdx = settings.ContextDblClickIdx;
			SetContextControls();
		}

		/// <summary>
		/// Read the state of all controls and store them in the current settings class.
		/// </summary>
		private void GetControls()
		{
            if (rbSourceNewer1.Checked)
            {
                settings.SourceNewer = 1;
            }
            else if (rbSourceNewer2.Checked)
            {
                settings.SourceNewer = 2;
            }
            else if (rbSourceNewer3.Checked)
            {
                settings.SourceNewer = 3;
            }

            if (rbDestNewer1.Checked)
            {
                settings.DestNewer = 1;
            }
            else if (rbDestNewer2.Checked)
            {
                settings.DestNewer = 2;
            }
            else if (rbDestNewer3.Checked)
            {
                settings.DestNewer = 3;
            }

            if (rbSourceMissing1.Checked)
            {
                settings.SourceMissing = 1;
            }
            else if (rbSourceMissing2.Checked)
            {
                settings.SourceMissing = 2;
            }
            else if (rbSourceMissing3.Checked)
            {
                settings.SourceMissing = 3;
            }

            if (rbDestMissing1.Checked)
            {
                settings.DestMissing = 1;
            }
            else if (rbDestMissing2.Checked)
            {
                settings.DestMissing = 2;
            }
            else if (rbDestMissing3.Checked)
            {
                settings.DestMissing = 3;
            }

			settings.RunSilent = chkSilent.Checked;
			settings.TerminateApp = chkTerminateApp.Checked;
			settings.BackupMode = chkBackup.Checked;
			settings.Template = cbTemplates.SelectedIndex;

			settings.DirectoriesLikeFiles = chkDirectoriesLikeFiles.Checked;
			settings.SubDirectories = chkSubDirectories.Checked;
			settings.ProcessHiddenFiles = chkHiddenFiles.Checked;
			settings.ProcessSystemFiles = chkSystemFiles.Checked;

			// format list of extensions
			settings.ExtensionList = FormatExtensionList(txtExt.Text);
			settings.ExcludedExtensionList = FormatExtensionList(txtExcludeExt.Text);

			settings.ShowLogWindow = chkShowLogWindow.Checked;
			settings.SaveLog = chkSaveLog.Checked;
			settings.AppendLog = optAppend.Checked;
			if (cbLogFile.SelectedIndex == 0)
			{
				settings.LogFile = "1";
			}
			else if (cbLogFile.SelectedIndex == 1)
			{
				settings.LogFile = "2";
			}
			else
			{
				settings.LogFile = cbLogFile.Text;
			}
			settings.RemoveReadOnlyFlag = chkReadOnly.Checked;
			settings.FileAssociation = chkFileAss.Checked;
			settings.DoubleClickRunsJob = optDblClickRun.Checked;

			settings.ContextMenu.Clear();
			foreach (string cme in lstContext.Items)
			{
				settings.ContextMenu.Add(cme);
			}
			settings.ContextDblClickIdx = contextDblClickIdx;
		}

		#region Event-Handler

		private void cmdOk_Click(object sender, System.EventArgs e)
		{
			// read settings from controls and update settings object
			GetControls();
			settings.Apply();

			// hide dialog
			this.DialogResult = DialogResult.OK;
			this.Hide();
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			// hide dialog without saving something
			Changed = false;
			this.DialogResult = DialogResult.Cancel;
			this.Hide();
		}

		private void SettingsForm_Shown(object sender, EventArgs e)
		{
			// load file extensions and initialize controls
			LoadFileExtensions();
			SetControls();
			Changed = false;
		}

		#region Tab "General"

		private void chkSaveLog_CheckedChanged(object sender, System.EventArgs e)
		{
			cbLogFile.Enabled = chkSaveLog.Checked;
			optAppend.Enabled = chkSaveLog.Checked;
			optOverwrite.Enabled = chkSaveLog.Checked;
		}

		private void chkFileAss_CheckedChanged(object sender, System.EventArgs e)
		{
			chkTerminateApp.Enabled = chkFileAss.Checked;
			chkSilent.Enabled = chkTerminateApp.Checked && chkTerminateApp.Enabled;
			optDblClickRun.Enabled = chkFileAss.Checked;
			optDblClickOpen.Enabled = chkFileAss.Checked;
			CheckAdmin();
		}

        private void optDblClickRun_CheckedChanged(object sender, EventArgs e)
        {
            CheckAdmin();
        }

        private void chkTerminateApp_CheckedChanged(object sender, System.EventArgs e)
		{
			chkSilent.Enabled = chkTerminateApp.Checked && chkTerminateApp.Enabled;
		}

		#endregion

		#region Tab "Filter options"

		private void chkDirectoriesLikeFiles_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void chkSubDirectories_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void chkHiddenFiles_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void chkSystemFiles_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		#endregion

		#region Tab "File extensions"

		private void txtExt_TextChanged(object sender, System.EventArgs e)
		{
			Changed = true;
			SetExtensionControls();
		}

		private void txtExt_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
            if ("\\/:.\"<>|".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = true;
            }
		}

		private void tvExt_DoubleClick(object sender, System.EventArgs e)
		{
			AddCurrentNode();
		}

		private void cmdRemoveAll_Click(object sender, System.EventArgs e)
		{
			txtExt.Text = "";
		}

		private void cmdAddAll_Click(object sender, EventArgs e)
		{
			txtExt.Text = "*";
		}

		private void cmdAddExt_Click(object sender, System.EventArgs e)
		{
			AddCurrentNode();
		}

		private void cmdRemoveExt_Click(object sender, System.EventArgs e)
		{
			RemoveCurrentNode();
		}

		private void txtExcludeExt_KeyPress(object sender, KeyPressEventArgs e)
		{
            if ("\\/:.\"<>|".IndexOf(e.KeyChar) != -1)
            {
                e.Handled = true;
            }
		}

		private void txtExcludeExt_TextChanged(object sender, EventArgs e)
		{
			Changed = true;
			SetExtensionControls();
		}

		private void cmdExcludeRemove_Click(object sender, EventArgs e)
		{
			txtExcludeExt.Text = "";
		}

		#endregion

		#region Tab "Actions"

		private void rbSourceNewer1_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbSourceNewer2_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbSourceNewer3_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbSourceMissing1_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbSourceMissing2_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbSourceMissing3_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbDestNewer1_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbDestNewer2_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbDestNewer3_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbDestMissing1_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbDestMissing2_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void rbDestMissing3_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void chkBackup_CheckedChanged(object sender, System.EventArgs e)
		{
			Changed = true;
		}

		private void cbTemplates_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			rbSourceNewer1.Enabled = (cbTemplates.SelectedIndex == 0);
			rbSourceNewer2.Enabled = (cbTemplates.SelectedIndex == 0);
			rbSourceNewer3.Enabled = (cbTemplates.SelectedIndex == 0);
			rbSourceMissing1.Enabled = (cbTemplates.SelectedIndex == 0);
			rbSourceMissing2.Enabled = (cbTemplates.SelectedIndex == 0);
			rbSourceMissing3.Enabled = (cbTemplates.SelectedIndex == 0);
			rbDestNewer1.Enabled = (cbTemplates.SelectedIndex == 0);
			rbDestNewer2.Enabled = (cbTemplates.SelectedIndex == 0);
			rbDestNewer3.Enabled = (cbTemplates.SelectedIndex == 0);
			rbDestMissing1.Enabled = (cbTemplates.SelectedIndex == 0);
			rbDestMissing2.Enabled = (cbTemplates.SelectedIndex == 0);
			rbDestMissing3.Enabled = (cbTemplates.SelectedIndex == 0);
			chkBackup.Enabled = (cbTemplates.SelectedIndex == 0);

			switch (cbTemplates.SelectedIndex)
			{
				case 1:
					rbSourceNewer1.Checked = true;
					rbSourceMissing2.Checked = true;
					rbDestNewer2.Checked = true;
					rbDestMissing1.Checked = true;
					chkBackup.Checked = false;
					break;
				case 2:
					rbSourceNewer2.Checked = true;
					rbSourceMissing1.Checked = true;
					rbDestNewer1.Checked = true;
					rbDestMissing2.Checked = true;
					chkBackup.Checked = false;
					break;
				case 3:
					rbSourceNewer1.Checked = true;
					rbSourceMissing1.Checked = true;
					rbDestNewer1.Checked = true;
					rbDestMissing1.Checked = true;
					chkBackup.Checked = false;
					break;
				case 4:
					rbSourceNewer1.Checked = true;
					rbSourceMissing3.Checked = true;
					rbDestNewer3.Checked = true;
					rbDestMissing1.Checked = true;
					chkBackup.Checked = true;
					break;
			}
		}

		#endregion

		#region Tab "Context menu"

		private void cmdContextAdd_Click(object sender, EventArgs e)
		{
			lstContext.Items.Add(txtContextCaption.Text + "\t" + txtContextFile.Text);
			txtContextCaption.Text = "";
			txtContextFile.Text = "";
			if (chkExecAtDblClick.Checked)
			{
				contextDblClickIdx = lstContext.Items.Count - 1;
			}
		}

		private void lstContext_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstContext.SelectedIndex != -1)
			{
				string[] item = lstContext.SelectedItem.ToString().Split('\t');
				txtContextCaption.Text = item[0];
				txtContextFile.Text = item[1];
				chkExecAtDblClick.Checked = (lstContext.SelectedIndex == contextDblClickIdx);
			}
			SetContextControls();
		}

		private void cmdContextRemove_Click(object sender, EventArgs e)
		{
			if (lstContext.SelectedIndex != -1)
			{
				if (lstContext.SelectedIndex == contextDblClickIdx)
				{
					contextDblClickIdx = -1;
				}
				lstContext.Items.RemoveAt(lstContext.SelectedIndex);
			}
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			if (File.Exists(txtContextFile.Text))
			{
				openFileDialog.InitialDirectory = Path.GetDirectoryName(txtContextFile.Text);
			}
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				txtContextFile.Text = openFileDialog.FileName;
				if (string.IsNullOrEmpty(txtContextCaption.Text))
				{
					txtContextCaption.Text = Path.GetFileName(openFileDialog.FileName);
				}
			}
		}

		private void cmdContextUpdate_Click(object sender, EventArgs e)
		{
			if (lstContext.SelectedIndex != -1)
			{
				if (chkExecAtDblClick.Checked)
				{
					contextDblClickIdx = lstContext.SelectedIndex;
				}
				else if (lstContext.SelectedIndex == contextDblClickIdx)
				{
					contextDblClickIdx = -1;
				}
				lstContext.Items[lstContext.SelectedIndex] = txtContextCaption.Text + "\t" + txtContextFile.Text;
			}
		}

		private void txtContextCaption_TextChanged(object sender, EventArgs e)
		{
			SetContextControls();
		}

		private void txtContextFile_TextChanged(object sender, EventArgs e)
		{
			SetContextControls();
		}

		private void chkExecAtDblClick_CheckedChanged(object sender, EventArgs e)
		{
			SetContextControls();
		}

		/// <summary>
		/// Search for some default applications and add them to the list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdDefaults_Click(object sender, EventArgs e)
		{
			// add items to temporary list
			List<string> tempList = new List<string>();
			foreach (string cme in lstContext.Items)
			{
				tempList.Add(cme);
			}

			// search for default applications and add them to the list
			settings.SetContextMenuDefaults(tempList);

			// copy back into list control
			lstContext.Items.Clear();
			foreach (string cme in tempList)
			{
				lstContext.Items.Add(cme);
			}
		}

		#endregion

		#endregion

		/// <summary>
		/// Parse an XML node containing file extensions.
		/// </summary>
		/// <param name="node">XmlNode class to parse</param>
		/// <returns>TreeNode of ListView</returns>
		private TreeNode ParseExtensionNode(XmlNode node)
		{
			TreeNode tn = null;
			XmlAttribute attrName = node.Attributes["name"];
			if (attrName != null)
			{
				string name = attrName.Value;
				XmlAttribute attrList = node.Attributes["list"];
				if (attrList != null)
				{
					name += " (" + attrList.Value + ")";
					tn = new TreeNode(name, 2, 3);
				} 
				else 
				{
					tn = new TreeNode(name, 0, 1);
				}				
				foreach (XmlNode child in node.ChildNodes)
				{
					TreeNode tnChild = ParseExtensionNode(child);
					if (tnChild != null)
					{
						tn.Nodes.Add(tnChild);
					}
				}
			}
			return tn;
		}

		/// <summary>
		/// Load a list of file extensions from an XML file called "FileExtensions.xml"
		/// which must be in the same directory as the application's executable.
		/// </summary>
		private void LoadFileExtensions()
		{
			tvExt.Nodes.Clear();

			string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "FileExtensions.xml");
			if (File.Exists(path))
			{		
				XmlDocument	doc = new XmlDocument();
				try
				{
					doc.Load(path);

					XmlNodeList nodes = doc.SelectNodes("InZync/extensions/node");
					foreach (XmlNode node in nodes)
					{
						TreeNode tn = ParseExtensionNode(node);
						if (tn != null)
						{
							tvExt.Nodes.Add(tn);
						}
					}
				}
				catch
				{
                    // ignore
                }
			}	
		}

		/// <summary>
		/// Extract all extensions from a single node and return them as a string.
		/// </summary>
		/// <param name="node">TreeNode to parse</param>
		/// <returns></returns>
		private string AddExtNode(TreeNode node)
		{
			string list = node.Text;
			int i = list.IndexOf("(");
			if (i >= 0)
			{
				list = list.Substring(i + 1);
				list = list.Replace(")", "").Replace(" ", "");
			}
			return list;
		}

		/// <summary>
		/// Extract all extensions from a single node or a node containing subnodes
		/// and return them as a string array.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private string[] GetListFromNode(TreeNode node)
		{
			string list = "";
			if (node.Nodes.Count > 0)
			{
				// group of extensions
				foreach (TreeNode child in node.Nodes)
				{
					list += AddExtNode(child);
				}
			} 
			else 
			{
				// single extension
				list += AddExtNode(node);
			}
			if (list.EndsWith(";"))
			{
				list = list.Substring(0, list.Length - 1);
			}
			return list.Split(new char[] {';'});
		}

		/// <summary>
		/// All extensions from the selected node in the ListView are added to the
		/// extension textbox.
		/// </summary>
		private void AddCurrentNode()
		{
			txtExt.Text = txtExt.Text.Trim();
			if (txtExt.Text != "")
			{
				if (!txtExt.Text.EndsWith(";"))
				{
					txtExt.Text += ";";
				}
			}

			TreeNode node = tvExt.SelectedNode;
			if (node != null)
			{
				string[] exts = GetListFromNode(node);
				foreach (string ext in exts)
				{
					string list = " " + txtExt.Text;
					if (list.IndexOf(" " + ext + ";") == -1)
					{
						txtExt.Text += " " + ext + ";";
					}
				}
				if (txtExt.Text.StartsWith(" "))
				{
					txtExt.Text = txtExt.Text.Substring(1);
				}
			}
		}

		/// <summary>
		/// All extensions from the selected node in the ListView are removed from the
		/// extension textbox.
		/// </summary>
		private void RemoveCurrentNode()
		{
			TreeNode node = tvExt.SelectedNode;
			if (node != null)
			{
				string list = " " + txtExt.Text;
				string[] exts = GetListFromNode(node);
				foreach (string ext in exts)
				{
					if (list.IndexOf(" " + ext + ";") != -1)
					{
						list = list.Replace(" " + ext + ";", "");
					}
				}
				if (list.StartsWith(" "))
				{
					list = list.Substring(1);
				}
				txtExt.Text = list;
			}
		}

		/// <summary>
		/// The given list containing file extensions is formatted. Extensions must be
		/// separated by ; characters.
		/// </summary>
		/// <param name="txt">text to format</param>
		/// <returns>formatted text</returns>
		private string FormatExtensionList(string txt)
		{
			string result = txt.Trim().Replace(" ", "");
			while (result.IndexOf(";;") != -1)
			{
				result = result.Replace(";;", "");
			}
			result = result.TrimStart(new char[] {';'});
			result = result.Replace(";", "; ");
			result = result.TrimEnd(new char[] {' ', ';'});
			return result;
		}

		/// <summary>
		/// Open online help for the "Settings" dialog in the web browser.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SettingsForm_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			string commandText = "https://github.com/b43r/inzync";
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo.FileName = commandText;
			proc.StartInfo.Verb = "open";
			proc.StartInfo.UseShellExecute = true;
			proc.Start();
		}

		/// <summary>
		/// Enable/disable controls on the "file extension" tab depending on
		/// their content.
		/// </summary>
		private void SetExtensionControls()
		{
			cmdAddAll.Enabled = (txtExt.Text != "*");
			cmdRemoveAll.Enabled = (txtExt.Text != "");
			txtExcludeExt.Enabled = (txtExt.Text == "*");
			lblExclude.Enabled = txtExcludeExt.Enabled;
			cmdExcludeRemove.Enabled = (txtExcludeExt.Text != "") && (txtExcludeExt.Enabled);
		}

		/// <summary>
		///  Enable/disable controls on the "context menu" tab depending on
		/// </summary>
		private void SetContextControls()
		{
			cmdContextAdd.Enabled = (txtContextCaption.Text != "") && (txtContextFile.Text != "");
			cmdContextRemove.Enabled = (lstContext.SelectedIndex != -1);
			cmdContextUpdate.Enabled = ContextSelectionDifferent();
		}

		/// <summary>
		/// Check whether the texts entered in the context menu tab differ
		/// to the selected item in the listbox.
		/// </summary>
		/// <returns>true if text differs</returns>
		private bool ContextSelectionDifferent()
		{
			if (lstContext.SelectedIndex != -1)
			{
				string[] item = lstContext.SelectedItem.ToString().Split('\t');
				return (txtContextCaption.Text != item[0]) ||
					(txtContextFile.Text != item[1]) ||
					(chkExecAtDblClick.Checked && (contextDblClickIdx != lstContext.SelectedIndex)) ||
					(!chkExecAtDblClick.Checked && (contextDblClickIdx == lstContext.SelectedIndex));
			}
			return false;
		}

        /// <summary>
        /// Show/hide admin privilege shield on "OK" button.
        /// </summary>
        private void CheckAdmin()
        {
            if (!isAdmin)
            {
                if (settings.FileAssociation != chkFileAss.Checked || settings.DoubleClickRunsJob != optDblClickRun.Checked)
                {
                    // draw shield
                    cmdOk.FlatStyle = FlatStyle.System;
                    SendMessage(cmdOk.Handle, BCM_SETSHIELD, 0, (IntPtr)1);
                }
                else
                {
                    // remove shield
                    cmdOk.FlatStyle = FlatStyle.Standard;
                    SendMessage(cmdOk.Handle, BCM_SETSHIELD, 0, (IntPtr)0);
                }
            }
        }
    }
}
