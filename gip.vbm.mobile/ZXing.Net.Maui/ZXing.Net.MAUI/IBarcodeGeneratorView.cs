// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;

namespace ZXing.Net.Maui
{
	public interface IBarcodeGeneratorView : IView
	{
		BarcodeFormat Format { get; }

		string Value { get; }

		Color ForegroundColor { get; }

		Color BackgroundColor { get; }

		int BarcodeMargin { get; }
	}
}
