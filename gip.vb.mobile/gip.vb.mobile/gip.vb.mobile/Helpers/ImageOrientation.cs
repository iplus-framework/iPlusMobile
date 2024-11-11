// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    /// <summary>
	/// Specifies where the image will occur relative to the text on a
	/// </summary>
	public enum ImageOrientation
    {
        /// <summary>
        /// The image to left
        /// </summary>
        ImageToLeft = 0,
        /// <summary>
        /// The image on top
        /// </summary>
        ImageOnTop = 1,
        /// <summary>
        /// The image to right
        /// </summary>
        ImageToRight = 2,
        /// <summary>
        /// The image on bottom
        /// </summary>
        ImageOnBottom = 3,
        /// <summary>
        /// The image centered
        /// </summary>
        ImageCentered = 4
    }
}
