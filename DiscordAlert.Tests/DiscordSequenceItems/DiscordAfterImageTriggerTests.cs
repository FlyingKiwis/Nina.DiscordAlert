using DiscordAlert.Tests.TestUtility;
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
using NINA.Sequencer;

namespace DiscordAlert.Tests.DiscordSequenceItems {
    [TestFixture]
    public class DiscordMessageAfterImageTriggerTests : BaseTextFixture {
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

            var cancelTokenSource = new CancellationTokenSource();
            var pathToImage = "C:/test/path.png";
            var expectedText = "This is a test";
            var expectedTempPath = "C:/temp/path.png";
            var expectedContainer = Mock.Of<ISequenceContainer>();
            var pixelFormat = new PixelFormat();
            pixelFormat = PixelFormats.Pbgra32;
            var bitmap = new WriteableBitmap(100, 100, 10, 10, pixelFormat, new BitmapPalette(new List<Color>() { Colors.Red }));
            var imagePatterns = new ImagePatterns();
            var imageSaveMediator = new Mock<IImageSaveMediator>();
            var imageDataFactoryMock = new Mock<IImageDataFactory>();
            var imagingMediatorMock = new Mock<IImagingMediator>(); 
            var discordClientFactory = new Mock<IDiscordWebhookClientFactory>();
            var profileServiceMock = new Mock<IProfileService>();
            var activeProfileMock = new Mock<IProfile>();
            profileServiceMock.Setup(o => o.ActiveProfile).Returns(activeProfileMock.Object);
            var cameraSettingsMock = new Mock<ICameraSettings>();
            var discordClient = new Mock<IDiscordWebhookClient>();
            var savedImageMock = new Mock<ISavedImageContainer>();
            var imageDataMock = new Mock<IImageData>();
            var renderedImageMock = new Mock<IRenderedImage>();
            var discordHelperMock = new Mock<IDiscordHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            fileHelperMock.Setup(o => o.Exists(pathToImage)).Returns(true);
            Helpers.SetFileHelper(fileHelperMock.Object);
            var templateHelperMock = new Mock<ITemplateHelper>();
            templateHelperMock.Setup(o => o.GetImageTemplateValues(It.IsAny<IRenderedImage>())).Returns(imagePatterns);
            Helpers.SetTemplateHelper(templateHelperMock.Object);
            discordClientFactory.Setup(o => o.Create()).Returns(discordClient.Object);
            Factories.SetDiscordClientFactory(discordClientFactory.Object);
            var tempImageWriterMock = new Mock<ITemporaryImageFileWriter>();
            tempImageWriterMock.SetupGet(o => o.Filename).Returns(expectedTempPath);
            var tempImageWriterFactoryMock = new Mock<ITemporaryImageFileWriterFactory>();
            tempImageWriterFactoryMock.Setup(o => o.Create(activeProfileMock.Object, bitmap)).Returns(tempImageWriterMock.Object);
            Factories.SetTemporaryImageFileWriterFactory(tempImageWriterFactoryMock.Object);            
            savedImageMock.Setup(o => o.PathToImage).Returns(new Uri(pathToImage));
            savedImageMock.Setup(o => o.IsBayered).Returns(true);
            activeProfileMock.Setup(o => o.CameraSettings).Returns(cameraSettingsMock.Object);
            cameraSettingsMock.Setup(o => o.BitDepth).Returns(42);
            cameraSettingsMock.Setup(o => o.RawConverter).Returns(NINA.Core.Enum.RawConverterEnum.FREEIMAGE);
            imageDataFactoryMock.Setup(o => o.CreateFromFile(new Uri(pathToImage).AbsolutePath, 42, true, NINA.Core.Enum.RawConverterEnum.FREEIMAGE, It.IsAny<CancellationToken>())).ReturnsAsync(imageDataMock.Object);
            var imagedSaveArgs = ImageTestUtility.GenerateImageSavedEventArgs(savedImageMock.Object);
            imagingMediatorMock.Setup(o => o.PrepareImage(imageDataMock.Object, It.IsAny<PrepareImageParameters>(), It.IsAny<CancellationToken>())).ReturnsAsync(renderedImageMock.Object);
            renderedImageMock.Setup(o => o.Image).Returns(bitmap);
            Helpers.SetDiscordHelper(discordHelperMock.Object);

            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator.Object, imagingMediatorMock.Object, imageDataFactoryMock.Object, profileServiceMock.Object) {
                Text = expectedText
            };
            discordMessageAfterImage.SequenceBlockInitialize();
            discordMessageAfterImage.ShouldTrigger(Mock.Of<ISequenceItem>(), Mock.Of<IExposureItem>());
            await discordMessageAfterImage.Execute(expectedContainer, Mock.Of<IProgress<ApplicationStatus>>(), cancelTokenSource.Token);
            imageSaveMediator.Raise(o => o.ImageSaved += null, imagedSaveArgs);

