// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using ZXing.Net.Maui.Readers;

namespace ZXing.Net.Maui
{
	public interface ICameraBarcodeReaderView : ICameraView
	{
		BarcodeReaderOptions Options { get; }

		void BarcodesDetected(BarcodeDetectionEventArgs args);

		bool IsDetecting { get; set; }
	}
}
