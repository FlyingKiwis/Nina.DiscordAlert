using NINA.WPF.Base.Interfaces.Mediator;
using System;

namespace NINA.DiscordAlert.Images {
    public interface IImageSaveMonitor : IDisposable
    {
        ISavedImageContainer LastImage { get; }
        event EventHandler<ISavedImageContainer> ImageSaved;
    }
}
