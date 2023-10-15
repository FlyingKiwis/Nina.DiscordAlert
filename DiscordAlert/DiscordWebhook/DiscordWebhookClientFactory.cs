namespace NINA.DiscordAlert.DiscordWebhook {
    public class DiscordWebhookClientFactory : IDiscordWebhookClientFactory {
        public IDiscordWebhookClient Create() {
            return new DiscordWebhookClient(Properties.Settings.Default.DiscordWebhookURL);
        }
    }
}
