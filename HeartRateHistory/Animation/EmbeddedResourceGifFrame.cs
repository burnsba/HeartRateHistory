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
    /// <summary>
    /// Single frame to be used in an animation.
    /// </summary>
    public class EmbeddedResourceGifFrame : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceGifFrame"/> class.
        /// </summary>
        /// <param name="currentFrame">Which frame this is out of the total.</param>
        /// <param name="imageSource">Image data source.</param>
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

        /// <summary>
        /// Gets or sets the current frame.
        /// </summary>
        public int CurrentFrame { get; set; }

        /// <summary>
        /// Gets or sets the frame height.
        /// </summary>
        public int FrameHeight { get; set; }

        /// <summary>
        /// Gets or sets the frame width.
        /// </summary>
        public int FrameWidth { get; set; }

        /// <summary>
        /// Gets or sets the image data.
        /// </summary>
        public BitmapImage Image { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Image = null;
        }
    }
}
