using NINA.Image.ImageData;
using NINA.Image.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using System;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    internal class SavedImageContainer : ISavedImageContainer {
        public ImageMetaData MetaData { get; }
        public BitmapSource Image { get; }
        public IImageStatistics Statistics { get; }
        public IStarDetectionAnalysis StarDetectionAnalysis { get; }
        public Uri PathToImage { get; }
        public bool IsBayered { get; }

        public SavedImageContainer(ImageSavedEventArgs args) 
        {
            MetaData = args.MetaData;
            Image = args.Image;
            Statistics = args.Statistics;
            StarDetectionAnalysis = args.StarDetectionAnalysis;
            PathToImage = args.PathToImage;    
            IsBayered = args.IsBayered;
        }
    }
}
