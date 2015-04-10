using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Billionaires.Model
{
    internal class PortraitsData
    {
        public int index { get; set; }
        public string faces { get; set; }
        public string halos { get; set; }
        public string masks { get; set; }

        public WriteableBitmap GetImage()
        {
            var facesImage = faces.Substring(22);
            var imageDataRaw = Convert.FromBase64String(facesImage);
            var image = new WriteableBitmap(2850, 100);
            using (var stream = new MemoryStream(imageDataRaw))
                image.SetSource(stream);

            return image;

        }

        public WriteableBitmap GetTileImage(WriteableBitmap image)
        {

            // TODO: Use the Masks to create tiles with the correct background

            //var baseImg = new WriteableBitmap(2850, 100);
            //var currentAccentColorHex = (Color)Application.Current.Resources["PhoneAccentColor"];
            //baseImg.FillRectangle(0, 0, 2850, 100, currentAccentColorHex);


            // This doesn't work, since masks is a GIF
            //var masksImage = masks.Substring(22);
            //var imageDataRaw = Convert.FromBase64String(masksImage);
            //var mask = new WriteableBitmap(2850, 100);
            //using (var stream = new MemoryStream(imageDataRaw))
            //    mask.SetSource(stream);
            //baseImg.Blit(new Rect(0, 0, 2850, 100), mask, new Rect(0, 0, 2850, 100));


            //baseImg.Blit(new Rect(0, 0, 2850, 100), image, new Rect(0, 0, 2850, 100),
            //           WriteableBitmapExtensions.BlendMode.Additive);

            return image;

        }
    }
}