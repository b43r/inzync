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
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;

namespace InZync
{
    /// <summary>
    /// This class stores all settings.
    /// </summary>
    public class Settings
    {
        private string extensions;
        private string excludedExtensions;
        private string logFile;

        private bool oldDoubleClickRunsJob;
        private bool oldFileAssociation;

        /// <summary>
        /// Constructor, read some settings from the registry.
        /// </summary>
        public Settings()
        {
            LoadRegistrySettings();

            oldDoubleClickRunsJob = DoubleClickRunsJob;
            oldFileAssociation = FileAssociation;            
        }

        /// <summary>
        /// Load all settings stored in the registry.
        /// </summary>
        public void LoadRegistrySettings()
        {
            FileAssociation = false;
            using (var subkey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".syncjob"))
            {
                if (subkey != null)
                {
                    FileAssociation = (string)subkey.GetValue("") == "InZync";
                }				
            }

            DoubleClickRunsJob = false;
            using (var subkey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("InZync\\shell\\open"))
            {
                if (subkey != null)
                {
                    DoubleClickRunsJob = (string)subkey.GetValue("MuiVerb") == "Run job";
                }
            }
        }

        #region Properties

        public int SourceNewer { get; set; }

        public int DestNewer { get; set; }

        public int SourceMissing { get; set; }

        public int DestMissing { get; set; }

        public bool BackupMode { get; set; }

        public int Template { get; set; }

        public bool DirectoriesLikeFiles { get; set; }

        public bool SubDirectories { get; set; }

        public string[] Extensions
        {
            get
            {
                return extensions.ToLower().Replace(" ", "").Split(';');
            }
        }

        public string ExtensionList
        {
            get { return extensions; }
            set { extensions = value.Trim(); }
        }

        public string[] ExcludedExtensions
        {
            get
            {
                return excludedExtensions.ToLower().Replace(" ", "").Split(';');
            }
        }

        public string ExcludedExtensionList
        {
            get { return excludedExtensions; }
            set { excludedExtensions = value.Trim(); }
        }

        public bool ProcessHiddenFiles { get; set; }

        public bool ProcessSystemFiles { get; set; }

        public bool ShowLogWindow { get; set; }

        public bool SaveLog { get; set; }

        public bool AppendLog { get; set; }

