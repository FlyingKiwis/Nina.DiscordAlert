using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public interface ITemporaryImageFileWriterFactory {
        ITemporaryImageFileWriter Create(BitmapSource image);
    }
}