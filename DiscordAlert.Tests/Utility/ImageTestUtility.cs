using Moq;
using NINA.DiscordAlert.Images;
using NINA.WPF.Base.Interfaces.Mediator;

namespace DiscordAlert.Tests.Utility {
    public class ImageTestUtility 
    {
        public static ImageSavedEventArgs GenerateImageSavedEventArgs(ISavedImageContainer container) 
        {
            var imageSavedEventArgs = new ImageSavedEventArgs() {
                MetaData = container.MetaData,
                Image = container.Image,
                StarDetectionAnalysis = container.StarDetectionAnalysis,
                Statistics = container.Statistics,
                PathToImage = container.PathToImage,
                IsBayered = container.IsBayered
            };


            return imageSavedEventArgs;
        }
    }
}
