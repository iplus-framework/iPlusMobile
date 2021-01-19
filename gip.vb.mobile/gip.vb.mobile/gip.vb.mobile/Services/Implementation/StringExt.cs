using gip.core.webservices;

namespace gip.vb.mobile.Services.Implementation
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
    }
}
