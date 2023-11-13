using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public static class BitmapSourceExtensions {
        public static BitmapFrame CreateResizedImage(this ImageSource source, double maxDimension) {

            var width = source.Width;
            var height = source.Height;
            var maxCurrent = Math.Max(width, height);
            var scale = maxDimension / maxCurrent;

            var newWidth = width;
            var newHeight = height;
            if (scale < 1.0) {
                newHeight *= scale;
                newWidth *= scale;
            }

            var intWdith = Convert.ToInt32(newWidth);
            var intHeight = Convert.ToInt32(newHeight);

            var rect = new System.Windows.Rect(0, 0, newWidth, newHeight);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                intWdith, intHeight,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            var frame = BitmapFrame.Create(resizedImage);
            frame.Freeze();
            return frame;
        }
    }
}
