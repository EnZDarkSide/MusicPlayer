using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;

namespace MusicPlayer.DataHelper
{
    static class ImageHelper
    {
        public static BitmapImage GetImage(byte[] data)
        {
            var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad};
            RenderOptions.SetBitmapScalingMode(bitmapImage, BitmapScalingMode.HighQuality);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(data);
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
