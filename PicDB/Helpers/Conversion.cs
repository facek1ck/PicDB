using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace PicDB.Helpers
{
    class Conversion
    {
        internal static BitmapImage FromBase64(string base64)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
                
            }
        }

        internal static string MigrDocFilenameFromBase64(string base64)
        {
            return "base64:" + base64;
        }
    }
}
