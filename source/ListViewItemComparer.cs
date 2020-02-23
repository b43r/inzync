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
using System.Collections;
using System.Windows.Forms;

namespace InZync
{
    /// <summary>
    /// Implements the manual sorting of items by columns.
    /// </summary>
    class ListViewItemComparer : IComparer
    {
        private readonly int col;
        private readonly bool ascending;
        private readonly int indexOfSizeCol;

        public ListViewItemComparer() 
        {
            col = 0;
            ascending = true;
        }

        public ListViewItemComparer(int column, bool asc, int indexOfSizeCol) 
        {
            col = column;
            ascending = asc;
            this.indexOfSizeCol = indexOfSizeCol;
        }

        public int Compare(object x, object y) 
        {
            if (col == indexOfSizeCol)
            {
                // special treatment
                string textX = ((ListViewItem)x).SubItems[col].Text.Replace("'", "").Replace(" Bytes", "");
                string textY = ((ListViewItem)y).SubItems[col].Text.Replace("'", "").Replace(" Bytes", "");

                int multiplierX = GetMultiplier(textX);
                int multiplierY = GetMultiplier(textY);

                textX = textX.Replace(" KB", "").Replace(" MB", "").Replace(" GB", "");
                textY = textY.Replace(" KB", "").Replace(" MB", "").Replace(" GB", "");

                long sizeX = (textX == "" ? 0 : long.Parse(textX)) * multiplierX;
                long sizeY = (textY == "" ? 0 : long.Parse(textY)) * multiplierY;
                if (ascending)
                {
                    return (sizeX < sizeY) ? -1 : (sizeX == sizeY) ? 0 : 1;
                } 
                else
                {
                    return (sizeY < sizeX) ? -1 : (sizeY == sizeX) ? 0 : 1;
                }
            }
            else
            {
                // normal string sorting
                if (ascending)
                {
                    return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                } 
                else
                {
                    return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
                }
            }
        }

        private int GetMultiplier(string text)
        {
            if (text.Length > 2)
            {
                switch (text.Substring(text.Length - 2, 2))
                {
                    case "KB":
                        return 1024;
                    case "MB":
                        return 1024 * 1024;
                }
            }
            return 1;
        }
    }
}
