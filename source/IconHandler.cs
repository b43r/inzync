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
using System.Runtime.InteropServices;
using System.Drawing;

namespace InZync
{
    /// <summary>
    /// This is a helper class for extracting icons from exe files.
    /// </summary>
    public class IconHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        private const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        /// <summary>
        /// Create an instance of the class and reads the main icon of the given file.
        /// The icon is then converted into a bitmap so it is available both as icon object
        /// and as bitmap object.
        /// </summary>
        /// <param name="file">file to read icon from</param>
        public IconHandler(string file)
        {
            if (System.IO.File.Exists(file))
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(file, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                FileIcon = Icon.FromHandle(shinfo.hIcon);
                FileBitmap = FileIcon.ToBitmap();
            }
        }

        /// <summary>
        /// Returns the icon.
        /// </summary>
        public Icon FileIcon { get; } = null;

        /// <summary>
        /// Returns the icon as a bitmap object.
        /// </summary>
        public Bitmap FileBitmap { get; } = null;
    }
}
