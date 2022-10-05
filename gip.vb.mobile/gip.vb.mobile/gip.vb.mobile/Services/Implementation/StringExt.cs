using gip.core.webservices;
using System;

namespace gip.vb.mobile.Services
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
            if (i < 0)
                i = uri.OriginalString.IndexOf('*');
            if (i < 0)
                i = uri.OriginalString.LastIndexOf('/');
            if (i <= 1)
                return uri.OriginalString;
            else
                return uri.OriginalString.Substring(0, i-1);
        }
    }
}
