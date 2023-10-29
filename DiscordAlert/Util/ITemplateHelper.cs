using NINA.Core.Model;
using NINA.Image.Interfaces;
using NINA.Profile.Interfaces;
using NINA.Sequencer;

namespace NINA.DiscordAlert.Util {
    public interface ITemplateHelper {
        public ImagePatterns GetSequenceTemplateValues(ISequenceEntity sequenceEntity, IProfileService profileService);

        public ImagePatterns GetImageTemplateValues(IRenderedImage image);
    }
}
