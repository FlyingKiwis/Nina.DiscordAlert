using NINA.Image.ImageData;
using NINA.Image.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public interface ISavedImageContainer 
    {
        public ImageMetaData MetaData { get; }
        public BitmapSource Image { get; }
        public IImageStatistics Statistics { get; }
        public IStarDetectionAnalysis StarDetectionAnalysis { get; }
        public Uri PathToImage { get; }
        public bool IsBayered { get; }
    }
}
