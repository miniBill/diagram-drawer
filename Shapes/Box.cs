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

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Nummite.Forms;
using Nummite.Properties;
using Nummite.Export;

namespace Nummite.Shapes
{
	internal class Box : IShape, IAutoSizeable, IStyleable
	{
		public Font Font
		{
			get;
			set;
		}

		private string text;

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				OnTextChange();
			}
		}

		protected virtual void OnTextChange()
		{
			if (AutoResizeWidth || AutoResizeHeight)
				DoAutoResize();
		}

		protected Brush BackBrush
		{
			get;
			private set;
		}

		protected Pen BackPen
		{
			get;
			private set;
		}

		private Color backgroundColor;

		public Color BackgroundColor
		{
			get
			{
				return backgroundColor;
			}
			set
			{
				backgroundColor = value;
				OnBackgroundColorChange();
			}
		}

		public event EventHandler BackgroundColorChange;

		private void OnBackgroundColorChange()
		{
			if (BackBrush != null)
				BackBrush.Dispose();
			if (BackPen != null)
				BackPen.Dispose();
			BackBrush = new SolidBrush(backgroundColor);
			BackPen = new Pen(backgroundColor);
			if (BackgroundColorChange != null)
				BackgroundColorChange(this, EventArgs.Empty);
		}

		protected Brush ForeBrush
		{
			get;
			private set;
		}

		protected Pen ForePen
		{
			get;
			private set;
		}

		private Color foregroundColor;

		public Color ForegroundColor
		{
			get
			{
				return foregroundColor;
			}
			set
			{
				foregroundColor = value;
				if (ForeBrush != null)
					ForeBrush.Dispose();
				if (ForePen != null)
					ForePen.Dispose();
				ForeBrush = new SolidBrush(foregroundColor);
				ForePen = new Pen(foregroundColor);
				if (ForegroundColorChange != null)
					ForegroundColorChange(this, EventArgs.Empty);
			}
		}

		public event EventHandler ForegroundColorChange;
		public event EventHandler BorderColorChange;

		protected Pen BorderPen
		{
			get;
			private set;
		}

		protected Brush BorderBrush
		{
			get;
			private set;
		}

		private Color borderColor;

		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				if (ForeBrush != null)
					ForeBrush.Dispose();
				if (ForePen != null)
					ForePen.Dispose();
				ForeBrush = new SolidBrush(foregroundColor);
				ForePen = new Pen(foregroundColor);
				if (ForegroundColorChange != null)
					ForegroundColorChange(this, EventArgs.Empty);


				borderColor = value;
				if (BorderBrush != null)
					BorderBrush.Dispose();
				if (BorderPen != null)
					BorderPen.Dispose();
				BorderBrush = new SolidBrush(borderColor);
				BorderPen = new Pen(borderColor);
				if (BorderColorChange != null)
					BorderColorChange(this, EventArgs.Empty);
			}
		}

		public Box()
		{
			ContextMenu = new ContextMenu();
			Center = Point.Empty;
			Size = new Size(100, 50);
			BackgroundColor = Color.White;
			ForegroundColor = Color.Black;
			BorderColor = Color.Black;
			text = String.Empty;
			Font = SystemFonts.DefaultFont;
			ContextMenu.MenuItems.Add("Elimina", DelClick);
			ContextMenu.MenuItems.Add("Dimensione", SizeClick);
			ContextMenu.MenuItems.Add("Porta in primo piano", ForeBring);
		}

		private void ForeBring(object sender, EventArgs e)
		{
			ShapeContainer.BringForeground(this);
		}

		private void DelClick(object sender, EventArgs e)
		{
			new MethodInvoker(Delete).BeginInvoke(null, null);
		}

		private void SizeClick(object sender, EventArgs e)
		{
			new SizeForm(this).Show();
		}

		public virtual bool Contains(PointF point)
		{
			return point.X > Location.X && point.X < Location.X + Width && point.Y > Location.Y && point.Y < Location.Y + Height;
		}

		public virtual void DrawTo(Graphics graphics)
		{
			DrawBackground(graphics);
			DrawText(graphics);
		}

		protected virtual void DrawBackground(Graphics graphics)
		{
			var bounds = new Rectangle(Location.X, Location.Y, Width, Height);
			graphics.FillRectangle(BackBrush, bounds);
			graphics.DrawRectangle(BorderPen, bounds);
		}

		private void DrawText(Graphics graphics)
		{
			var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			graphics.DrawString(Text, Font, ForeBrush, Center, sf);
		}

		public Point Center
		{
			get;
			private set;
		}

		public virtual PointF GetIntersection(PointF other)
		{
			float r = Height;
			r /= Width;
			var l = -r;
			PointF c = Center;
			var ox = other.X - c.X;
			var oy = other.Y - c.Y;
			if (Math.Abs(ox) < Options.TOLERANCE)
				return new PointF(c.X, c.Y + Height / 2F * Math.Sign(oy));
			var m = oy / ox;
			float x, y;
			if (l < m && m < r)
			{
				x = Width / 2F * Math.Sign(ox);
				y = m * x;
			}
			else
			{
				y = Height / 2F * Math.Sign(oy);
				x = y / m;
			}
			return new PointF(x + c.X, y + c.Y);
		}

		protected ContextMenu ContextMenu { get; private set; }

		public virtual void OpenMenu(PointF point)
		{
			ContextMenu.Show(ShapeContainer, new Point((int)point.X, (int)point.Y));
		}

		protected void Delete()
		{
			ShapeContainer.RemoveShape(this);
			if (Deleted != null)
				Deleted(this, EventArgs.Empty);
		}

		public void Move(Point point)
		{
			var x = point.X - Offset.X;
			if (x < 0)
				x = 0;
			if (x + Width > ShapeContainer.Width)
				x = ShapeContainer.Width - Width;
			var y = point.Y - Offset.Y;
			if (y < 0)
				y = 0;
			if (y + Height > ShapeContainer.Height)
				y = ShapeContainer.Height - Height;
			Location = new Point(x, y);
			if (ShapeContainer.Grid)
				Location = ShapeContainer.CutToGrid(Location);
			if (Moving != null)
				Moving(this, EventArgs.Empty);
		}

		public static string Description
		{
			get
			{
				return "Rettangolo";
			}
		}

		private bool dragged;

		public bool Dragged
		{
			get
			{
				return dragged;
			}
			set
			{
				if (value)
				{
					if (DragStart != null)
						DragStart(this, EventArgs.Empty);
				}
				else if (dragged)
					if (DragEnd != null)
						DragEnd(this, EventArgs.Empty);
				dragged = value;
			}
		}

		private Point location;

		public Point Location
		{
			get
			{
				return location;
			}
			protected set
			{
				location = value;
				Center = VCenter;
			}
		}

		/// <summary>
		/// For optimization issues, this is the "true" center proprierty, whereas Center is just a "buffer"
		/// </summary>
		protected virtual Point VCenter
		{
			get
			{
				return new Point(Location.X + Width / 2, Location.Y + Height / 2);
			}
		}

		public Point Offset
		{
			get;
			set;
		}

		public Size Size
		{
			get
			{
				return size;
			}
			set
			{
				if (size == value) return;
				size = value;
				OnSizeChange();
			}
		}

		protected virtual void OnSizeChange()
		{
			Center = VCenter;
		}

		public int Width { get { return Size.Width; } set { Size = new Size(value, Height); } }
		public int Height { get { return Size.Height; } set { Size = new Size(Width, value); } }

		public int X { get { return Location.X; } set { Location = new Point(value, Y); } }
		public int Y { get { return Location.Y; } set { Location = new Point(X, value); } }

		public event EventHandler Moving;

		public bool Depends
		{
			get;
			set;
		}

		public event EventHandler DragStart;
		public event EventHandler DragEnd;
		public event EventHandler Deleted;

		public string Name
		{
			get;
			set;
		}

		public virtual void BeginInitialize()
		{
		}

		public virtual void EndInitialize(KeyedCollection<string, IShape> list)
		{
		}

		public virtual bool NeedInitialize
		{
			get
			{
				return false;
			}
		}
		public ShapeContainer ShapeContainer
		{
			get;
			set;
		}

		public virtual bool Linkable
		{
			get
			{
				return true;
			}
		}

		public static readonly IShapeHelper Helper = new BoxHelper<Box>(Description, Resources.Rectangle);

		public void Refresh()
		{
			ShapeContainer.ForceRefresh();
		}

		public void SetLocation(Point point)
		{
			Location = point;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			BackBrush.Dispose();
			ForeBrush.Dispose();
			BorderBrush.Dispose();
			BackPen.Dispose();
			ForePen.Dispose();
			BorderPen.Dispose();
			Font.Dispose();
			ContextMenu.Dispose();
		}

		protected void DoAutoResize()
		{
			if (!(AutoResizeHeight || AutoResizeWidth) || ShapeContainer == null)
				return;
			Size newSize;
			using (var g = ShapeContainer.CreateGraphics())
				newSize = GetSize(g, Text, Font);
			if (AutoResizeWidth)
				Width = newSize.Width;
			if (AutoResizeHeight)
				Height = newSize.Height;
			Size = newSize;
			ShapeContainer.ForceRefresh();
		}

		public static Size GetSize(Graphics g, string text, Font font)
		{
			Size newSize = Size.Empty;
			var textSize = g.MeasureString(text, font);
			var w = (int)textSize.Width + 10;
			newSize.Width = Math.Max(w, Options.MinimumWidth);
			var h = (int)textSize.Height + 5;
			newSize.Height = Math.Max(h, Options.MinimumHeight);
			return newSize;
		}

		public virtual void SvgSave(XmlWriter writer)
		{
			Svg.WriteRectangle(writer, Location, Size, BackgroundColor, BorderPen);
			Svg.WriteText(writer, Center, ForegroundColor, Font, Text);
		}

		private bool autoresizewidth;

		public bool AutoResizeWidth
		{
			get
			{
				return autoresizewidth;
			}
			set
			{
				autoresizewidth = value;
				if (value)
					DoAutoResize();
			}
		}

		private bool autoresizeheight;
		private Size size;

		public bool AutoResizeHeight
		{
			get
			{
				return autoresizeheight;
			}
			set
			{
				autoresizeheight = value;
				if (value)
					DoAutoResize();
			}
		}
	}
}