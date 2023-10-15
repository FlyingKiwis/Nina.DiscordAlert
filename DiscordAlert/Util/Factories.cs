using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.SequenceFailureMonitor;
using NINA.Core.Utility;
using NINA.DiscordAlert.Images;

namespace NINA.DiscordAlert.Util {
    public static class Factories 
    {
        public static IDiscordWebhookClientFactory DiscordClientFactory { 
            get 
            {
                if(_discordClientFactory == null) {
                    Logger.Debug("Create discord client");
                    _discordClientFactory = new DiscordWebhookClientFactory();
                }

                return _discordClientFactory;
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

        public static IImageSaveMonitorFactory ImageSaveMonitorFactory {
            get {
                if (_imageSaveMonitorFactory == null) {
                    Logger.Debug("Create failure monitor");
                    _imageSaveMonitorFactory = new ImageSaveMonitorFactory();
                }

                return _imageSaveMonitorFactory;
            }
        }

        private static IDiscordWebhookClientFactory _discordClientFactory;
        private static ISequenceFailureMonitorFactory _failureMonitorFactory;
        private static IImageSaveMonitorFactory _imageSaveMonitorFactory;

        public static void SetDiscordClientFactory(IDiscordWebhookClientFactory discordClientFactory) {
            Logger.Debug(string.Empty);
            _discordClientFactory = discordClientFactory;
        }

        public static void SetSequenceFailureMonitorFactory(ISequenceFailureMonitorFactory failureMonitor) {
            Logger.Debug(string.Empty);
            _failureMonitorFactory = failureMonitor;
        }

        public static void SetImageSaveMonitorFactory(IImageSaveMonitorFactory imageSaveMonitorFactory) {
            Logger.Debug(string.Empty);
            _imageSaveMonitorFactory = imageSaveMonitorFactory;
        }
    }
}
