// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.DataGrid
{
	public sealed class PaletteCollection : List<Color>, IColorProvider
	{
		public Color GetColor(int rowIndex, object item)
		{
			return Count > 0 ? this.ElementAt(rowIndex % Count) : default;
		}
	}
}
