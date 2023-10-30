using NINA.Core.Model;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Image.Interfaces;
using NINA.Profile.Interfaces;
using NINA.Sequencer;

namespace NINA.DiscordAlert.Util {
    public interface ITemplateHelper {
        ImagePatterns GetSequenceTemplateValues(ISequenceEntity sequenceEntity, ITelescopeMediator telescopeMediator, ICameraMediator cameraMediator, IFilterWheelMediator filterWheelMediator, IFocuserMediator focuserMediator, IRotatorMediator rotatorMediator);

        public ImagePatterns GetImageTemplateValues(IRenderedImage image);
    }
}
