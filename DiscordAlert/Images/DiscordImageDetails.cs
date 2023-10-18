using NINA.Core.Model;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public class DiscordImageDetails
    {
        public BitmapSource Image { get; }
        public ImagePatterns ImagePatterns { get; }

        public DiscordImageDetails(BitmapSource image, ImagePatterns imagePatterns) {
            Image = image;
            ImagePatterns = imagePatterns;
        }
    }
}
