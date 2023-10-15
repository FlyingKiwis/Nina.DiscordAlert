using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.DiscordWebhook {
    public interface IDiscordWebhookClientFactory {
        IDiscordWebhookClient Create();
    }
}
