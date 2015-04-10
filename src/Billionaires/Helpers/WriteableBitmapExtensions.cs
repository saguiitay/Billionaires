using System;
using System.Windows.Media.Imaging;

namespace Billionaires.Helpers
{
    public static class WriteableBitmapExtensions
    {
        public static WriteableBitmap CropWriteableBitmap(this WriteableBitmap image, int imageLeft)
        {
            var resultImage = new WriteableBitmap(95, 100);
            for (var x = 0; x <= 95 - 1; x++)
            {
                var sourceIndex = imageLeft + x * image.PixelWidth;
                var destinationIndex = x * 95;

                Array.Copy(image.Pixels, sourceIndex, resultImage.Pixels, destinationIndex, 95);
            }
            return resultImage;
        }
    }
}