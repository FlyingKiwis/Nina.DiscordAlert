using NUnit.Framework;
using Moq;
using NINA.Sequencer.SequenceItem;
using NINA.DiscordAlert.DiscordAlertSequenceItems;
using NINA.Sequencer.Container;
using NINA.Core.Model;
using System.Threading;
using NINA.DiscordAlert.Util;
using System.Collections.Generic;
using Discord;
using System.Threading.Tasks;

namespace DiscordAlert.Tests
{
    [TestFixture]
    public class AlertOnErrorTests
    {
        [Test]
        public void ShouldTriggerAfter_FailedSequenceItem_ResturnTrue() {
            var sequenceItem = new Mock<ISequenceItem>();
            sequenceItem.Setup(o => o.Status).Returns(NINA.Core.Enum.SequenceEntityStatus.FAILED);
            var alertOnError = new DiscordAlertOnErrorTrigger();

            var result = alertOnError.ShouldTriggerAfter(sequenceItem.Object, null);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldTriggerAfter_FinishedSequenceItem_ResturnFalse() {
            var sequenceItem = new Mock<ISequenceItem>();
            sequenceItem.Setup(o => o.Status).Returns(NINA.Core.Enum.SequenceEntityStatus.FINISHED);
            var alertOnError = new DiscordAlertOnErrorTrigger();

            var result = alertOnError.ShouldTriggerAfter(sequenceItem.Object, null);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldTriggerAfter_NullSequenceItem_ResturnFalse() {
            var alertOnError = new DiscordAlertOnErrorTrigger();

            var result = alertOnError.ShouldTriggerAfter(null, null);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Execute_GivenFailedSequence_SendsDiscordMessage() {
            var mockWebhookClient = new Mock<IDiscordWebhookClient>();
            var sequenceItem = new Mock<ISequenceItem>();
            sequenceItem.Setup(o => o.Status).Returns(NINA.Core.Enum.SequenceEntityStatus.FAILED);
            var alertOnError = new DiscordAlertOnErrorTrigger();
            alertOnError.ShouldTriggerAfter(sequenceItem.Object, null);
            var cancelSource = new CancellationTokenSource();
            DiscordResources.SetWebsocketClient(mockWebhookClient.Object);

            await alertOnError.Execute(Mock.Of<ISequenceContainer>(), Mock.Of<System.IProgress<ApplicationStatus>>(), cancelSource.Token);

            mockWebhookClient.Verify(o => o.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<Embed>>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<RequestOptions>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<MessageFlags>(), It.IsAny<ulong?>()), Times.Once);
        }
    }
}
