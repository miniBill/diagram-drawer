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

using System.Collections.ObjectModel;
using System.Drawing;
using System;
using System.Xml;
using Nummite.Properties;

namespace Nummite.Shapes {
	class VisiblePoint : Box
	{
		public VisiblePoint ()
		{
			Width = Height = 20;
			Text = String.Empty;
			ContextMenu.MenuItems.Add ("Mostra", ShowClick);
			ContextMenu.MenuItems.Add ("Nascondi", HideClick);
		}

		public override bool NeedInitialize {
			get {
				return true;
			}
		}

		public override void BeginInitialize ()
		{
			
		}

		public override void EndInitialize (KeyedCollection<string, IShape> list)
		{
			Open = false;
		}

		void ShowClick (object sender, EventArgs e)
		{
			Open = true;
		}

		void HideClick (object sender, EventArgs e)
		{
			Open = false;
		}

		public static new string Description { 
			get {
				return "Punto";
			}
		}

		bool open = true;

		public bool Open {
			private get {
				return open;
			}
			set {
				open = value;
				ShapeContainer.ForceRefresh ();
			}
		}

		public override void DrawTo (Graphics graphics)
		{
			if (!Open)
				return;
			graphics.DrawEllipse (BorderPen, Location.X, Location.Y, Width, Height);
			graphics.FillEllipse (ForeBrush,
			                      Location.X + Width / 3F, Location.Y + Height / 3F,
			                      Width / 3F, Height / 3F);
		}

		public readonly static new IShapeCreator Creator = new ShapeCreator<VisiblePoint> (Description, Resources.Point);

		public override PointF GetIntersection (PointF other)
		{
			return Center;
		}

		public override void SvgSave (XmlWriter writer)
		{
		}
	}
}
