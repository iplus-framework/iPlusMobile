// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using System;

namespace ZXing.Net.Maui
{
	public interface ICameraFrameAnalyzer
	{
		void FrameReady(CameraFrameBufferEventArgs args);
	}

	public interface ICameraView : IView, ICameraFrameAnalyzer
	{
		CameraLocation CameraLocation { get; set; }

		//CameraMode Mode { get; set; }

		void AutoFocus();

		void Focus(Point point);

		bool IsTorchOn { get; set; }
	}
}
