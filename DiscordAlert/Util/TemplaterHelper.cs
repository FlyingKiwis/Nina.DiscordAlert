using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Image.ImageData;
using NINA.Image.Interfaces;
using NINA.Profile.Interfaces;
using NINA.Sequencer;
using System;

namespace NINA.DiscordAlert.Util {
    public class TemplaterHelper : ITemplateHelper {
        public ImagePatterns GetImageTemplateValues(IRenderedImage image) {
            if (image.RawImageData is not BaseImageData imageData)
                return new ImagePatterns();
            
            return imageData.GetImagePatterns();
        }

        public ImagePatterns GetSequenceTemplateValues(ISequenceEntity sequenceEntity, IProfileService profileService) {
            ImagePatterns imagePatterns = new ImagePatterns();

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

            //Equipment
            var telescopeName = profileService?.ActiveProfile?.TelescopeSettings?.Name;
            if(telescopeName != null) {
                imagePatterns.Set(ImagePatternKeys.Telescope, telescopeName);
            }

            var cameraName = profileService?.ActiveProfile?.CameraSettings?.Id;
            if(cameraName != null) {
                imagePatterns.Set(ImagePatternKeys.Camera, cameraName);
            }

            return imagePatterns;
        }
    }
}
