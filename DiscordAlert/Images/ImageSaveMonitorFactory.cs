using NINA.WPF.Base.Interfaces.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.Images {
    public class ImageSaveMonitorFactory : IImageSaveMonitorFactory {
        public IImageSaveMonitor Create(IImageSaveMediator imageMediator) {
            return new ImageSaveMonitor(imageMediator);
        }
    }
}
