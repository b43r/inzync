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

using System.Collections.Generic;

namespace InZync
{
    /// <summary>
    /// This class represents a source/destination path pair.
    /// </summary>
    public class PathPair
    {
        public string Source { get; set; }

        public string Destination { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PathPair other)
            {
                return Source.Equals(other.Source) && Destination.Equals(other.Destination);
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 1918477335;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Destination);
            return hashCode;
        }
    }
}
