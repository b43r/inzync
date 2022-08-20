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

namespace InZync
{
    /// <summary>
    /// This class represents an item (file or directory) which must be synced
    /// and which is stored in a SyncList class.
    /// </summary>
    public class SyncItem
    {
        public string SourcePath;
        public string DestPath;
        public string RelPath;
        public string Size;
        public long NumSize;
        public string Date;
        public string Status;
        public string Action;
        public System.Drawing.Color Color;
        public bool Processed;
    }
}
