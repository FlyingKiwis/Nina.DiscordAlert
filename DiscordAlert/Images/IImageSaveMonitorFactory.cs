using NINA.WPF.Base.Interfaces.Mediator;

namespace NINA.DiscordAlert.Images {
    public interface IImageSaveMonitorFactory {
        IImageSaveMonitor Create(IImageSaveMediator imageMediator);
    }
}
