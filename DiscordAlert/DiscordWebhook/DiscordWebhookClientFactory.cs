using System.Diagnostics.CodeAnalysis;

namespace NINA.DiscordAlert.DiscordWebhook {

    [ExcludeFromCodeCoverage]
    public class DiscordWebhookClientFactory : IDiscordWebhookClientFactory {
        public IDiscordWebhookClient Create() {
            return new DiscordWebhookClient(Properties.Settings.Default.DiscordWebhookURL);
        }
    }
}
