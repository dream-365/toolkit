using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace OnlineStroage.Services
{
    public class ImageResizeService
    {
        public Stream Resize(Stream source, int? width, int? height)
        {
            var streamDestination = new MemoryStream();

            var bitmapSource = new Bitmap(source);

            double scaleWidth = width.HasValue ? (double)width.Value / bitmapSource.Width : 1;
            double scaleHeight = height.HasValue? (double)height.Value / bitmapSource.Height : 1;

            double scale = Math.Min(scaleWidth, scaleHeight);

            if (scale > 1)
            {
                scale = 1;
            }

            var size = new Size
            {
                Width = (int)(bitmapSource.Width * scale),
                Height = (int)(bitmapSource.Height * scale)
            };

            var bitmapDestination = new Bitmap(size.Width, size.Height);

            using (Graphics graphics = Graphics.FromImage(bitmapDestination))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(bitmapSource, 0, 0, size.Width, size.Height);

            }

            bitmapDestination.Save(streamDestination, bitmapSource.RawFormat);

            streamDestination.Position = 0;

            return streamDestination;
        }
    }
}