using NINA.DiscordAlert.DiscordWebhook;

namespace NINA.DiscordAlert.Util {
    public static class Helpers {
        private static IDiscordHelper _discordHelper;
        public static IDiscordHelper Discord {
            get {
                if (_discordHelper == null) {
                    _discordHelper = new DiscordHelper();
                }
                return _discordHelper;
            }
        }

        public static void SetDiscordHelper(IDiscordHelper discordHelper)
        {
            _discordHelper = discordHelper;
        }
    }
}
