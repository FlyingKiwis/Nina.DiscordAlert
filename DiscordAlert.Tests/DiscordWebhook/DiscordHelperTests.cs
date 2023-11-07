using Discord;
using Moq;
using NINA.DiscordAlert.Images;
using NUnit.Framework;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.Util;
using NINA.Sequencer;
using NINA.Core.Model;
using System.Threading;
using MessageType = NINA.DiscordAlert.DiscordWebhook.MessageType;
using System.Threading.Tasks;
using System;
using System.Linq;
using NINA.Sequencer.Container;
using NINA.Astrometry;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace DiscordAlert.Tests.DiscordWebhook {
    [TestFixture]
    public class DiscordHelperTests {
        [Test]
        public async Task SendMessage_InfoMessageWithImage_SendsCorrectly() 
        {
            var mre = new ManualResetEvent(false);
            var expectedMessage = "This is the expected message $$TEST$$";
            var filename = "nina.jpg";
            var expectedFilename = $"attachment://{filename}";
            var expectedResult = expectedMessage.Replace("$$TEST$$", "with replacement");
            var cancelTokenSource = new CancellationTokenSource();
            var templatePatterns = new ImagePatterns();
            templatePatterns.Add(new ImagePattern("$$TEST$$", "test"));
            templatePatterns.Set("$$TEST$$", "with replacement");
            var sequenceItemMock = new Mock<ISequenceEntity>();
            var dsoContainer = new Mock<IDeepSkyObjectContainer>();
            sequenceItemMock.SetupGet(o => o.Parent).Returns(dsoContainer.Object);
            var target = new InputTarget(Angle.Zero, Angle.Zero, null);
            var expectedTargetName = "test target";
            target.TargetName = expectedTargetName;
            dsoContainer.Setup(o => o.Target).Returns(target);
            var expectedSequenceName = "I'm a sequence";
            dsoContainer.Setup(o => o.Name).Returns(expectedSequenceName);
            var pixelFormat = new PixelFormat();
            pixelFormat = PixelFormats.Pbgra32;
            var bitmap = new WriteableBitmap(100, 100, 10, 10, pixelFormat, new BitmapPalette(new List<Color>() { Color.FromArgb(0, 0, 0, 0) }));
            var temporaryImageWriterFactoryMock = new Mock<ITemporaryImageFileWriterFactory>();
            var temporaryImageWriterMock = new Mock<ITemporaryImageFileWriter>();
            temporaryImageWriterFactoryMock.Setup(o => o.Create(bitmap)).Returns(temporaryImageWriterMock.Object);
            temporaryImageWriterMock.SetupGet(o => o.Filename).Returns(filename);
            Factories.SetTemporaryImageFileWriterFactory(temporaryImageWriterFactoryMock.Object);
            var discordClientFactoryMock = new Mock<IDiscordWebhookClientFactory>();
            var discordClientMock = new Mock<IDiscordWebhookClient>();
            discordClientFactoryMock.Setup(o => o.Create()).Returns(discordClientMock.Object);
            discordClientMock.Setup(o => o.SendFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Embed>>())).Callback((string filename, string text, IEnumerable<Embed> embeds) => {
                Assert.AreEqual(expectedResult, text);
                var embed = embeds.First();
                Assert.AreEqual(expectedFilename, embed.Image.Value.Url);
                Assert.AreEqual(Discord.Color.Blue, embed.Color);
                var hasTargetName = false;
                var hasSeqName = false;
                foreach(var field in embed.Fields) 
                {
                    var fieldName = field.Name;
                    switch(fieldName) {
                        case "Target":
                            hasTargetName = true;
                            Assert.AreEqual(expectedTargetName, field.Value);
                            break;
                        case "Sequence Name":
                            hasSeqName = true;
                            Assert.AreEqual(expectedSequenceName, field.Value);
                            break;
                    }
                }
                Assert.IsTrue(hasTargetName);
                Assert.IsTrue(hasSeqName);
                mre.Set();
            });
            Factories.SetDiscordClientFactory(discordClientFactoryMock.Object);

            var discordHelper = new DiscordHelper();
            await discordHelper.SendMessage(MessageType.Information, expectedMessage, sequenceItemMock.Object, cancelTokenSource.Token, attachedImage: bitmap, templateValues: templatePatterns);
        
            Assert.IsTrue(mre.WaitOne(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task SendMessage_InfoMessageWithoutImage_SendsCorrectly() 
        {
            var mre = new ManualResetEvent(false);
            var expectedMessage = "This is the expected message $$TEST$$";
            var expectedResult = expectedMessage.Replace("$$TEST$$", "with replacement");
            var cancelTokenSource = new CancellationTokenSource();
            var templatePatterns = new ImagePatterns();
            templatePatterns.Add(new ImagePattern("$$TEST$$", "test"));
            templatePatterns.Set("$$TEST$$", "with replacement");
            var sequenceItemMock = new Mock<ISequenceEntity>();
            var dsoContainer = new Mock<IDeepSkyObjectContainer>();
            sequenceItemMock.SetupGet(o => o.Parent).Returns(dsoContainer.Object);
            var target = new InputTarget(Angle.Zero, Angle.Zero, null);
            var expectedTargetName = "test target";
            target.TargetName = expectedTargetName;
            dsoContainer.Setup(o => o.Target).Returns(target);
            var expectedSequenceName = "I'm a sequence";
            dsoContainer.Setup(o => o.Name).Returns(expectedSequenceName);
            var discordClientFactoryMock = new Mock<IDiscordWebhookClientFactory>();
            var discordClientMock = new Mock<IDiscordWebhookClient>();
            discordClientFactoryMock.Setup(o => o.Create()).Returns(discordClientMock.Object);
            discordClientMock.Setup(o => o.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<Embed>>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<RequestOptions>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<MessageFlags>(), It.IsAny<ulong?>())).Callback(
                (string message, bool tts, IEnumerable<Embed> embeds, string username, string avatarUrl, RequestOptions requestOptions, AllowedMentions allowedMentions,
                MessageComponent messageComponents, MessageFlags flags, ulong? threadId) => {
                    Assert.AreEqual(expectedResult, message);
                    var embed = embeds.First();
                    Assert.AreEqual(Discord.Color.Blue, embed.Color);
                    var hasTargetName = false;
                    var hasSeqName = false;
                    foreach (var field in embed.Fields) {
                        var fieldName = field.Name;
                        switch (fieldName) {
                            case "Target":
                                hasTargetName = true;
                                Assert.AreEqual(expectedTargetName, field.Value);
                                break;
                            case "Sequence Name":
                                hasSeqName = true;
                                Assert.AreEqual(expectedSequenceName, field.Value);
                                break;
                        }
                    }
                    Assert.IsTrue(hasTargetName);
                    Assert.IsTrue(hasSeqName);
                    mre.Set();
                });
            Factories.SetDiscordClientFactory(discordClientFactoryMock.Object);

            var discordHelper = new DiscordHelper();
            await discordHelper.SendMessage(MessageType.Information, expectedMessage, sequenceItemMock.Object, cancelTokenSource.Token, templateValues: templatePatterns);

            Assert.IsTrue(mre.WaitOne(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task SendMessage_InfoMessageWithoutTemplate_SendsCorrectlyWithPlaceholders() {
            var mre = new ManualResetEvent(false);
            var expectedMessage = "This is the expected message $$TEST$$";
            var expectedResult = expectedMessage;
            var cancelTokenSource = new CancellationTokenSource();
            var sequenceItemMock = new Mock<ISequenceEntity>();
            var dsoContainer = new Mock<IDeepSkyObjectContainer>();
            sequenceItemMock.SetupGet(o => o.Parent).Returns(dsoContainer.Object);
            var target = new InputTarget(Angle.Zero, Angle.Zero, null);
            var expectedTargetName = "test target";
            target.TargetName = expectedTargetName;
            dsoContainer.Setup(o => o.Target).Returns(target);
            var expectedSequenceName = "I'm a sequence";
            dsoContainer.Setup(o => o.Name).Returns(expectedSequenceName);
            var discordClientFactoryMock = new Mock<IDiscordWebhookClientFactory>();
            var discordClientMock = new Mock<IDiscordWebhookClient>();
            discordClientFactoryMock.Setup(o => o.Create()).Returns(discordClientMock.Object);
            discordClientMock.Setup(o => o.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<Embed>>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<RequestOptions>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<MessageFlags>(), It.IsAny<ulong?>())).Callback(
                (string message, bool tts, IEnumerable<Embed> embeds, string username, string avatarUrl, RequestOptions requestOptions, AllowedMentions allowedMentions,
                MessageComponent messageComponents, MessageFlags flags, ulong? threadId) => {
                    Assert.AreEqual(expectedResult, message);
                    var embed = embeds.First();
                    Assert.AreEqual(Discord.Color.Blue, embed.Color);
                    var hasTargetName = false;
                    var hasSeqName = false;
                    foreach (var field in embed.Fields) {
                        var fieldName = field.Name;
                        switch (fieldName) {
                            case "Target":
                                hasTargetName = true;
                                Assert.AreEqual(expectedTargetName, field.Value);
                                break;
                            case "Sequence Name":
                                hasSeqName = true;
                                Assert.AreEqual(expectedSequenceName, field.Value);
                                break;
                        }
                    }
                    Assert.IsTrue(hasTargetName);
                    Assert.IsTrue(hasSeqName);
                    mre.Set();
                });
            Factories.SetDiscordClientFactory(discordClientFactoryMock.Object);

            var discordHelper = new DiscordHelper();
            await discordHelper.SendMessage(MessageType.Information, expectedMessage, sequenceItemMock.Object, cancelTokenSource.Token);

            Assert.IsTrue(mre.WaitOne(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task SendMessage_ErrorMessage_SendsCorrectly() {
            var mre = new ManualResetEvent(false);
            var expectedMessage = "This is the expected message";
            var expectedResult = expectedMessage;
            var cancelTokenSource = new CancellationTokenSource();
            var expectedFailingStep = "This is the failing step";
            var sequenceItemMock = new Mock<ISequenceEntity>();
            sequenceItemMock.Setup(o => o.Name).Returns(expectedFailingStep);
            var dsoContainer = new Mock<IDeepSkyObjectContainer>();
            sequenceItemMock.SetupGet(o => o.Parent).Returns(dsoContainer.Object);
            var target = new InputTarget(Angle.Zero, Angle.Zero, null);
            var expectedTargetName = "test target";
            var expectedExceptionText = "This is an expected exception";
            var exception = new Exception(expectedExceptionText);
            target.TargetName = expectedTargetName;
            dsoContainer.Setup(o => o.Target).Returns(target);
            var expectedSequenceName = "I'm a sequence";
            dsoContainer.Setup(o => o.Name).Returns(expectedSequenceName);
            var discordClientFactoryMock = new Mock<IDiscordWebhookClientFactory>();
            var discordClientMock = new Mock<IDiscordWebhookClient>();
            discordClientFactoryMock.Setup(o => o.Create()).Returns(discordClientMock.Object);
            discordClientMock.Setup(o => o.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<Embed>>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<RequestOptions>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<MessageFlags>(), It.IsAny<ulong?>())).Callback(
                (string message, bool tts, IEnumerable<Embed> embeds, string username, string avatarUrl, RequestOptions requestOptions, AllowedMentions allowedMentions,
                MessageComponent messageComponents, MessageFlags flags, ulong? threadId) => {
                    Assert.AreEqual(expectedResult, message);
                    var embed = embeds.First();
                    Assert.AreEqual(Discord.Color.Red, embed.Color);
                    var hasTargetName = false;
                    var hasSeqName = false;
                    var hasFailStep = false;
                    var hasException = false;
                    foreach (var field in embed.Fields) {
                        var fieldName = field.Name;
                        switch (fieldName) {
                            case "Target":
                                hasTargetName = true;
                                Assert.AreEqual(expectedTargetName, field.Value);
                                break;
                            case "Sequence Name":
                                hasSeqName = true;
                                Assert.AreEqual(expectedSequenceName, field.Value);
                                break;
                            case "Failing Step":
                                hasFailStep = true;
                                Assert.AreEqual(expectedFailingStep, field.Value);
                                break;
                            case "Issues":
                                hasException = true;
                                Assert.IsTrue(field.Value.Contains(expectedExceptionText));
                                break;
                        }
                    }
                    Assert.IsTrue(hasTargetName);
                    Assert.IsTrue(hasSeqName);
                    Assert.IsTrue(hasFailStep);
                    Assert.IsTrue(hasException);
                    mre.Set();
                });
            Factories.SetDiscordClientFactory(discordClientFactoryMock.Object);

            var discordHelper = new DiscordHelper();
            await discordHelper.SendMessage(MessageType.Error, expectedMessage, sequenceItemMock.Object, cancelTokenSource.Token, exception: exception);

            Assert.IsTrue(mre.WaitOne(TimeSpan.FromSeconds(5)));
        }

    }

    /*
                

    */
}
