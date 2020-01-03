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
    public class EmbeddedResourceGif : IDisposable
    {
        public string EmbeddedResourcePath { get; set; }

        public List<EmbeddedResourceGifFrame> Frames { get; set; }

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

        public Storyboard MakeWpfImageStoryboard(System.Windows.Controls.Image wpfImage, TimeSpan totalDuration)
        {
            var sb = new Storyboard();
            var animation = ToAnimationFlat(totalDuration);

            Storyboard.SetTarget(animation, wpfImage);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.SourceProperty));

            sb.Children.Add(animation);

            return sb;
        }

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
    }
}
