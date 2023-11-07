using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {

    [ExcludeFromCodeCoverage]
    public class TemporaryImageFileWriter : ITemporaryImageFileWriter {

        public TemporaryImageFileWriter(BitmapSource image) {
            _image = image;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_image));
            Filename = GenerateFilename();

            using (var fileStream = new FileStream(Filename, FileMode.Create)) {
                encoder.Save(fileStream);
            }
        }

        public string Filename { get; private set; }

        private readonly BitmapSource _image;

        public void Dispose() {
            if (Filename != null && File.Exists(Filename)) {
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
