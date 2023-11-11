using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Util {
    public interface IFileHelper {
        string Combine(params string[] paths);
        void Delete(string filename);
        bool Exists(string filename);
        string GetFileName(string filename);
        string GetTempPath();
        void WriteImageToFile(string filename, BitmapEncoder bitmapEncoder);
    }
}