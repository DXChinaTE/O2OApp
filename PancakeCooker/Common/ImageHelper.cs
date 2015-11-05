using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

namespace PancakeCooker.Common
{
    public static class ImageHelper
    {
        public static BitmapImage GetBitmap(ImageBrush brush)
        {
            BitmapImage bmp = (BitmapImage)brush.ImageSource;
            return bmp;
        }
    }
   
}
