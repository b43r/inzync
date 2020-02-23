/*
 * InZync
 * 
 * Copyright (C) 2020 by Simon Baer
 * 
 * Based on code by Eddie Velasquez. Get original source at:
 * https://www.codeproject.com/Articles/2163/ListViewSortManager-control
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
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace InZync
{
    public delegate void ShowPopupMenuEventHandler(object sender, MouseEventArgs e);
    public delegate void HeaderMouseDownEventHandler(object sender, MouseEventArgs e);

    /// <summary>
    /// Zusammenfassung für SyncListView.
    /// </summary>
    public class SyncListView : ListView
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public event ShowPopupMenuEventHandler ShowPopupMenu;
        public event HeaderMouseDownEventHandler HeaderMouseDown;

        private HeaderControl headerControl;

        private bool useNativeArrows;
        private int m_column;
        private ImageList imgList;

        /// <summary>
        /// Sort the items in the ListView by the given column and order.
        /// </summary>
        /// <param name="column">column to sort after</param>
        /// <param name="order">order to sort</param>
        /// <param name="sizeColIdx">index of column containing file size</param>
        public void Sort(int column, SortOrder order, int sizeColIdx)
        {
            // show arrow
            ShowSortArrow(column, order);

            // create new sorter
            ListViewItemSorter = new ListViewItemComparer(column, (order == SortOrder.Ascending), sizeColIdx);

            // do sorting
            Sort();
        }

        /// <summary>
        /// Shows a little arrow in the column header denoting the sort column.
        /// </summary>
        /// <param name="column">Column to be sorted</param>
        /// <param name="order">Sort order</param>
        private void ShowSortArrow(int column, SortOrder order)
        {
            if (column != m_column)
            {
                ShowHeaderIcon(this, m_column, SortOrder.None);
                m_column = column;
            }

            ShowHeaderIcon(this, m_column, order);
        }

        /// <summary>
        /// As soon as the handle for the ListView object is created we query the
        /// version of comctrl.dll and create the HeaderControl object.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            // create arrow images if neccessary
            useNativeArrows = ComCtlDllSupportsArrows();
            if (!useNativeArrows)
            {
                imgList = new ImageList();
                imgList.ImageSize = new Size(8, 8);
                imgList.TransparentColor = Color.Magenta;

                imgList.Images.Add(GetArrowBitmap(ArrowType.Ascending));		// Add ascending arrow
                imgList.Images.Add(GetArrowBitmap(ArrowType.Descending));		// Add descending arrow

                SetHeaderImageList(this, imgList);
            }

            // create a new HeaderControl object
            headerControl = new HeaderControl(this);
            headerControl.MouseDown += new InZync.SyncListView.HeaderControl.MouseDownEventHandler(header_MouseDown);
            base.OnHandleCreated(e);
        }

        /// <summary>
        /// The right mouse button is catched.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0204) //WM_RBUTTONDOWN)
            {
                if (ShowPopupMenu != null)
                {
                    int x = Helper.LoWord((int)m.LParam);
                    int y = Helper.HiWord((int)m.LParam);
                    ShowPopupMenu(this, new MouseEventArgs(MouseButtons.None, 0, x, y, 0));
                }
                return;
            }

            if ((m.Msg == 0x0205) || (m.Msg == 0x0206)) // WM_RBUTTONUP || WM_RBUTTONDBLCLK
            {
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Header control fired a mousedown event which is now forwarded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void header_MouseDown(object sender, MouseEventArgs e)
        {
            HeaderMouseDown?.Invoke(this, e);
        }

        #region HeaderControl

        /// <summary>
        /// This object is used to override the WndProc of the header cotrol
        /// so we can trap the mouse-down event.
        /// </summary>
        internal class HeaderControl : NativeWindow
        {
            public delegate void MouseDownEventHandler(object sender, MouseEventArgs e);
            public event MouseDownEventHandler MouseDown;

            ListView parent;
            public HeaderControl(ListView m)
            {
                parent = m;
                //Get the header control handle
                IntPtr header = SendMessage(parent.Handle, (0x1000 + 31), IntPtr.Zero, IntPtr.Zero);
                AssignHandle(header);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x0204) //WM_RBUTTONDOWN)
                {
                    if (MouseDown != null)
                    {
                        int x = Helper.LoWord((int)m.LParam);
                        int y = Helper.HiWord((int)m.LParam);
                        MouseDown(this, new MouseEventArgs(MouseButtons.Right, 0, x, y, 0));
                    }
                    return;
                }
                base.WndProc(ref m);
            }
        }

        #endregion

        #region Graphics

        private enum ArrowType { Ascending, Descending }

        private Bitmap GetArrowBitmap(ArrowType type)
        {
            Bitmap bmp = new Bitmap(8, 8);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                Pen lightPen = SystemPens.ControlLightLight;
                Pen shadowPen = SystemPens.ControlDark;

                gfx.FillRectangle(System.Drawing.Brushes.Magenta, 0, 0, 8, 8);

                if (type == ArrowType.Ascending)
                {
                    gfx.DrawLine(lightPen, 0, 7, 7, 7);
                    gfx.DrawLine(lightPen, 7, 7, 4, 0);
                    gfx.DrawLine(shadowPen, 3, 0, 0, 7);
                }
                else if (type == ArrowType.Descending)
                {
                    gfx.DrawLine(lightPen, 4, 7, 7, 0);
                    gfx.DrawLine(shadowPen, 3, 7, 0, 0);
                    gfx.DrawLine(shadowPen, 0, 0, 7, 0);
                }
            }

            return bmp;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HDITEM
        {
            public Int32 mask;
            public Int32 cxy;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 lParam;
            public Int32 iImage;
            public Int32 iOrder;
        };

        [DllImport("user32", EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage2(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);

        const Int32 HDI_WIDTH = 0x0001;
        const Int32 HDI_HEIGHT = HDI_WIDTH;
        const Int32 HDI_TEXT = 0x0002;
        const Int32 HDI_FORMAT = 0x0004;
        const Int32 HDI_LPARAM = 0x0008;
        const Int32 HDI_BITMAP = 0x0010;
        const Int32 HDI_IMAGE = 0x0020;
        const Int32 HDI_DI_SETITEM = 0x0040;
        const Int32 HDI_ORDER = 0x0080;
        const Int32 HDI_FILTER = 0x0100;		// 0x0500

        const Int32 HDF_LEFT = 0x0000;
        const Int32 HDF_RIGHT = 0x0001;
        const Int32 HDF_CENTER = 0x0002;
        const Int32 HDF_JUSTIFYMASK = 0x0003;
        const Int32 HDF_RTLREADING = 0x0004;
        const Int32 HDF_OWNERDRAW = 0x8000;
        const Int32 HDF_STRING = 0x4000;
        const Int32 HDF_BITMAP = 0x2000;
        const Int32 HDF_BITMAP_ON_RIGHT = 0x1000;
        const Int32 HDF_IMAGE = 0x0800;
        const Int32 HDF_SORTUP = 0x0400;		// 0x0501
        const Int32 HDF_SORTDOWN = 0x0200;		// 0x0501

        const Int32 LVM_FIRST = 0x1000;		// List messages
        const Int32 LVM_GETHEADER = LVM_FIRST + 31;

        const Int32 HDM_FIRST = 0x1200;		// Header messages
        const Int32 HDM_SETIMAGELIST = HDM_FIRST + 8;
        const Int32 HDM_GETIMAGELIST = HDM_FIRST + 9;
        const Int32 HDM_GETITEM = HDM_FIRST + 11;
        const Int32 HDM_SETITEM = HDM_FIRST + 12;

        private void ShowHeaderIcon(ListView list, int columnIndex, SortOrder sortOrder)
        {
            if (columnIndex < 0 || columnIndex >= list.Columns.Count)
            {
                return;
            }

            IntPtr hHeader = SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            ColumnHeader colHdr = list.Columns[columnIndex];

            HDITEM hd = new HDITEM();
            hd.mask = HDI_FORMAT;

            HorizontalAlignment align = colHdr.TextAlign;

            if (align == HorizontalAlignment.Left)
            {
                hd.fmt = HDF_LEFT | HDF_STRING | HDF_BITMAP_ON_RIGHT;
            }
            else if (align == HorizontalAlignment.Center)
            {
                hd.fmt = HDF_CENTER | HDF_STRING | HDF_BITMAP_ON_RIGHT;
            }
            else    // HorizontalAlignment.Right
            {
                hd.fmt = HDF_RIGHT | HDF_STRING;
            }

            if (useNativeArrows)
            {
                if (sortOrder == SortOrder.Ascending)
                {
                    hd.fmt |= HDF_SORTUP;
                }
                else if (sortOrder == SortOrder.Descending)
                {
                    hd.fmt |= HDF_SORTDOWN;
                }
            }
            else
            {
                hd.mask |= HDI_IMAGE;

                if (sortOrder != SortOrder.None)
                {
                    hd.fmt |= HDF_IMAGE;
                }

                hd.iImage = (int)sortOrder - 1;
            }

            SendMessage2(hHeader, HDM_SETITEM, new IntPtr(columnIndex), ref hd);
        }

        private void SetHeaderImageList(ListView list, ImageList imgList)
        {
            IntPtr hHeader = SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hHeader, HDM_SETIMAGELIST, IntPtr.Zero, imgList.Handle);
        }

        #endregion

        #region ComCtrl information

        [StructLayout(LayoutKind.Sequential)]
        private struct DLLVERSIONINFO
        {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("comctl32.dll")]
        static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);

        static private bool ComCtlDllSupportsArrows()
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = LoadLibrary("comctl32.dll");
                if (hModule != IntPtr.Zero)
                {
                    UIntPtr proc = GetProcAddress(hModule, "DllGetVersion");
                    if (proc == UIntPtr.Zero)    // Old versions don't support this method
                    {
                        return false;
                    }
                }

                DLLVERSIONINFO vi = new DLLVERSIONINFO();
                vi.cbSize = Marshal.SizeOf(typeof(DLLVERSIONINFO));

                DllGetVersion(ref vi);

                return vi.dwMajorVersion >= 6;
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                {
                    FreeLibrary(hModule);
                }
            }
        }

        #endregion
    }

    public class Helper
    {
        static public int HiWord(int number)
        {
            if ((number & 0x80000000) == 0x80000000)
            {
                return (number >> 16);
            }
            else
            {
                return (number >> 16) & 0xffff;
            }
        }

        static public int LoWord(int number)
        {
            return number & 0xffff;
        }
    }
}
