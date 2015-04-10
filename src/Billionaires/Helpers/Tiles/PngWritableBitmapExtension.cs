/*
 * ToolStack.com C# WriteableBitmap extension for PNG Writer library by Greg Ross
 * 
 * Homepage: http://ToolStack.com/PNGWriter
 * 
 * This library is based upon the examples hosted at the forums on WriteableBitmapEx
 * project at the codeplex site (http://writeablebitmapex.codeplex.com/discussions/274445).
 * 
 * This is public domain software, use and abuse as you see fit.
 * 
 * Version 1.0 - Released Feburary 22, 2012
*/

using System.Windows.Media.Imaging;

namespace Billionaires.Helpers.Tiles
{
    /// <summary>
    /// WriteableBitmap Extensions for PNG Writing
    /// </summary>
    public static class WriteableBitmapExtensions
    {
        /// <summary>
        /// Write and PNG file out to a file stream.  Currently compression is not supported.
        /// </summary>
        /// <param name="image">The WriteableBitmap to work on.</param>
        /// <param name="stream">The destination file stream.</param>
        public static void WritePng(this WriteableBitmap image, System.IO.Stream stream)
        {
            PngWriter.DetectWbByteOrder();
            PngWriter.WritePng(image, stream);
        }
    }
}
