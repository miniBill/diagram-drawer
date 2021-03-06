/* Copyright (C) 2008 Leonardo Taglialegne <leonardotaglialegne@gmail.com>
 *
 * This file is part of Nummite.
 *
 * Nummite is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Nummite is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Nummite.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Drawing;
using Nummite.Properties;

namespace Nummite.Shapes.Lines {
	class TwoArrowsAngle : OneArrowAngle
	{
		public override void DrawTo (Graphics graphics)
		{
			if (!(SubLines [0] is OneArrow)) {
				var l = SubLines [0];
				SubLines [0] = new OneArrow {
					Origin = l.Pointed,
					Pointed = l.Origin
				};
			}
			base.DrawTo (graphics);
		}

		public readonly static new ILineCreator Creator = new LineCreator<TwoArrowsAngle>(Description, Resources.TwoArrowsAngle);

		public static new string Description { 
			get {
				return "Ad angolo con doppia freccia";
			}
		}
	}
}
