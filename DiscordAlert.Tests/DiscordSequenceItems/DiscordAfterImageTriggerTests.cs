using Discord;
using DiscordAlert.Tests.Utility;
using Moq;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.DiscordAlert.DiscordAlertSequenceItems;
using NINA.DiscordAlert.DiscordWebhook;
using NINA.DiscordAlert.Images;
using NINA.DiscordAlert.Util;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Image.Interfaces;
using NINA.Profile.Interfaces;
using NINA.Sequencer.Container;
using NINA.Sequencer.Interfaces;
using NINA.Sequencer.SequenceItem;
using NINA.WPF.Base.Interfaces.Mediator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace DiscordAlert.Tests.DiscordSequenceItems {
    [TestFixture]
    public class DiscordMessageAfterImageTriggerTests {
        [Test]
        public void SequenceBlockInitialized_Always_ListenToImageSaveMonitor() {
            var imageSaveMediator = new Mock<IImageSaveMediator>();
            
            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator.Object, Mock.Of<IImagingMediator>(), Mock.Of<IImageDataFactory>(), Mock.Of<IProfileService>());
            discordMessageAfterImage.SequenceBlockInitialize();

            imageSaveMediator.VerifyAdd(o => o.ImageSaved += It.IsAny<EventHandler<ImageSavedEventArgs>>(), Times.Once);          
        }

        [Test]
        public void ShouldTrigger_NextItemIsAnExposureItem_True() {
            var imageSaveMediator = Mock.Of<IImageSaveMediator>();

            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator, Mock.Of<IImagingMediator>(), Mock.Of<IImageDataFactory>(), Mock.Of<IProfileService>());
            var result = discordMessageAfterImage.ShouldTrigger(Mock.Of<ISequenceItem>(), Mock.Of<IExposureItem>());

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldTrigger_NextItemIsNotAnExposureItem_False() {
            var imageSaveMediator = Mock.Of<IImageSaveMediator>();

            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator, Mock.Of<IImagingMediator>(), Mock.Of<IImageDataFactory>(), Mock.Of<IProfileService>());
            var result = discordMessageAfterImage.ShouldTrigger(Mock.Of<ISequenceItem>(), Mock.Of<ISequenceItem>());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task ImageMonitorImageSaved_AfterEnabled_SendsADiscordMessage() {
            var imageSaveMediator = new Mock<IImageSaveMediator>();
            var imageDataFactoryMock = new Mock<IImageDataFactory>();
            var imagingMediatorMock = new Mock<IImagingMediator>(); 
            var discordClientFactory = new Mock<IDiscordWebhookClientFactory>();
            var profileServiceMock = new Mock<IProfileService>();
            var activeProfileMock = new Mock<IProfile>();
            var cameraSettingsMock = new Mock<ICameraSettings>();
            var discordClient = new Mock<IDiscordWebhookClient>();
            var savedImageMock = new Mock<ISavedImageContainer>();
            var imageDataMock = new Mock<IImageData>();
            var renderedImageMock = new Mock<IRenderedImage>();
            var discordHelperMock = new Mock<IDiscordHelper>();
            discordClientFactory.Setup(o => o.Create()).Returns(discordClient.Object);
            Factories.SetDiscordClientFactory(discordClientFactory.Object);
            var cancelTokenSource = new CancellationTokenSource();
            var pathUri = new Uri("C:/test/path");
            var pixelFormat = new PixelFormat();
            pixelFormat = PixelFormats.Pbgra32;
            var bitmap = new WriteableBitmap(100, 100, 10, 10, pixelFormat, new BitmapPalette(new List<Color>() { Colors.Red }));
            savedImageMock.Setup(o => o.PathToImage).Returns(pathUri);
            savedImageMock.Setup(o => o.IsBayered).Returns(true);
            profileServiceMock.Setup(o => o.ActiveProfile).Returns(activeProfileMock.Object);
            activeProfileMock.Setup(o => o.CameraSettings).Returns(cameraSettingsMock.Object);
            cameraSettingsMock.Setup(o => o.BitDepth).Returns(42);
            cameraSettingsMock.Setup(o => o.RawConverter).Returns(NINA.Core.Enum.RawConverterEnum.FREEIMAGE);
            imageDataFactoryMock.Setup(o => o.CreateFromFile(pathUri.AbsolutePath, 42, true, NINA.Core.Enum.RawConverterEnum.FREEIMAGE, It.IsAny<CancellationToken>())).ReturnsAsync(imageDataMock.Object);
            var imagedSaveArgs = ImageTestUtility.GenerateImageSavedEventArgs(savedImageMock.Object);
            imagingMediatorMock.Setup(o => o.PrepareImage(imageDataMock.Object, It.IsAny<PrepareImageParameters>(), It.IsAny<CancellationToken>())).ReturnsAsync(renderedImageMock.Object);
            renderedImageMock.Setup(o => o.Image).Returns(bitmap);
            Helpers.SetDiscordHelper(discordHelperMock.Object);

            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator.Object, imagingMediatorMock.Object, imageDataFactoryMock.Object, profileServiceMock.Object);
            discordMessageAfterImage.SequenceBlockInitialize();
            discordMessageAfterImage.ShouldTrigger(Mock.Of<ISequenceItem>(), Mock.Of<IExposureItem>());
            await discordMessageAfterImage.Execute(Mock.Of<ISequenceContainer>(), Mock.Of<IProgress<ApplicationStatus>>(), cancelTokenSource.Token);
            imageSaveMediator.Raise(o => o.ImageSaved += null, imagedSaveArgs);

            discordHelperMock.Verify(o => o.SendMessage(NINA.DiscordAlert.DiscordWebhook.MessageType.Information, It.IsAny<string>(), It.IsAny<ISequenceItem>(), It.IsAny<CancellationToken>(), It.IsAny<ISavedImageContainer>(), bitmap, null), Times.Once);
        }

        [Test]
        public void ImageMonitorImageSaved_NotEnabled_RemovesListener() 
        {
            var imageSaveMediator = new Mock<IImageSaveMediator>();
            var imageDataFactoryMock = new Mock<IImageDataFactory>();
            var imagingMediatorMock = new Mock<IImagingMediator>();
            var profileServiceMock = new Mock<IProfileService>();
            var savedImageMock = new Mock<ISavedImageContainer>();
            var imagedSaveArgs = ImageTestUtility.GenerateImageSavedEventArgs(savedImageMock.Object);

            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator.Object, imagingMediatorMock.Object, imageDataFactoryMock.Object, profileServiceMock.Object);
            discordMessageAfterImage.SequenceBlockInitialize();
            discordMessageAfterImage.SequenceBlockTeardown();
            imageSaveMediator.Raise(o => o.ImageSaved += null, imagedSaveArgs);

            imageSaveMediator.VerifyRemove(o => o.ImageSaved -= It.IsAny<EventHandler<ImageSavedEventArgs>>(), Times.Exactly(2)); //Twice because we do a remove/add to start with
        }
    }
}
