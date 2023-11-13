using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Util {
    [ExcludeFromCodeCoverage(Justification = "Abstraction of System.IO")]
    public class FileHelper : IFileHelper {
        public bool Exists(string filename) {
            return Path.Exists(filename);
        }

        public string GetFileName(string filename) {
            return Path.GetFileName(filename);
        }

        public void Delete(string filename) {
            File.Delete(filename);
        }

        public string GetTempPath() {
            return Path.GetTempPath();
        }

        public string Combine(params string[] paths) {
            return Path.Combine(paths);
        }

        public void WriteImageToFile(string filename, BitmapEncoder bitmapEncoder) {
            if (Exists(filename))
                throw new IOException($"{filename} already exists");

            using (var fileStream = new FileStream(filename, FileMode.Create)) {
                bitmapEncoder.Save(fileStream);
            }
        }
    }
}
