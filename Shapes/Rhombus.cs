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
using System;
using Nummite.Properties;

namespace Nummite.Shapes {
	class Rhombus : Box
	{
		public override bool Contains (PointF point)
		{
			var c = Center;
			var ox = point.X - c.X;
			var oy = point.Y - c.Y;
			if (ox == 0)
				return Math.Abs (oy) <= (Height / 2F);
			if (oy == 0)
				return Math.Abs (ox) <= (Width / 2F);
			var m = oy / ox;
			if (m > 0) {
				var qa = Height / 2F * Math.Sign (ox);
				float ma = -Height;
				ma /= Width;
				return oy >= 0
					? oy < (ox * ma + qa)
					: oy > (ox * ma + qa);
			}
			var qb = -Height / 2F * Math.Sign (ox);
			float mb = Height;
			mb /= Width;
			return oy >= 0
				? oy < (ox * mb + qb)
				: oy > (ox * mb + qb);
		}

		public override PointF GetIntersection (PointF other)
		{
			PointF c = Center;
			var ox = other.X - c.X;
			var oy = other.Y - c.Y;
			if (ox == 0)
				return new PointF (c.X, c.Y + Height / 2 * Math.Sign (oy));
			if (oy == 0)
				return new PointF (c.X + Width / 2 * Math.Sign (ox), c.Y);
			var m = oy / ox;
			var qa = Height / 2F * Math.Sign (oy);
			float ma = -Height * Math.Sign (m);
			ma /= Width;
			var x = qa / (m - ma);
			var y = m * x;
			return new PointF (x + c.X, y + c.Y);
		}

		protected override void DrawBackground (Graphics graphics)
		{
			var a = Width / 2F;
			var b = Height / 2F;
			PointF c = Center;
			var points = new[] {
				new PointF (c.X - a, c.Y),
				new PointF (c.X, c.Y + b),
				new PointF (c.X + a, c.Y),
				new PointF (c.X, c.Y - b),
				new PointF (c.X - a, c.Y)
			};
			graphics.FillPolygon (BackBrush, points);
			graphics.DrawPolygon (BorderPen, points);
		}

		public static new string Description { 
			get {
				return "Rombo";
			}
		}

		public readonly static new IShapeCreator Creator = new ShapeCreator<Rhombus> (Description, Resources.Rhombus);
	}
}
