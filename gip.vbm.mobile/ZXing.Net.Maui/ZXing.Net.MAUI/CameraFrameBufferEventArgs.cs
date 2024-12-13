// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using ZXing.Net.Maui.Readers;

namespace ZXing.Net.Maui
{
	public class CameraFrameBufferEventArgs : EventArgs
	{
		public CameraFrameBufferEventArgs(PixelBufferHolder pixelBufferHolder) : base()
			=> Data = pixelBufferHolder;

		public readonly PixelBufferHolder Data;
	}
}
