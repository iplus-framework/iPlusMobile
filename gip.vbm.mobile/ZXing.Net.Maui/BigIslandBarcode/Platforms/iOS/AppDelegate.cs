// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace BigIslandBarcode
{
	[Register("AppDelegate")]
	public class AppDelegate : MauiUIApplicationDelegate
	{
		protected override MauiApp CreateMauiApp()
			=> MauiProgram.Create();
	}
}
