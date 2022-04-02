using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace Register.Web.Helper
{
    [SupportedOSPlatform("windows")]
    public static class ImageConverter
    {      

        public static void Base64ToImage(this string path,string base64)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                using Bitmap bm2 = new Bitmap(ms);
                bm2.Save(path);
            }
        }


    }
}
