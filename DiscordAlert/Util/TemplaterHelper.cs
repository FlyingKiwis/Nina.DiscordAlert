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
        public ImagePatterns GetImageTemplateValues(IRenderedImage image) {
            if (image.RawImageData is not BaseImageData imageData)
                return new ImagePatterns();
            
            return imageData.GetImagePatterns();
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
    }
}
