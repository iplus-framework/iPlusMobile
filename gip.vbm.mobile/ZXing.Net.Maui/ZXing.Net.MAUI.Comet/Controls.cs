// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using Comet;
using ZXing.Net.Maui;

[assembly: CometGenerate(typeof(ICameraBarcodeReaderView), nameof(ICameraBarcodeReaderView.BarcodesDetected))]
[assembly: CometGenerate(typeof(ICameraView), nameof(ICameraView.CameraLocation))]
[assembly: CometGenerate(typeof(IBarcodeGeneratorView), nameof(IBarcodeGeneratorView.Value), nameof(IBarcodeGeneratorView.Format), Skip = new[] { $"{nameof(IBarcodeGeneratorView.ForegroundColor)}:{EnvironmentKeys.Colors.Color}" })]