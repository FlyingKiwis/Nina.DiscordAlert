using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.SequenceFailureMonitor;
using NINA.Core.Utility;
using NINA.DiscordAlert.Images;

namespace NINA.DiscordAlert.Util {
    public static class Resources 
    {
        public static IDiscordWebhookClient Client { 
            get 
            {
                if(_client == null) {
                    Logger.Debug("Create discord client");
                    _client = new DiscordWebhookClient(Properties.Settings.Default.DiscordWebhookURL);
                }

                return _client;
            } 
        }

        public static ISequenceFailureMonitorFactory SequenceFailureMonitorFactory {
            get {
                if (_failureMonitorFactory == null) {
                    Logger.Debug("Create failure monitor");
                    _failureMonitorFactory = new SequenceFailureMonitorFactory();
                }

                return _failureMonitorFactory;
            }
        }

        public static IImageSaveMonitor ImageSaveMonitor {
            get 
            {
                return _imageSaveMonitor;
            }
        }

        private static IDiscordWebhookClient _client;
        private static ISequenceFailureMonitorFactory _failureMonitorFactory;
        private static IImageSaveMonitor _imageSaveMonitor;


        public static void SetWebsocketClient(IDiscordWebhookClient client) {
            Logger.Debug(string.Empty);
            _client = client;
        }

        public static void SetSequenceFailureMonitorFactory(ISequenceFailureMonitorFactory failureMonitor) {
            Logger.Debug(string.Empty);
            _failureMonitorFactory = failureMonitor;
        }

        public static void SetImageSaveMonitor(IImageSaveMonitor imageSaveMonitor) {
            Logger.Debug(string.Empty);
            _imageSaveMonitor = imageSaveMonitor;
        }
    }
}
