using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NINA.DiscordAlert.Images {
    [ExcludeFromCodeCoverage]
    public class TemporaryImageFileWriterFactory : ITemporaryImageFileWriterFactory {
        public ITemporaryImageFileWriter Create(BitmapSource image) {
            return new TemporaryImageFileWriter(image);
        }
    }
}
