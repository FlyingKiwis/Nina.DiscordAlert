namespace NINA.DiscordAlert.Util {
    public static class Resources 
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

        public static ISequenceFailureMonitorFactory SequenceFailureMonitorFactory {
            get {
                if (_failureMonitorFactory == null) {
                    var monitor = new SequenceFailureMonitorFactory();
                }

                return _failureMonitorFactory;
            }
        }

        private static IDiscordWebhookClient _client;
        private static ISequenceFailureMonitorFactory _failureMonitorFactory;

        public static void SetWebsocketClient(IDiscordWebhookClient client) {
            _client = client;
        }

        public static void SetSequenceFailureMonitor(ISequenceFailureMonitorFactory failureMonitor) {
            _failureMonitorFactory = failureMonitor;
        }
    }
}
