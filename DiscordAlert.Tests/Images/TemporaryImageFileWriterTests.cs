using Moq;
using NINA.DiscordAlert.Images;
using NINA.DiscordAlert.Util;
using NUnit.Framework;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;
using System;
using System.Threading.Tasks;
using NINA.Profile.Interfaces;

namespace DiscordAlert.Tests.Images {
    [TestFixture]
    public class TemporaryImageFileWriterTests : BaseTextFixture {
        [Test]
        public void TemporaryImageFileWriter_FileDoesNotExistYet_CreatesImageThenDeletesImage() 
        {
            var basePath = "base_path";
            var fullPath = $"{basePath}//image.png";
            var imageFileSettingsMock = new Mock<IImageFileSettings>();
            imageFileSettingsMock.SetupGet(o => o.FilePath).Returns(basePath);
            var profileMock = new Mock<IProfile>();
            profileMock.Setup(o => o.ImageFileSettings).Returns(imageFileSettingsMock.Object);
            var fileHelperMock = new Mock<IFileHelper>();
            Helpers.SetFileHelper(fileHelperMock.Object);
            fileHelperMock.Setup(o => o.Combine(It.Is<string[]>(o => o[0].Equals(basePath)))).Returns(fullPath);
            fileHelperMock.Setup(o => o.Exists(fullPath)).Returns(false);
            var pixelFormat = new PixelFormat();
            pixelFormat = PixelFormats.Pbgra32;
            var bitmap = new WriteableBitmap(100, 100, 10, 10, pixelFormat, new BitmapPalette(new List<Color>() { Colors.Red }));

            using (var tempFileWriter = new TemporaryImageFileWriter(profileMock.Object, bitmap)) {
                fileHelperMock.Verify(o => o.Exists(fullPath), Times.Once);
                fileHelperMock.Verify(o => o.WriteImageToFile(fullPath, It.IsAny<BitmapEncoder>()), Times.Once);
                fileHelperMock.Setup(o => o.Exists(fullPath)).Returns(true);
            }
   
            fileHelperMock.Verify(o => o.Delete(fullPath), Times.Once);
        }

        [Test]
        public void TemporaryImageFileWriter_FileAlreadyExistsAfterManyAttempts_ThrowsAndDoesNotHang() {
            var imageFileSettingsMock = new Mock<IImageFileSettings>();
            imageFileSettingsMock.SetupGet(o => o.FilePath).Returns("C:/");
            var profileMock = new Mock<IProfile>();
            profileMock.Setup(o => o.ImageFileSettings).Returns(imageFileSettingsMock.Object);
            var timeout = TimeSpan.FromSeconds(5);
            var fileHelperMock = new Mock<IFileHelper>();
            Helpers.SetFileHelper(fileHelperMock.Object);
            fileHelperMock.Setup(o => o.Exists(It.IsAny<string>())).Returns(true);
            fileHelperMock.Setup(o => o.Combine(It.IsAny<string[]>())).Returns("C:\\path\\image.png");
            var cancelTokenSource = new CancellationTokenSource();

            var task = Task.Run(() => {
                var pixelFormat = new PixelFormat();
                pixelFormat = PixelFormats.Pbgra32;
                var bitmap = new WriteableBitmap(100, 100, 10, 10, pixelFormat, new BitmapPalette(new List<Color>() { Colors.Red }));
                Assert.Throws<Exception>(() => new TemporaryImageFileWriter(profileMock.Object, bitmap));
            }, cancelTokenSource.Token);

            Assert.IsTrue(task.Wait(timeout));
            cancelTokenSource.Cancel();
        }
    }
}
