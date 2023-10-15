using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Image.ImageData;
using NINA.WPF.Base.Interfaces.Mediator;
using System;

namespace NINA.DiscordAlert.ImageSaveMonitor {
    public class ImageSaveMonitor : IImageSaveMonitor {

        public ImageSavedEventArgs LastImage { get; private set; }

        public event EventHandler<ImageSavedEventArgs> ImageSaved;

        private readonly IImageSaveMediator _imageSaveMediator;
        public ImageSaveMonitor(IImageSaveMediator imageSaveMediator) 
        {
            _imageSaveMediator = imageSaveMediator;
            _imageSaveMediator.ImageSaved += ImageSaveMediator_ImageSaved;
        }

        private void ImageSaveMediator_ImageSaved(object sender, ImageSavedEventArgs e)
        {
            Logger.Debug($"New image saved={e.PathToImage}");
            LastImage = e;
            ImageSaved?.Invoke(sender, e);
        }

        public string ReplacePlaceholders(string textWithPlaceholders, ImageSavedEventArgs image) 
        {
            var patterns = GetImagePatterns(image);
            return patterns.GetImageFileString(textWithPlaceholders);
        }

        //This function is copied and slightly modified from NINA itself, I could not find a way that it was exposed to plugins 
        private static ImagePatterns  GetImagePatterns(ImageSavedEventArgs image) {
            var metadata = image.MetaData;
            var p = new ImagePatterns();
            p.Set(ImagePatternKeys.Filter, metadata.FilterWheel.Filter);
            p.Set(ImagePatternKeys.ExposureTime, metadata.Image.ExposureTime);
            p.Set(ImagePatternKeys.ApplicationStartDate, CoreUtil.ApplicationStartDate.ToString("yyyy-MM-dd"));
            p.Set(ImagePatternKeys.Date, metadata.Image.ExposureStart.ToLocalTime().ToString("yyyy-MM-dd"));

            // ExposureStart is initialized to DateTime.MinValue, and we cannot subtract time from that. Only evaluate
            // the $$DATEMINUS12$$ pattern if the time is at least 12 hours on from DateTime.MinValue.
            if (metadata.Image.ExposureStart > DateTime.MinValue.AddHours(12)) {
                p.Set(ImagePatternKeys.DateMinus12, metadata.Image.ExposureStart.ToLocalTime().AddHours(-12).ToString("yyyy-MM-dd"));
            }

            p.Set(ImagePatternKeys.DateUtc, metadata.Image.ExposureStart.ToUniversalTime().ToString("yyyy-MM-dd"));
            p.Set(ImagePatternKeys.Time, metadata.Image.ExposureStart.ToLocalTime().ToString("HH-mm-ss"));
            p.Set(ImagePatternKeys.TimeUtc, metadata.Image.ExposureStart.ToUniversalTime().ToString("HH-mm-ss"));
            p.Set(ImagePatternKeys.DateTime, metadata.Image.ExposureStart.ToLocalTime().ToString("yyyy-MM-dd_HH-mm-ss"));
            p.Set(ImagePatternKeys.FrameNr, metadata.Image.ExposureNumber.ToString("0000"));
            p.Set(ImagePatternKeys.ImageType, metadata.Image.ImageType);
            p.Set(ImagePatternKeys.TargetName, metadata.Target.Name);

            if (metadata.Image.RecordedRMS != null) {
                p.Set(ImagePatternKeys.RMS, metadata.Image.RecordedRMS.Total);
                p.Set(ImagePatternKeys.RMSArcSec, metadata.Image.RecordedRMS.Total * metadata.Image.RecordedRMS.Scale);
                p.Set(ImagePatternKeys.PeakRA, metadata.Image.RecordedRMS.PeakRA);
                p.Set(ImagePatternKeys.PeakRAArcSec, metadata.Image.RecordedRMS.PeakRA * metadata.Image.RecordedRMS.Scale);
                p.Set(ImagePatternKeys.PeakDec, metadata.Image.RecordedRMS.PeakDec);
                p.Set(ImagePatternKeys.PeakDecArcSec, metadata.Image.RecordedRMS.PeakDec * metadata.Image.RecordedRMS.Scale);
            }

            if (metadata.Focuser.Position.HasValue) {
                p.Set(ImagePatternKeys.FocuserPosition, metadata.Focuser.Position.Value);
            }

            if (!double.IsNaN(metadata.Focuser.Temperature)) {
                p.Set(ImagePatternKeys.FocuserTemp, metadata.Focuser.Temperature);
            }

            if (metadata.Camera.Binning == string.Empty) {
                p.Set(ImagePatternKeys.Binning, "1x1");
            } else {
                p.Set(ImagePatternKeys.Binning, metadata.Camera.Binning);
            }

            if (!double.IsNaN(metadata.Camera.Temperature)) {
                p.Set(ImagePatternKeys.SensorTemp, metadata.Camera.Temperature);
            }

            if (!double.IsNaN(metadata.Camera.SetPoint)) {
                p.Set(ImagePatternKeys.TemperatureSetPoint, metadata.Camera.SetPoint);
            }

            if (metadata.Camera.Gain >= 0) {
                p.Set(ImagePatternKeys.Gain, metadata.Camera.Gain);
            }

            if (metadata.Camera.Offset >= 0) {
                p.Set(ImagePatternKeys.Offset, metadata.Camera.Offset);
            }

            if (metadata.Camera.USBLimit >= 0) {
                p.Set(ImagePatternKeys.USBLimit, metadata.Camera.USBLimit);
            }

            if (!double.IsNaN(image.StarDetectionAnalysis.HFR)) {
                p.Set(ImagePatternKeys.HFR, image.StarDetectionAnalysis.HFR);
            }

            if (!double.IsNaN(metadata.WeatherData.SkyQuality)) {
                p.Set(ImagePatternKeys.SQM, metadata.WeatherData.SkyQuality);
            }

            if (!string.IsNullOrEmpty(metadata.Camera.ReadoutModeName)) {
                p.Set(ImagePatternKeys.ReadoutMode, metadata.Camera.ReadoutModeName);
            }

            if (!string.IsNullOrEmpty(metadata.Camera.Name)) {
                p.Set(ImagePatternKeys.Camera, metadata.Camera.Name);
            }

            if (!string.IsNullOrEmpty(metadata.Telescope.Name)) {
                p.Set(ImagePatternKeys.Telescope, metadata.Telescope.Name);
            }

            if (!double.IsNaN(metadata.Rotator.MechanicalPosition)) {
                p.Set(ImagePatternKeys.RotatorAngle, metadata.Rotator.MechanicalPosition);
            }

            if (image.StarDetectionAnalysis.DetectedStars >= 0) {
                p.Set(ImagePatternKeys.StarCount, image.StarDetectionAnalysis.DetectedStars);
            }

            p.Set(ImagePatternKeys.SequenceTitle, metadata.Sequence.Title);

            return p;
        }
    }
}
