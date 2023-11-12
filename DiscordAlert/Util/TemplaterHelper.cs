using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Image.ImageData;
using NINA.Image.Interfaces;
using NINA.Sequencer;
using System;
using System.Linq;

namespace NINA.DiscordAlert.Util {
    public class TemplaterHelper : ITemplateHelper {
        public ImagePatterns GetImageTemplateValues(ImageMetaData metadata, IStarDetectionAnalysis starAnalysis) {
            var patterns = new ImagePatterns();

            patterns.Set(ImagePatternKeys.Filter, metadata.FilterWheel.Filter);
            patterns.Set(ImagePatternKeys.ExposureTime, metadata.Image.ExposureTime);
            patterns.Set(ImagePatternKeys.ApplicationStartDate, CoreUtil.ApplicationStartDate.ToString("yyyy-MM-dd"));
            patterns.Set(ImagePatternKeys.Date, metadata.Image.ExposureStart.ToLocalTime().ToString("yyyy-MM-dd"));

            // ExposureStart is initialized to DateTime.MinValue, and we cannot subtract time from that. Only evaluate
            // the $$DATEMINUS12$$ pattern if the time is at least 12 hours on from DateTime.MinValue.
            if (metadata.Image.ExposureStart > DateTime.MinValue.AddHours(12)) {
                patterns.Set(ImagePatternKeys.DateMinus12, metadata.Image.ExposureStart.ToLocalTime().AddHours(-12).ToString("yyyy-MM-dd"));
            }

            patterns.Set(ImagePatternKeys.DateUtc, metadata.Image.ExposureStart.ToUniversalTime().ToString("yyyy-MM-dd"));
            patterns.Set(ImagePatternKeys.Time, metadata.Image.ExposureStart.ToLocalTime().ToString("HH-mm-ss"));
            patterns.Set(ImagePatternKeys.TimeUtc, metadata.Image.ExposureStart.ToUniversalTime().ToString("HH-mm-ss"));
            patterns.Set(ImagePatternKeys.DateTime, metadata.Image.ExposureStart.ToLocalTime().ToString("yyyy-MM-dd_HH-mm-ss"));
            patterns.Set(ImagePatternKeys.FrameNr, metadata.Image.ExposureNumber.ToString("0000"));
            patterns.Set(ImagePatternKeys.ImageType, metadata.Image.ImageType);
            patterns.Set(ImagePatternKeys.TargetName, metadata.Target.Name);

            if (metadata.Image.RecordedRMS != null) {
                patterns.Set(ImagePatternKeys.RMS, metadata.Image.RecordedRMS.Total);
                patterns.Set(ImagePatternKeys.RMSArcSec, metadata.Image.RecordedRMS.Total * metadata.Image.RecordedRMS.Scale);
                patterns.Set(ImagePatternKeys.PeakRA, metadata.Image.RecordedRMS.PeakRA);
                patterns.Set(ImagePatternKeys.PeakRAArcSec, metadata.Image.RecordedRMS.PeakRA * metadata.Image.RecordedRMS.Scale);
                patterns.Set(ImagePatternKeys.PeakDec, metadata.Image.RecordedRMS.PeakDec);
                patterns.Set(ImagePatternKeys.PeakDecArcSec, metadata.Image.RecordedRMS.PeakDec * metadata.Image.RecordedRMS.Scale);
            }

            if (metadata.Focuser.Position.HasValue) {
                patterns.Set(ImagePatternKeys.FocuserPosition, metadata.Focuser.Position.Value);
            }

            if (!double.IsNaN(metadata.Focuser.Temperature)) {
                patterns.Set(ImagePatternKeys.FocuserTemp, metadata.Focuser.Temperature);
            }

            if (metadata.Camera.Binning == string.Empty) {
                patterns.Set(ImagePatternKeys.Binning, "1x1");
            } else {
                patterns.Set(ImagePatternKeys.Binning, metadata.Camera.Binning);
            }

            if (!double.IsNaN(metadata.Camera.Temperature)) {
                patterns.Set(ImagePatternKeys.SensorTemp, metadata.Camera.Temperature);
            }

            if (!double.IsNaN(metadata.Camera.SetPoint)) {
                patterns.Set(ImagePatternKeys.TemperatureSetPoint, metadata.Camera.SetPoint);
            }

            if (metadata.Camera.Gain >= 0) {
                patterns.Set(ImagePatternKeys.Gain, metadata.Camera.Gain);
            }

            if (metadata.Camera.Offset >= 0) {
                patterns.Set(ImagePatternKeys.Offset, metadata.Camera.Offset);
            }

            if (metadata.Camera.USBLimit >= 0) {
                patterns.Set(ImagePatternKeys.USBLimit, metadata.Camera.USBLimit);
            }

            if (!double.IsNaN(starAnalysis.HFR)) {
                patterns.Set(ImagePatternKeys.HFR, starAnalysis.HFR);
            }

            if (!double.IsNaN(metadata.WeatherData.SkyQuality)) {
                patterns.Set(ImagePatternKeys.SQM, metadata.WeatherData.SkyQuality);
            }

            if (!string.IsNullOrEmpty(metadata.Camera.ReadoutModeName)) {
                patterns.Set(ImagePatternKeys.ReadoutMode, metadata.Camera.ReadoutModeName);
            }

            if (!string.IsNullOrEmpty(metadata.Camera.Name)) {
                patterns.Set(ImagePatternKeys.Camera, metadata.Camera.Name);
            }

            if (!string.IsNullOrEmpty(metadata.Telescope.Name)) {
                patterns.Set(ImagePatternKeys.Telescope, metadata.Telescope.Name);
            }

            if (!double.IsNaN(metadata.Rotator.MechanicalPosition)) {
                patterns.Set(ImagePatternKeys.RotatorAngle, metadata.Rotator.MechanicalPosition);
            }

            if (starAnalysis.DetectedStars >= 0) {
                patterns.Set(ImagePatternKeys.StarCount, starAnalysis.DetectedStars);
            }

            patterns.Set(ImagePatternKeys.SequenceTitle, metadata.Sequence.Title);

            return patterns;
        }

