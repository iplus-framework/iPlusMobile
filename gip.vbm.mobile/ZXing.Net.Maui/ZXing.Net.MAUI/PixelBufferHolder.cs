// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using Microsoft.Maui.Graphics;

namespace ZXing.Net.Maui.Readers
{
	public record PixelBufferHolder
	{
		public Size Size { get; init; }

		public

#if ANDROID
		Java.Nio.ByteBuffer
#elif IOS || MACCATALYST
		CoreVideo.CVPixelBuffer
#else
		byte[]
#endif

		Data { get; init; }
	}
}
