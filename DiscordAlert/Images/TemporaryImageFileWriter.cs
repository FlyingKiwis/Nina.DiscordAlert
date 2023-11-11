using NINA.Core.Utility;
using NINA.DiscordAlert.Util;
using NINA.Profile.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {

    public class TemporaryImageFileWriter : ITemporaryImageFileWriter {

        public TemporaryImageFileWriter(IProfile profile, BitmapSource image) {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            Filename = GenerateFilename(profile.ImageFileSettings.FilePath);
            Helpers.File.WriteImageToFile(Filename, encoder);
        }

        public string Filename { get; private set; }

        public void Dispose() {
            if (Filename != null && Helpers.File.Exists(Filename)) {
                Helpers.File.Delete(Filename);
            }
        }

        private static string GenerateFilename(string basePath) {
            var maxAttempts = 20;

            for (var i = 0; i < maxAttempts; i++ ) {
                var filename = Helpers.File.Combine(basePath, $"discordAttachment-{Guid.NewGuid()}.png");
                Logger.Trace($"Checking filename: {filename}");
                if (!Helpers.File.Exists(filename)) {
                    return filename;
                }
            }

            throw new Exception($"Could not create an avaliable filename after {maxAttempts} attemps");
        }
    }
}
