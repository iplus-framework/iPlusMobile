// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXingFormat = ZXing.BarcodeFormat;

namespace ZXing.Net.Maui
{
	public record BarcodeResult
	{
		public byte[] Raw { get; init; }

		public string Value { get; init; }

		public BarcodeFormat Format { get; init; }

		public IReadOnlyDictionary<MetadataType, object> Metadata { get; init; }

		public PointF[] PointsOfInterest { get; init; }
	}
}
