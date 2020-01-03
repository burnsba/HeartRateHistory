using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace HeartRateHistory.Animation
{
    public class EmbeddedResourceGifFrame : IDisposable
    {
        public EmbeddedResourceGifFrame(int currentFrame, System.Drawing.Image imageSource)
        {
            if (currentFrame < 0)
            {
                throw new ArgumentException(nameof(currentFrame));
            }

            if (object.ReferenceEquals(null, imageSource))
            {
                throw new ArgumentNullException(nameof(imageSource));
            }

            CurrentFrame = currentFrame;
            FrameWidth = imageSource.Width;
            FrameHeight = imageSource.Height;

            var bm = new BitmapImage();

            using (var ms = new System.IO.MemoryStream())
            {
                imageSource.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                bm.BeginInit();
                bm.CacheOption = BitmapCacheOption.OnLoad;
                bm.UriSource = null;
                bm.StreamSource = ms;
                bm.EndInit();
            }

            Image = bm;
        }

        public int CurrentFrame { get; set; }

        public int FrameHeight { get; set; }

        public int FrameWidth { get; set; }

        public BitmapImage Image { get; set; }

        public void Dispose()
        {
            Image = null;
        }
    }
}
