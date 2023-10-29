using Moq;
using NINA.Core.Model;
using NINA.DiscordAlert.DiscordAlertSequenceItems;
using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.Util;
using NINA.Profile.Interfaces;
using NINA.Sequencer;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiscordAlert.Tests.DiscordSequenceItems {

    [TestFixture]
    public class DiscordMessageInstructionTests {
        [Test]
        public async Task Execute_GoodMessage_MessageSentToDiscord() {
            var expectedMessage = "some text";
            var discordHelperMock = new Mock<IDiscordHelper>();
            Helpers.SetDiscordHelper(discordHelperMock.Object);
            var templateHelperMock = new Mock<ITemplateHelper>();
            Helpers.SetTemplateHelper(templateHelperMock.Object);
            var profileServiceMock = new Mock<IProfileService>();
            var imagePatterns = new ImagePatterns();
            var discordMessageInstruction = new DiscordMessageInstruction(profileServiceMock.Object);
            templateHelperMock.Setup(o => o.GetSequenceTemplateValues(discordMessageInstruction, profileServiceMock.Object)).Returns(imagePatterns);
            var cancelationTokenSource = new CancellationTokenSource();

            discordMessageInstruction.Text = expectedMessage;
            await discordMessageInstruction.Execute(Mock.Of<IProgress<ApplicationStatus>>(), cancelationTokenSource.Token);

            discordHelperMock.Verify(o => o.SendMessage(MessageType.Information, expectedMessage, discordMessageInstruction, cancelationTokenSource.Token, imagePatterns, null, null), Times.Once);
        }

        [Test]
        public void Execute_Exception_DoesNotThrowNoMessage() {
            var expectedMessage = "some text";
            var discordHelperMock = new Mock<IDiscordHelper>();
            Helpers.SetDiscordHelper(discordHelperMock.Object);
            var templateHelperMock = new Mock<ITemplateHelper>();
            Helpers.SetTemplateHelper(templateHelperMock.Object);
            var profileServiceMock = new Mock<IProfileService>();
            var imagePatterns = new ImagePatterns();
            var discordMessageInstruction = new DiscordMessageInstruction(profileServiceMock.Object);
            templateHelperMock.Setup(o => o.GetSequenceTemplateValues(discordMessageInstruction, profileServiceMock.Object)).Throws(new Exception());
            var cancelationTokenSource = new CancellationTokenSource();

            discordMessageInstruction.Text = expectedMessage;
            Assert.DoesNotThrowAsync(async () => await discordMessageInstruction.Execute(Mock.Of<IProgress<ApplicationStatus>>(), cancelationTokenSource.Token));

            discordHelperMock.Verify(o => o.SendMessage(It.IsAny<MessageType>(), It.IsAny<string>(), It.IsAny<ISequenceEntity>(), It.IsAny<CancellationToken>(), It.IsAny<ImagePatterns>(), It.IsAny<BitmapSource>(), It.IsAny<Exception>()), Times.Never);
        }
    }
}
