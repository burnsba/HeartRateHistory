using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BurnsBac.Mvvm;
using BurnsBac.WindowsHardware;
using HeartRateHistory.ViewModels;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace HeartRateHistory.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml .
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly TimeSpan DataXferGifNaturalDuration = new TimeSpan(0, 0, 0, 4, 0);
        private static readonly TimeSpan HeartGifNaturalDuration = new TimeSpan(0, 0, 1);

        private Storyboard _datxferStoryboard;
        private Storyboard _heartStoryboard;
        private MainViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel();

            _vm.PlayOnceImageDataXfer = PlayOnceImageDataXfer;
            _vm.StopAnimations = StopAnimations;
            _vm.ChangeHeartRateImageBpm = ChangeHeartRateImageBpm;

            DataContext = _vm;

            var heartAnimation = new Animation.EmbeddedResourceGif("HeartRateHistory.img.heart.gif");
            _heartStoryboard = heartAnimation.MakeWpfImageStoryboard(ImageHeart, HeartGifNaturalDuration);
            _heartStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            _heartStoryboard.Duration = HeartGifNaturalDuration;
            _heartStoryboard.Name = "HeartStoryboard";
            _heartStoryboard.Begin(ImageHeart, true);
            _heartStoryboard.Pause(ImageHeart);

            var dataxferAnimation = new Animation.EmbeddedResourceGif("HeartRateHistory.img.dataxfer_reverse3.gif");
            _datxferStoryboard = dataxferAnimation.MakeWpfImageStoryboard(ImageDataXfer, DataXferGifNaturalDuration);
            _datxferStoryboard.RepeatBehavior = new RepeatBehavior(1);
            _datxferStoryboard.Duration = DataXferGifNaturalDuration;
            _datxferStoryboard.Name = "DatxferStoryboard";
            _datxferStoryboard.Begin(ImageDataXfer, true);
            _datxferStoryboard.Pause(ImageDataXfer);

            heartAnimation.Dispose();
            dataxferAnimation.Dispose();
        }

        private void ChangeHeartRateImageBpm(int bpm)
        {
            if (bpm < 1)
            {
                return;
            }

            double scaleFactor = HeartGifNaturalDuration.TotalSeconds * (double)bpm / 60.0;

            Dispatcher.Invoke(() =>
            {
                var currentTime = TimeSpan.Zero;

                // For some reason the unqualified call always throws an InvalidOperationException.
                // Adding the argument results in the warning text
                //     System.Windows.Media.Animation Warning: 6 : Unable to perform action because the specified Storyboard was never applied to this object for interactive control.
                // But seems to work.
                currentTime = _heartStoryboard.GetCurrentTime(ImageHeart) ?? TimeSpan.Zero;

                _heartStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                _heartStoryboard.Stop();
                _heartStoryboard.SpeedRatio = scaleFactor;
                _heartStoryboard.Begin(ImageHeart, true);
                _heartStoryboard.Seek(currentTime);
            });
        }

        private void PlayOnceImageDataXfer()
        {
            Dispatcher.Invoke(() =>
            {
                _datxferStoryboard.Pause(ImageDataXfer);
                _datxferStoryboard.Seek(ImageDataXfer, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
                _datxferStoryboard.Resume(ImageDataXfer);
            });
        }

        private void StopAnimations()
        {
            Dispatcher.Invoke(() =>
            {
                _heartStoryboard.Pause(ImageHeart);
                _heartStoryboard.Seek(ImageHeart, TimeSpan.Zero, TimeSeekOrigin.BeginTime);

                _datxferStoryboard.Pause(ImageDataXfer);
                _datxferStoryboard.Seek(ImageDataXfer, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
            });
        }
    }
}