            discordHelperMock.Verify(o => o.SendMessage(MessageType.Information, expectedText, expectedContainer, cancelTokenSource.Token, imagePatterns, expectedTempPath, null), Times.Once);
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

        [Test]
        public async Task ImageMonitorImageSaved_DiscordHelperException_Handled() {
            var cancelTokenSource = new CancellationTokenSource();
            var pathToImage = "C:/test/path.png";
            var expectedText = "This is a test";
            var expectedTempPath = "temp_path";
            var pixelFormat = new PixelFormat();
            var expectedContainer = Mock.Of<ISequenceContainer>();
            pixelFormat = PixelFormats.Pbgra32;
            var bitmap = new WriteableBitmap(100, 100, 10, 10, pixelFormat, new BitmapPalette(new List<Color>() { Colors.Red }));
            var imagePatterns = new ImagePatterns();
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
            var fileHelperMock = new Mock<IFileHelper>();
            fileHelperMock.Setup(o => o.Exists(pathToImage)).Returns(true);
            Helpers.SetFileHelper(fileHelperMock.Object);
            var templateHelperMock = new Mock<ITemplateHelper>();
            templateHelperMock.Setup(o => o.GetImageTemplateValues(It.IsAny<IRenderedImage>())).Returns(imagePatterns);
            Helpers.SetTemplateHelper(templateHelperMock.Object);
            discordClientFactory.Setup(o => o.Create()).Returns(discordClient.Object);
            Factories.SetDiscordClientFactory(discordClientFactory.Object);
            var tempImageWriterMock = new Mock<ITemporaryImageFileWriter>();
            tempImageWriterMock.SetupGet(o => o.Filename).Returns(expectedTempPath);
            var tempImageWriterFactoryMock = new Mock<ITemporaryImageFileWriterFactory>();
            tempImageWriterFactoryMock.Setup(o => o.Create(activeProfileMock.Object, bitmap)).Returns(tempImageWriterMock.Object);
            Factories.SetTemporaryImageFileWriterFactory(tempImageWriterFactoryMock.Object);
            savedImageMock.Setup(o => o.PathToImage).Returns(new Uri(pathToImage));
            savedImageMock.Setup(o => o.IsBayered).Returns(true);
            profileServiceMock.Setup(o => o.ActiveProfile).Returns(activeProfileMock.Object);
            activeProfileMock.Setup(o => o.CameraSettings).Returns(cameraSettingsMock.Object);
            cameraSettingsMock.Setup(o => o.BitDepth).Returns(42);
            cameraSettingsMock.Setup(o => o.RawConverter).Returns(NINA.Core.Enum.RawConverterEnum.FREEIMAGE);
            imageDataFactoryMock.Setup(o => o.CreateFromFile(new Uri(pathToImage).AbsolutePath, 42, true, NINA.Core.Enum.RawConverterEnum.FREEIMAGE, It.IsAny<CancellationToken>())).ReturnsAsync(imageDataMock.Object);
            var imagedSaveArgs = ImageTestUtility.GenerateImageSavedEventArgs(savedImageMock.Object);
            imagingMediatorMock.Setup(o => o.PrepareImage(imageDataMock.Object, It.IsAny<PrepareImageParameters>(), It.IsAny<CancellationToken>())).ReturnsAsync(renderedImageMock.Object);
            renderedImageMock.Setup(o => o.Image).Returns(bitmap);
            discordHelperMock.Setup(o => o.SendMessage(It.IsAny<MessageType>(), It.IsAny<string>(), It.IsAny<ISequenceEntity>(), It.IsAny<CancellationToken>(), It.IsAny<ImagePatterns>(), It.IsAny<string>(), It.IsAny<Exception>()))
                .Throws(new Exception());
            Helpers.SetDiscordHelper(discordHelperMock.Object);

            var discordMessageAfterImage = new DiscordMessageAfterImageTrigger(imageSaveMediator.Object, imagingMediatorMock.Object, imageDataFactoryMock.Object, profileServiceMock.Object) {
                Text = expectedText
            };
            discordMessageAfterImage.SequenceBlockInitialize();
            discordMessageAfterImage.ShouldTrigger(Mock.Of<ISequenceItem>(), Mock.Of<IExposureItem>());
            await discordMessageAfterImage.Execute(expectedContainer, Mock.Of<IProgress<ApplicationStatus>>(), cancelTokenSource.Token);
            Assert.DoesNotThrow(() => imageSaveMediator.Raise(o => o.ImageSaved += null, imagedSaveArgs));

            discordHelperMock.Verify(o => o.SendMessage(MessageType.Information, expectedText, expectedContainer, cancelTokenSource.Token, imagePatterns, expectedTempPath, null), Times.Once);

        }
    }
}
