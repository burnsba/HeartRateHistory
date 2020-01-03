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
    /// Class to manage gif file from embedded resource.
    /// </summary>
    public class EmbeddedResourceGif : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceGif"/> class.
        /// </summary>
        /// <param name="embeddedResourcePath">Path to embedded resource.</param>
        /// <example>
        /// <code>
        /// var heartAnimation = new Animation.EmbeddedResourceGif("HeartRateHistory.img.heart.gif");
        /// </code>
        /// </example>
        public EmbeddedResourceGif(string embeddedResourcePath)
        {
            Frames = new List<EmbeddedResourceGifFrame>();
            EmbeddedResourcePath = embeddedResourcePath;

            using (var img = System.Drawing.Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourcePath)))
            {
                var dimension = new FrameDimension(img.FrameDimensionsList[0]);
                var frameCount = img.GetFrameCount(dimension);

                for (int i = 0; i < frameCount; i++)
                {
                    img.SelectActiveFrame(dimension, i);
                    Frames.Add(new EmbeddedResourceGifFrame(i, img));
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to the embedded resource.
        /// </summary>
        public string EmbeddedResourcePath { get; set; }

        /// <summary>
        /// Gets or sets the collection of frames within the gif.
        /// </summary>
        public List<EmbeddedResourceGifFrame> Frames { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!object.ReferenceEquals(null, Frames))
            {
                foreach (var frame in Frames)
                {
                    frame.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates a storyboard for the WPF image from this gif. Frame duration within the gif is ignored,
        /// each frame is given an equal duration based on the <paramref name="totalDuration"/>.
        /// </summary>
        /// <param name="wpfImage">Image to create storyboard for.</param>
        /// <param name="totalDuration">Total duration of animation.</param>
        /// <returns>Storyboard for animation.</returns>
        public Storyboard MakeWpfImageStoryboard(System.Windows.Controls.Image wpfImage, TimeSpan totalDuration)
        {
            var sb = new Storyboard();
            var animation = ToAnimationFlat(totalDuration);

            Storyboard.SetTarget(animation, wpfImage);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.SourceProperty));

            sb.Children.Add(animation);

            return sb;
        }

        /// <summary>
        /// Helper function, creates the necessary keyframes for the storyboard.
        /// </summary>
        /// <param name="totalDuration">Total duration of animation. Each keyframe is given a duration of <paramref name="totalDuration"/> / <see cref="EmbeddedResourceGif.Frames"/>.Count.</param>
        /// <returns>Keyframe animation.</returns>
        private ObjectAnimationUsingKeyFrames ToAnimationFlat(TimeSpan totalDuration)
        {
            var oaukf = new ObjectAnimationUsingKeyFrames();

            TimeSpan current = TimeSpan.Zero;
            TimeSpan frameDuration = totalDuration / Frames.Count;

            foreach (var frame in Frames)
            {
                var kf = new DiscreteObjectKeyFrame(frame.Image, current);
                oaukf.KeyFrames.Add(kf);
                current += frameDuration;
            }

            oaukf.Duration = current;

            return oaukf;
        }
    }
}
