using NINA.Profile.Interfaces;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public interface ITemporaryImageFileWriterFactory {
        ITemporaryImageFileWriter Create(IProfile profile, BitmapSource image);
    }
}