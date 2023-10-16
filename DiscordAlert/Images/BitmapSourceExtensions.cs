using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public static class BitmapSourceExtensions 
        {
        public static BitmapSource Resize(this BitmapSource image, int maxDimension) 
        {
            var width = image.Width;
            var height = image.Height;
            var maxCurrent = Math.Max(width, height);

            if(maxCurrent <= maxDimension) 
            {
                return image;
            }

            var scale = maxDimension /  maxCurrent;
            var scaledBitmap = new WriteableBitmap(new TransformedBitmap(image, new ScaleTransform(scale, scale)));
            scaledBitmap.Freeze();
            return scaledBitmap;
        }
    }
}
