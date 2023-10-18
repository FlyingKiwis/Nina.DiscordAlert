using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    public class TemporaryImageFileWriter : IDisposable {
        public string Filename { get; }

        public TemporaryImageFileWriter(BitmapSource image) 
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            Filename = GenerateFilename();

            using (var fileStream = new FileStream(Filename, FileMode.Create)) {
                encoder.Save(fileStream);
            }
        }

        public void Dispose() {
            if (File.Exists(Filename)) { 
                File.Delete(Filename); 
            }
        }

        private static string GenerateFilename() {
            var basePath = Path.GetTempPath();

            while (true) {
                var filename = Path.Combine(basePath, $"{Guid.NewGuid()}.png");
                if (!File.Exists(filename)) {
                    return filename;
                }
            }
        }
    }
}
