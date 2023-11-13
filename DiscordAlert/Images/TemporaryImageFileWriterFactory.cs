using NINA.Profile.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    [ExcludeFromCodeCoverage]
    public class TemporaryImageFileWriterFactory : ITemporaryImageFileWriterFactory {
        public ITemporaryImageFileWriter Create(IProfile profile, BitmapSource image) {
            return new TemporaryImageFileWriter(profile, image);
        }
    }
}
