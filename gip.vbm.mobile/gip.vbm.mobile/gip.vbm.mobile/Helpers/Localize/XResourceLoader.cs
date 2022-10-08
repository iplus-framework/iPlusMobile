using System;
using System.Collections.Generic;
using System.Text;

namespace gip.vbm.mobile.Helpers
{
    public static class XResourceLoader
    {
        private static IXResourceLoader _ResourceLoader;
        public static void SetResourceLoader(IXResourceLoader resourceLoader)
        {
            _ResourceLoader = resourceLoader;
        }

        public static string GetString(String resource)
        {
            if (_ResourceLoader != null)
            {
                return _ResourceLoader.GetString(resource);
                //string rString = _ResourceLoader.GetString(resource);
                //if (!String.IsNullOrEmpty(rString))
                //    rString = rString.Replace("`n", "\n");
                //return rString;
            }
            return resource;
        }
    }
}
