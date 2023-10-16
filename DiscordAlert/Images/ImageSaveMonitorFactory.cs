using NINA.WPF.Base.Interfaces.Mediator;

namespace NINA.DiscordAlert.Images {
    public class ImageSaveMonitorFactory : IImageSaveMonitorFactory {
        public IImageSaveMonitor Create(IImageSaveMediator imageMediator) {
            return new ImageSaveMonitor(imageMediator);
        }
    }
}
