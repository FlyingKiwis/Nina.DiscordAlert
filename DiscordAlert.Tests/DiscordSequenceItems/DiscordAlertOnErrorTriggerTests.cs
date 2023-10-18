using NUnit.Framework;
using Moq;
using NINA.Sequencer.SequenceItem;
using NINA.DiscordAlert.DiscordAlertSequenceItems;
using NINA.DiscordAlert.Util;
using System.Collections.Generic;
using Discord;
using NINA.Sequencer;
using System;
using NINA.DiscordAlert.SequenceFailureMonitor;
using NINA.DiscordAlert.DiscordWebhook;
using MessageType = NINA.DiscordAlert.DiscordWebhook.MessageType;
using System.Threading;
using NINA.DiscordAlert.Images;
using System.Windows.Media.Imaging;

namespace DiscordAlert.Tests {
    [TestFixture]
    public class DiscordAlertOnErrorTriggerTests
    {
        [Test]
        public void OnFailureFired_GivenFailedItem_SendsDiscordMessage() {
            var mockFailureFactory = new Mock<ISequenceFailureMonitorFactory>();
            var mockFailure = new Mock<ISequenceFailureMonitor>();
            mockFailureFactory.Setup(o => o.Create(It.IsAny<ISequenceEntity>())).Returns(mockFailure.Object);
            Factories.SetSequenceFailureMonitorFactory(mockFailureFactory.Object);
            var mockDiscordHelper = new Mock<IDiscordHelper>();
            Helpers.SetDiscordHelper(mockDiscordHelper.Object);
            var alertOnError = new DiscordAlertOnErrorTrigger();
            var mockSequenceEntity = new Mock<ISequenceEntity>();
            mockSequenceEntity.Setup(o => o.Name).Returns("Failure Item");

            var exception = new Exception("Test exception");

            alertOnError.SequenceBlockInitialize();
            mockFailure.Raise(o => o.OnFailure += null, new SequenceFailureEventArgs(mockSequenceEntity.Object, exception));

            mockDiscordHelper.Verify(o => o.SendMessage(MessageType.Error, It.IsAny<string>(), mockSequenceEntity.Object, It.IsAny<CancellationToken>(), null, exception), Times.Once);
        }

        [Test]
        public void OnFailureFired_DiscordException_Handled() {
            var mockFailureFactory = new Mock<ISequenceFailureMonitorFactory>();
            var mockFailure = new Mock<ISequenceFailureMonitor>();
            mockFailureFactory.Setup(o => o.Create(It.IsAny<ISequenceItem>())).Returns(mockFailure.Object);
            var mockDiscordClientFactory = new Mock<IDiscordWebhookClientFactory>();
            var mockDiscordClient = new Mock<IDiscordWebhookClient>();
            mockDiscordClientFactory.Setup(o => o.Create()).Returns(mockDiscordClient.Object);
            Factories.SetSequenceFailureMonitorFactory(mockFailureFactory.Object);
            var mockDiscordHelper = new Mock<IDiscordHelper>();
            Helpers.SetDiscordHelper(mockDiscordHelper.Object);
            var alertOnError = new DiscordAlertOnErrorTrigger();
            var mockSequenceEntity = new Mock<ISequenceEntity>();
            mockSequenceEntity.Setup(o => o.Name).Returns("Failure Item");
            mockDiscordHelper.Setup(o => o.SendMessage(It.IsAny<MessageType>(), It.IsAny<string>(), It.IsAny<ISequenceEntity>(), It.IsAny<CancellationToken>(), It.IsAny<DiscordImageDetails>(), It.IsAny<Exception>())).Throws(new Exception("TEST"));

            Assert.DoesNotThrow(() => mockFailure.Raise(o => o.OnFailure += null, new SequenceFailureEventArgs(mockSequenceEntity.Object, new Exception("Test exception"))));      
        }

        [Test]
        public void SequenceBlockTeardown_MockedFailureMonitor_FailureMonitorIsDisposed() {
            var mockFailureFactory = new Mock<ISequenceFailureMonitorFactory>();
            var mockFailure = new Mock<ISequenceFailureMonitor>();
            mockFailureFactory.Setup(o => o.Create(It.IsAny<ISequenceEntity>())).Returns(mockFailure.Object);
            var mockDiscordClientFactory = new Mock<IDiscordWebhookClientFactory>();
            var mockDiscordClient = new Mock<IDiscordWebhookClient>();
            mockDiscordClientFactory.Setup(o => o.Create()).Returns(mockDiscordClient.Object);
            Factories.SetSequenceFailureMonitorFactory(mockFailureFactory.Object);
            var alertOnError = new DiscordAlertOnErrorTrigger();

            alertOnError.SequenceBlockInitialize();
            alertOnError.SequenceBlockTeardown();

            mockFailure.Verify(o => o.Dispose(), Times.Once);
        }

    }
}
