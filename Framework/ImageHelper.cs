using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Holism.Framework
{
    public class ImageHelper
    {
        public static MemoryStream MakeImageThumbnail(int? maxWidth, int? maxHeight, byte[] bytes)
        {
            var image = System.Drawing.Image.FromStream(new MemoryStream(bytes));
            maxWidth = maxWidth ?? image.Width;
            maxHeight = maxHeight ?? image.Height;
            if (image.Width < maxWidth)
            {
                maxWidth = image.Width;
            }
            decimal imageRatio = (decimal)image.Width / (decimal)image.Height;
            decimal outputWidth = maxWidth.Value;
            int outputHeight = Convert.ToInt32(outputWidth / imageRatio);
            var thumbnail = image.GetThumbnailImage((int)outputWidth, outputHeight, () => true, IntPtr.Zero);
            var memoryStream = new MemoryStream();
            thumbnail.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}