// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
#if MACCATALYST || IOS
using AVFoundation;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;

namespace ZXing.Net.Maui
{
	class CaptureDelegate : NSObject, IAVCaptureVideoDataOutputSampleBufferDelegate
	{
		public Action<CVPixelBuffer> SampleProcessor { get; set; }

		[Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
		public void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			// Get the CoreVideo image
			using (var pixelBuffer = sampleBuffer.GetImageBuffer())
			{
				if (pixelBuffer is CVPixelBuffer cvPixelBuffer)
				{
					// Lock the base address
					cvPixelBuffer.Lock(CVPixelBufferLock.ReadOnly); // MAYBE NEEDS READ/WRITE

					SampleProcessor?.Invoke(cvPixelBuffer);

					cvPixelBuffer.Unlock(CVPixelBufferLock.ReadOnly);
				}
			}

			//
			// Although this looks innocent "Oh, he is just optimizing this case away"
			// this is incredibly important to call on this callback, because the AVFoundation
			// has a fixed number of buffers and if it runs out of free buffers, it will stop
			// delivering frames. 
			//	
			sampleBuffer?.Dispose();
		}

		[Export("captureOutput:didDropSampleBuffer:fromConnection:")]
		public void DidDropSampleBuffer(AVCaptureOutput captureOutput, CoreMedia.CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			
		}
	}
}
#endif