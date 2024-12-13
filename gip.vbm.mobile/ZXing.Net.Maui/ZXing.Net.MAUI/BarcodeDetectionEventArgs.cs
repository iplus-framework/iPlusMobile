// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace ZXing.Net.Maui
{
	public class BarcodeDetectionEventArgs : EventArgs
	{
		public BarcodeDetectionEventArgs(BarcodeResult[] results)
			: base()
		{
			Results = results;
		}

		public BarcodeResult[] Results { get; private set; }
	}
}
