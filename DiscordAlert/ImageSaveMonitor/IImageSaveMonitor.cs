using NINA.WPF.Base.Interfaces.Mediator;
using System;

namespace NINA.DiscordAlert.ImageSaveMonitor {
    public interface IImageSaveMonitor 
    {
        ImageSavedEventArgs LastImage { get; }
        event EventHandler<ImageSavedEventArgs> ImageSaved;
        string ReplacePlaceholders(string textWithPlaceholders, ImageSavedEventArgs image);
    }
}
