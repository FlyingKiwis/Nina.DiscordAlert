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

        private static ITemplateHelper _templateHelper;
        public static ITemplateHelper Template {
            get {
                if(_templateHelper == null) {
                    _templateHelper = new TemplaterHelper();
                }
                return _templateHelper;
            }
        }

        private static IFileHelper _fileHelper;
        public static IFileHelper File {
            get {
                if (_fileHelper == null) {
                    _fileHelper = new FileHelper();
                }
                return _fileHelper;
            }
        }

        public static void SetDiscordHelper(IDiscordHelper discordHelper)
        {
            _discordHelper = discordHelper;
        }

        public static void SetTemplateHelper(ITemplateHelper templateHelper) {
            _templateHelper = templateHelper;
        }

        public static void SetFileHelper(IFileHelper fileHelper) {
            _fileHelper = fileHelper;
        }

        public static void ClearHelpers() {
            _discordHelper = null;
            _templateHelper = null;
            _fileHelper = null;
        }
    }
}
