// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿namespace ZXing.Net.Maui
{
	public record BarcodeReaderOptions

	{
		public bool AutoRotate { get; init; }

		public bool TryHarder { get; init; }

		public bool TryInverted { get; init; }

		public BarcodeFormat Formats { get; init; }

		public bool Multiple { get; init; }

	}
}