        public string LogFile
        {
            get
            {
                if (logFile == "1")
                {
                    return Path.Combine(Application.StartupPath, "InZync.log");
                }
                if (logFile == "2")
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "InZync.log");
                }
                return logFile;
            }
            set
            {
                if (value == Application.StartupPath)
                {
                    logFile = "1";
                }
                else if (value == Environment.GetFolderPath(Environment.SpecialFolder.Personal))
                {
                    logFile = "2";
                }
                else
                {
                    logFile = value;
                }
            }
        }

        public bool RemoveReadOnlyFlag { get; set; }

        public bool FileAssociation { get; set; }

        public bool TerminateApp { get; set; }

        public bool RunSilent { get; set; }

        public bool DoubleClickRunsJob { get; set; }

        public List<string> ContextMenu { get; set; } = new List<string>();

        public int ContextDblClickIdx { get; set; }

        public bool ShowOnlyDifferentFiles { get; set; }

        #endregion

        /// <summary>
        /// Applies the settings by storing some of them in the registry and
        /// set/remove file associations.
        /// </summary>
        public void Apply()
        {
            //
            // set/remove file association
            //
            if ((!oldFileAssociation) && (FileAssociation))
            {
                // file association enabled
                SetFileAssociation(IsAdmin(), DoubleClickRunsJob);
            }
            else if ((oldFileAssociation) && (!FileAssociation))
            {
                // file association disabled
                RemoveFileAssociation(IsAdmin());
            }
            else if ((oldFileAssociation) && (FileAssociation) &&
                (((oldDoubleClickRunsJob) && (!DoubleClickRunsJob)) ||
                ((!oldDoubleClickRunsJob) && (DoubleClickRunsJob))))
            {
                // doubleclick action changed
                SetFileAssociation(IsAdmin(), DoubleClickRunsJob);
            }

            oldFileAssociation = FileAssociation;
            oldDoubleClickRunsJob = DoubleClickRunsJob;
        }

        /// <summary>
        /// Remove the file association for the extension ".syncjob" with "InZync".
        /// </summary>
        /// <param name="isAdmin">whether the process is started with admin privileges</param>
        public static void RemoveFileAssociation(bool isAdmin)
        {
            if (isAdmin)
            {
                try
                {
                    Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(".syncjob");
                    Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree("InZync");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, $"Failed to remove file association: {ex.Message}", "InZync - Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // start a new instance with elevated privileges
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Application.ExecutablePath;
                proc.Arguments = "/removefileextension";
                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, $"Failed to remove file association: {ex.Message}", "InZync - Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Create a file association for the extension ".syncjob" with "InZync".
        /// </summary>
        /// <param name="isAdmin">whether the process is started with admin privileges</param>
        public static void SetFileAssociation(bool isAdmin, bool doubleClickRunsJob)
        {
            if (isAdmin)
            {
                try
                {
                    using (var subkey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(".syncjob"))
                    {
                        subkey?.SetValue("", "InZync");
                    }

                    using (var subkey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("InZync"))
                    {
                        if (subkey != null)
                        {
                            subkey.SetValue("", "Job for InZync");
                            using (var defaultIcon = subkey.CreateSubKey("DefaultIcon"))
                            {
                                defaultIcon?.SetValue("", Application.ExecutablePath + ",0");
                            }
                        }
                    }

                    using (var subkey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("InZync\\shell\\open"))
                    {
                        if (subkey != null)
                        {
                            subkey.SetValue("MuiVerb", doubleClickRunsJob ? "Run job" : "Open job with InZync");
                            using (var command = subkey.CreateSubKey("command"))
                            {
                                command?.SetValue("", Application.ExecutablePath + (doubleClickRunsJob ? " /job \"%1\" /run" : " /job \"%1\""));
                            }
                        }
                    }

                    using (var subkey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("InZync\\shell\\edit"))
                    {
                        if (subkey != null)
                        {
                            subkey.SetValue("MuiVerb", !doubleClickRunsJob ? "Run job" : "Open job with InZync");
                            using (var command = subkey.CreateSubKey("command"))
                            {
                                command?.SetValue("", Application.ExecutablePath + (!doubleClickRunsJob ? " /job \"%1\" /run" : " /job \"%1\""));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, $"Failed to set file association: {ex.Message}", "InZync - Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // start a new instance with elevated privileges
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Application.ExecutablePath;
                proc.Arguments = "/setfileextension:" + (doubleClickRunsJob ? "1" : "0");
                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, $"Failed to remove file association: {ex.Message}", "InZync - Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Save settings into the given file.
        /// </summary>
        /// <param name="fileName">file to save settings to</param>
        /// <param name="syncPathList">list with paths to synchronize</param>
        public void SaveJob(string fileName, SyncPathList syncPathList)
        {
            XmlAttribute attr;
            XmlElement root;
            XmlNode node;
            XmlDocument	doc = new XmlDocument();
            XmlNode xmlnode = doc.CreateNode(XmlNodeType.XmlDeclaration,"","");
            doc.AppendChild(xmlnode);

            root = doc.CreateElement("InZync");

            // save paths to synchronize
            node = doc.CreateElement("SourcePaths");
            foreach (PathPair pp in syncPathList)
            {
                XmlNode pathNode = doc.CreateElement("Path");
                attr = doc.CreateAttribute("source");
                attr.Value = pp.Source;
                pathNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("destination");
                attr.Value = pp.Destination;
                pathNode.Attributes.Append(attr);
                node.AppendChild(pathNode);
            }
            root.AppendChild(node);
        
            node = doc.CreateElement("ShowLog");
            node.InnerText = ShowLogWindow.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("SaveLog");
            node.InnerText = SaveLog.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("AppendLog");
            node.InnerText = AppendLog.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("LogFile");
            node.InnerText = LogFile;
            root.AppendChild(node);

            node = doc.CreateElement("ReadOnly");
            node.InnerText = RemoveReadOnlyFlag.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("DirectoriesLikeFiles");
            node.InnerText = DirectoriesLikeFiles.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("SubDirectories");
            node.InnerText = SubDirectories.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("HiddenFiles");
            node.InnerText = ProcessHiddenFiles.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("SystemFiles");
            node.InnerText = ProcessSystemFiles.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("TerminateApp");
            node.InnerText = TerminateApp.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("RunSilent");
            node.InnerText = RunSilent.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("Extensions");
            node.InnerText = ExtensionList;
            root.AppendChild(node);

            node = doc.CreateElement("ExcludedExtensions");
            node.InnerText = ExcludedExtensionList;
            root.AppendChild(node);

            node = doc.CreateElement("SourceNewer");
            node.InnerText = SourceNewer.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("SourceMissing");
            node.InnerText = SourceMissing.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("DestNewer");
            node.InnerText = DestNewer.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("DestMissing");
            node.InnerText = DestMissing.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("BackupMode");
            node.InnerText = BackupMode.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("Template");
            node.InnerText = Template.ToString();
            root.AppendChild(node);

            node = doc.CreateElement("ShowOnlyDifferentFiles");
            node.InnerText = ShowOnlyDifferentFiles.ToString();
            root.AppendChild(node);

            // save context menu entries
            node = doc.CreateElement("ContextMenuEntries");
            for (int i = 0; i < ContextMenu.Count; i++)
            {
                string[] cme = ContextMenu[i].Split('\t');
                XmlNode pathNode = doc.CreateElement("ContextMenuEntry");
                attr = doc.CreateAttribute("caption");
                attr.Value = cme[0];
                pathNode.Attributes.Append(attr);
                attr = doc.CreateAttribute("file");
                attr.Value = cme[1];
                pathNode.Attributes.Append(attr);

                if (i == ContextDblClickIdx)
                {
                    attr = doc.CreateAttribute("DblClick");
                    //attr.Value = cme[1];
                    pathNode.Attributes.Append(attr);
                }

                node.AppendChild(pathNode);
            }
            root.AppendChild(node);

            doc.AppendChild(root);			
            doc.Save(fileName);
        }

        /// <summary>
        /// Load settings from the given file.
        /// </summary>
        /// <param name="fileName">file to load settings from</param>
        /// <param name="syncPathList">list with paths to synchronize</param>
        public void LoadJob(string fileName, SyncPathList syncPathList)
        {
            XmlDocument	doc = new XmlDocument();
            doc.Load(fileName);

            syncPathList.Clear();
            ContextMenu.Clear();
            bool contextMenuRead = false;
            ContextDblClickIdx = -1;

            foreach (XmlNode node in doc.ChildNodes)
            {
                if ((node.NodeType == XmlNodeType.Element) && (node.Name == "InZync"))
                {
                    if (node.HasChildNodes)
                    {
                        foreach (XmlNode param in node.ChildNodes)
                        {
                            if (param.NodeType == XmlNodeType.Element)
                            {

                                // read paths to synchronize
                                if (param.Name == "SourcePaths")
                                {
                                    foreach (XmlNode pathNode in param.ChildNodes)
                                    {
                                        if (pathNode.Name == "Path")
                                        {
                                            PathPair pp = new PathPair();
                                            XmlAttribute attr = pathNode.Attributes["source"];
                                            if (attr != null)
                                            {
                                                pp.Source = attr.Value;
                                                attr = pathNode.Attributes["destination"];
                                                if (attr != null)
                                                {
                                                    pp.Destination = attr.Value;
                                                    syncPathList.Add(pp);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (param.Name == "ShowLog")
                                {
                                    ShowLogWindow = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "SaveLog")
                                {
                                    SaveLog = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "AppendLog")
                                {
                                    AppendLog = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "LogFile")
                                {
                                    LogFile = param.InnerText;
                                }
                                if (param.Name == "ReadOnly")
                                {
                                    RemoveReadOnlyFlag = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "DirectoriesLikeFiles")
                                {
                                    DirectoriesLikeFiles = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "SubDirectories")
                                {
                                    SubDirectories = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "HiddenFiles")
                                {
                                    ProcessHiddenFiles = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "SystemFiles")
                                {
                                    ProcessSystemFiles = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "TerminateApp")
                                {
                                    TerminateApp = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "RunSilent")
                                {
                                    RunSilent = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "Extensions")
                                {
                                    ExtensionList = param.InnerText;
                                }
                                if (param.Name == "ExcludedExtensions")
                                {
                                    ExcludedExtensionList = param.InnerText;
                                }
                                if (param.Name == "SourceNewer")
                                {
                                    SourceNewer = Int32.Parse(param.InnerText);
                                }
                                if (param.Name == "SourceMissing")
                                {
                                    SourceMissing = Int32.Parse(param.InnerText);
                                }
                                if (param.Name == "DestNewer")
                                {
                                    DestNewer = Int32.Parse(param.InnerText);
                                }
                                if (param.Name == "DestMissing")
                                {
                                    DestMissing = Int32.Parse(param.InnerText);
                                }
                                if (param.Name == "BackupMode")
                                {
                                    BackupMode = bool.Parse(param.InnerText);
                                }
                                if (param.Name == "Template")
                                {
                                    Template = Int32.Parse(param.InnerText);
                                }
                                if (param.Name == "ShowOnlyDifferentFiles")
                                {
                                    ShowOnlyDifferentFiles = bool.Parse(param.InnerText);
                                }

                                // read context menu entries
                                if (param.Name == "ContextMenuEntries")
                                {
                                    contextMenuRead = true;
                                    foreach (XmlNode pathNode in param.ChildNodes)
                                    {
                                        if (pathNode.Name == "ContextMenuEntry")
                                        {
                                            XmlAttribute attr = pathNode.Attributes["caption"];
                                            if (attr != null)
                                            {
                                                string cme = attr.Value;
                                                attr = pathNode.Attributes["file"];
                                                if (attr != null)
                                                {
                                                    cme += "\t" + attr.Value;
                                                    ContextMenu.Add(cme);
                                                    if (pathNode.Attributes["DblClick"] != null)
                                                    {
                                                        ContextDblClickIdx = ContextMenu.Count - 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!contextMenuRead)
            {
                SetContextMenuDefaults(ContextMenu);
            }
        }

        /// <summary>
        /// Test if some applications are installed and set appropriate
        /// context menu entries.
        /// </summary>
        public void SetContextMenuDefaults(List<string> contextItemList)
        {
            string path;

            ContextDblClickIdx = -1;

            // check for ExamDiff Pro, 1st try..
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ExamDiff Pro\\ExamDiff.exe");
            if (File.Exists(path))
            {
                AddContextItem(contextItemList, "ExamDiff Pro", path);
            }
            else
            {
                // 2nd try...
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ExamDiff Pro\\ExamDiff.exe");
                if (File.Exists(path))
                {
                    AddContextItem(contextItemList, "ExamDiff Pro", path);
                }
                else
                {
                    // 3rd try...
                    using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\PrestoSoft\\ExamDiff Pro\\Settings"))
                    {
                        if (reg != null)
                        {
                            path = reg.GetValue("Horz HTML Template", "").ToString();
                            if (path != "")
                            {
                                path = Path.GetDirectoryName(path);
                                path = Path.Combine(path, "ExamDiff.exe");
                                if (File.Exists(path))
                                {
                                    AddContextItem(contextItemList, "ExamDiff Pro", path);
                                }
                            }
                        }
                    }
                }
            }

            // check for WinMerge, 1st try...
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WinMerge\\WinMerge.exe");
            if (File.Exists(path))
            {
                AddContextItem(contextItemList, "WinMerge", path);
            }
            else
            {
                // 2nd try...
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "WinMerge\\WinMerge.exe");
                if (File.Exists(path))
                {
                    AddContextItem(contextItemList, "WinMerge", path);
                }
                else
                {
                    // 3rd try...
                    using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Thingamahoochie\\WinMerge"))
                    {
                        if (reg != null)
                        {
                            path = reg.GetValue("Executable", "").ToString();
                            if (path != "")
                            {
                                if (File.Exists(path))
                                {
                                    AddContextItem(contextItemList, "WinMerge", path);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new item to the context menu item list, but only if
        /// the file not already exists.
        /// </summary>
        /// <param name="contextItemList">list with context menu items</param>
        /// <param name="title">menu caption</param>
        /// <param name="path">file</param>
        private void AddContextItem(List<string> contextItemList, string title, string path)
        {
            // check if the path already exists in the list
            foreach (string item in contextItemList)
            {
                string[] arr = item.Split('\t');
                if (arr.Length == 2)
                {
                    if (arr[1].ToString().ToLower() == path.ToLower())
                    {
                        return;
                    }
                }
            }

            // add new entry
            contextItemList.Add(title + '\t' + path);			
        }

        /// <summary>
        /// Check whether the current user is admin.
        /// </summary>
        /// <returns>true if user is admin</returns>
        private static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
