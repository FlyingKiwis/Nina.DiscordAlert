namespace NINA.DiscordAlert.Util {
    public static class DiscordResources 
    {
        public static IDiscordWebhookClient Client { 
            get 
            {
                if(_client == null) {
                    _client = new DiscordWebhookClient(Properties.Settings.Default.DiscordWebhookURL);
                }

                return _client;
            } 
        }

        private static IDiscordWebhookClient _client;

        public static void SetWebsocketClient(IDiscordWebhookClient client) {
            _client = client;
        }
    }
}
