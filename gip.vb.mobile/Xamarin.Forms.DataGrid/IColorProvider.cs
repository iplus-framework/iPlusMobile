// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace Xamarin.Forms.DataGrid
{
	public interface IColorProvider
	{
		Color GetColor(int rowIndex, object item);
	}
}
