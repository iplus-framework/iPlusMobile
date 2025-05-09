// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Net.Http;


namespace gip.vbm.mobile.Services
{
    public static class HttpRequestExtensions
    {
        private const string TimeoutPropertyKey = "RequestTimeout";

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));
            request.Options.Set<TimeSpan?>(new HttpRequestOptionsKey<TimeSpan?>(TimeoutPropertyKey), timeout);
            //request.Properties[TimeoutPropertyKey] = timeout;
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));
            if (request.Options.TryGetValue<TimeSpan?>(new HttpRequestOptionsKey<TimeSpan?>(TimeoutPropertyKey), out var value))
                return value;
            //if (request.Properties.TryGetValue(TimeoutPropertyKey, out var value) && value is TimeSpan timeout)
            //    return timeout;
            return null;
        }
    }
}