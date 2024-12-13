// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.webservices;
using System;

namespace gip.vbm.mobile.Services
{
    public static class StringExt
    {
        public static string CorrectEmptyUrlString(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return CoreWebServiceConst.EmptyParam;
            else
                return input;
        }

        public static string WebServiceMethodName(this Uri uri)
        {
            if (String.IsNullOrEmpty(uri.OriginalString))
                return "UnkownMethodName";
            int i = uri.OriginalString.IndexOf('{');
            if (i > 1)
                i--;
            else
            {
                i = uri.OriginalString.IndexOf('*');
                if (i > 1)
                    i--;
                else
                    i = uri.OriginalString.LastIndexOf('/');
            }
            if (i <= 1)
                return uri.OriginalString;
            else
                return uri.OriginalString.Substring(0, i);
        }
    }
}
