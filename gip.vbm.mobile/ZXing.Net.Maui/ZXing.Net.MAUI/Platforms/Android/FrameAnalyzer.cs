// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using AndroidX.Camera.Core;
using Java.Nio;
using Microsoft.Maui.Graphics;
using System;

namespace ZXing.Net.Maui
{
	internal class FrameAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
	{
		readonly Action<ByteBuffer, Size> frameCallback;

		public FrameAnalyzer(Action<ByteBuffer, Size> callback)
		{
			frameCallback = callback;
		}

		public void Analyze(IImageProxy image)
		{
			var buffer = image.GetPlanes()[0].Buffer;

			var s = new Size(image.Width, image.Height);

			frameCallback?.Invoke(buffer, s);

			image.Close();
		}
	}
}
