using System;

namespace NINA.DiscordAlert.Images {
    public interface ITemporaryImageFileWriter : IDisposable {
        string Filename { get; }
    }
}