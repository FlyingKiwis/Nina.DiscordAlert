using NINA.WPF.Base.Interfaces.Mediator;

namespace NINA.DiscordAlert.Images {
    public static class ImageSavedEventArgsExtensions 
    {
        public static ISavedImageContainer ToContainer(this ImageSavedEventArgs args) {
            return new SavedImageContainer(args);
        }
    }
}
