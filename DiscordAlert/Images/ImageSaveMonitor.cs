using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.WPF.Base.Interfaces.Mediator;
using System;

namespace NINA.DiscordAlert.Images {
    public class ImageSaveMonitor : IImageSaveMonitor {

        public ISavedImageContainer LastImage { get; private set; }

        public event EventHandler<ISavedImageContainer> ImageSaved;

        private readonly IImageSaveMediator _imageSaveMediator;
        public ImageSaveMonitor(IImageSaveMediator imageSaveMediator) 
        {
            if(imageSaveMediator == null) {
                throw new ArgumentNullException(nameof(imageSaveMediator));
            }

            _imageSaveMediator = imageSaveMediator;
            _imageSaveMediator.ImageSaved += ImageSaveMediator_ImageSaved;
        }

        private void ImageSaveMediator_ImageSaved(object sender, ImageSavedEventArgs e)
        {
            Logger.Debug($"New image saved={e?.PathToImage?.ToString() ?? "none"}");
            var image = new SavedImageContainer(e);
            LastImage = image;
            ImageSaved?.Invoke(sender, image);
        }

        public void Dispose() {
            try {
                _imageSaveMediator.ImageSaved -= ImageSaveMediator_ImageSaved;
            }
            catch { }
        }
    }
}