        public ImagePatterns GetSequenceTemplateValues(ISequenceEntity sequenceEntity, ITelescopeMediator telescopeMediator, ICameraMediator cameraMediator, IFilterWheelMediator filterWheelMediator, IFocuserMediator focuserMediator, IRotatorMediator rotatorMediator) {
            ImagePatterns imagePatterns = new ImagePatterns();

            try {

                //Time
                var now = DateTime.Now;
                var utcnow = DateTime.UtcNow;
                imagePatterns.Set(ImagePatternKeys.ApplicationStartDate, CoreUtil.ApplicationStartDate.ToString("yyyy-MM-dd"));
                imagePatterns.Set(ImagePatternKeys.Date, now.ToString("yyyy-MM-dd"));
                imagePatterns.Set(ImagePatternKeys.DateUtc, utcnow.ToString("yyyy-MM-dd"));
                imagePatterns.Set(ImagePatternKeys.Time, now.ToString("HH-mm-ss"));
                imagePatterns.Set(ImagePatternKeys.TimeUtc, utcnow.ToString("HH-mm-ss"));
                imagePatterns.Set(ImagePatternKeys.DateTime, now.ToString("yyyy-MM-dd_HH-mm-ss"));
                if (now > DateTime.MinValue.AddHours(12.0)) {
                    imagePatterns.Set(ImagePatternKeys.DateMinus12, now.AddHours(-12.0).ToString("yyyy-MM-dd"));
                }

                //Sequence info
                var targetName = sequenceEntity?.GetDSOContainer()?.Target?.TargetName;
                if (targetName != null) {
                    imagePatterns.Set(ImagePatternKeys.TargetName, targetName);
                }
                var sequenceTitle = sequenceEntity?.GetRootContainer()?.Name;
                if (sequenceTitle != null) {
                    imagePatterns.Set(ImagePatternKeys.SequenceTitle, sequenceTitle);
                }

                //Telescope
                var telescopeInfo = telescopeMediator?.GetInfo();
                var telescopeName = telescopeInfo?.Name;
                if (telescopeName != null) {
                    imagePatterns.Set(ImagePatternKeys.Telescope, telescopeName);
                }

                //Camera
                var cameraInfo = cameraMediator?.GetInfo();
                if (cameraInfo != null) {
                    var cameraName = cameraInfo?.Name;
                    if (cameraName != null) {
                        imagePatterns.Set(ImagePatternKeys.Camera, cameraName);
                    }

                    imagePatterns.Set(ImagePatternKeys.Binning, $"{cameraInfo.BinX}x{cameraInfo.BinY}");
                    imagePatterns.Set(ImagePatternKeys.Gain, cameraInfo.Gain);
                    imagePatterns.Set(ImagePatternKeys.Offset, cameraInfo.Offset);

                    var cameraReadoutMode = cameraInfo.ReadoutMode;
                    var cameraReadoutModes = cameraInfo.ReadoutModes;
                    if (cameraReadoutModes != null && cameraReadoutModes.Count() > cameraReadoutMode) {
                        imagePatterns.Set(ImagePatternKeys.ReadoutMode, cameraReadoutModes.ToArray()[cameraReadoutMode]);
                    }

                    imagePatterns.Set(ImagePatternKeys.SensorTemp, cameraInfo.Temperature);
                    imagePatterns.Set(ImagePatternKeys.TemperatureSetPoint, cameraInfo.TemperatureSetPoint);
                    imagePatterns.Set(ImagePatternKeys.USBLimit, cameraInfo.USBLimit);
                }

                //Filter wheel
                var filterName = filterWheelMediator?.GetInfo()?.SelectedFilter?.Name;
                if (filterName != null) {
                    imagePatterns.Set(ImagePatternKeys.Filter, filterName);
                }

                //Focuser
                var focuserInfo = focuserMediator?.GetInfo();
                if (focuserInfo != null) {
                    imagePatterns.Set(ImagePatternKeys.FocuserPosition, focuserInfo.Position);
                    imagePatterns.Set(ImagePatternKeys.FocuserTemp, focuserInfo.Temperature);
                }

                //Rotator
                var rotationAngle = rotatorMediator?.GetInfo()?.Position;
                if (rotationAngle.HasValue) {
                    imagePatterns.Set(ImagePatternKeys.RotatorAngle, rotationAngle.Value);
                }

                return imagePatterns;
            }
            catch (Exception ex) {
                Logger.Error("Isue generating file patters, using empty patterns instead", ex);
                return new ImagePatterns();
            }
        }

        public string ReplaceTemplate(string template, ImagePatterns patterns) {
            var text = template;
            foreach (var pattern in patterns.Items) {
                text = text.Replace(pattern.Key, pattern.Value);
            }

            Logger.Info($"Template={template} Result={text}");
            return text;
        }
    }
}
