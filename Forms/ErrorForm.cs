﻿/* Copyright (C) 2009 Leonardo Taglialegne <leonardotaglialegne@gmail.com>
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
using System.Windows.Forms;

namespace Nummite.Forms{
	partial class ErrorForm : Form {
		public ErrorForm(string error) {
			InitializeComponent();
			textBox1.Text = error;
		}
		void Button2Click(object sender, EventArgs e) {
			Close();
		}
		void Button1Click(object sender, EventArgs e) {
			Clipboard.SetText(textBox1.Text);
		}
	}
}
