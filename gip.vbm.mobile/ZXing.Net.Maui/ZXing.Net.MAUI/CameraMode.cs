// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;

namespace ZXing.Net.Maui
{
	[Flags]
	public enum CameraMode
	{
		Preview = 0,
		Capture = 1,
		Analyze = 2
	}
}
