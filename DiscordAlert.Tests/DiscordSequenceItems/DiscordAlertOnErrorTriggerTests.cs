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

namespace DiscordAlert.Tests {
    [TestFixture]
    public class DiscordAlertOnErrorTriggerTests
    {
        [Test]
        public void OnFailureFired_GivenFailedItem_SendsDiscordMessage() {
            var mockFailureFactory = new Mock<ISequenceFailureMonitorFactory>();
            var mockFailure = new Mock<ISequenceFailureMonitor>();
            mockFailureFactory.Setup(o => o.Create(It.IsAny<ISequenceEntity>())).Returns(mockFailure.Object);
            var mockDiscordClientFactory = new Mock<IDiscordWebhookClientFactory>();
            var mockDiscordClient = new Mock<IDiscordWebhookClient>();
            mockDiscordClientFactory.Setup(o => o.Create()).Returns(mockDiscordClient.Object);
            Factories.SetDiscordClientFactory(mockDiscordClientFactory.Object);
            Factories.SetSequenceFailureMonitorFactory(mockFailureFactory.Object);
            var alertOnError = new DiscordAlertOnErrorTrigger();
            var mockSequenceEntity = new Mock<ISequenceEntity>();
            mockSequenceEntity.Setup(o => o.Name).Returns("Failure Item");
            mockDiscordClient.Setup(o => o.SendSimpleMessageAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Embed>>()))
                .Callback((string text, IEnumerable<Embed> embeds) => HandleSendMessage(text, embeds, alertOnError.Text, "Failure Item", "Test exception"));

            alertOnError.SequenceBlockInitialize();
            mockFailure.Raise(o => o.OnFailure += null, new SequenceFailureEventArgs(mockSequenceEntity.Object, new Exception("Test exception")));

            mockDiscordClient.Verify(o => o.SendSimpleMessageAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Embed>>()), Times.Once);
        }

        [Test]
        public void OnFailureFired_DiscordException_Handled() {
            var mockFailureFactory = new Mock<ISequenceFailureMonitorFactory>();
            var mockFailure = new Mock<ISequenceFailureMonitor>();
            mockFailureFactory.Setup(o => o.Create(It.IsAny<ISequenceItem>())).Returns(mockFailure.Object);
            var mockDiscordClientFactory = new Mock<IDiscordWebhookClientFactory>();
            var mockDiscordClient = new Mock<IDiscordWebhookClient>();
            mockDiscordClientFactory.Setup(o => o.Create()).Returns(mockDiscordClient.Object);
            Factories.SetDiscordClientFactory(mockDiscordClientFactory.Object);
            Factories.SetSequenceFailureMonitorFactory(mockFailureFactory.Object);
            var alertOnError = new DiscordAlertOnErrorTrigger();
            var mockSequenceEntity = new Mock<ISequenceEntity>();
            mockSequenceEntity.Setup(o => o.Name).Returns("Failure Item");
            mockDiscordClient.Setup(o => o.SendSimpleMessageAsync(It.IsAny<string>(), It.IsAny<IEnumerable<Embed>>())).Throws(new Exception("TEST"));

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

        private void HandleSendMessage(string actualText, IEnumerable<Embed> actualEmbeds, string expectedText, string expectedEntityName, string expectedIssue) 
        {
            Assert.AreEqual(expectedText, actualText);

            var fieldMatches = 0;
            foreach (var embed in actualEmbeds) {
                foreach (var field in embed.Fields) {
                    if (field.Name == "Failing Step") {
                        Assert.AreEqual(expectedEntityName, field.Value);
                        fieldMatches++;
                    }

                    if(field.Name == "Issues") {
                        Assert.IsTrue(field.Value.Contains(expectedIssue));
                        fieldMatches++;
                    }
                }
            }

            if(fieldMatches != 2) {
                Assert.Fail("Not all expected fields were in the embed");
            }
        }
    }
}
