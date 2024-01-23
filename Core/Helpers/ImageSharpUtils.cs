using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace FEZRepacker.Core.Helpers
{
    internal static class ImageSharpUtils
    {
        public static MemoryStream SaveAsMemoryStream(this Image image, IImageEncoder encoder)
        {
            var outStream = new MemoryStream();
            image.Save(outStream, encoder);
            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }
    }
}
