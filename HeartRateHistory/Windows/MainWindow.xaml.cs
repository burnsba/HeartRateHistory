using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private static readonly TimeSpan AdjustHeartGifBpmInterval = new TimeSpan(0, 0, 5);

        private Storyboard _datxferStoryboard;
        private Storyboard _heartStoryboard;
        private MainViewModel _vm;

        private Stopwatch _timeSincelastUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel();

            _vm.StopNotification += StopUpdatesHandler;
            _vm.DataReceived += HeartRateReceived;

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

            _timeSincelastUpdate = new Stopwatch();
        }

        private void HeartRateReceived(object sender, BurnsBac.WindowsHardware.Bluetooth.Characteristics.HeartRateMeasurement e)
        {
            var currentTime = TimeSpan.Zero;
            double scaleFactor = HeartGifNaturalDuration.TotalSeconds * (double)e.HeartRate / 60.0;

            Dispatcher.Invoke(() =>
            {
                // trigger data transfer notification
                _datxferStoryboard.Pause(ImageDataXfer);
                _datxferStoryboard.Seek(ImageDataXfer, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
                _datxferStoryboard.Resume(ImageDataXfer);

                if (_timeSincelastUpdate.Elapsed == TimeSpan.Zero || _timeSincelastUpdate.Elapsed > AdjustHeartGifBpmInterval)
                {
                    _timeSincelastUpdate.Stop();

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

                    _timeSincelastUpdate.Restart();
                }
            });
        }

        private void StopUpdatesHandler(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _heartStoryboard.Pause(ImageHeart);
                _heartStoryboard.Seek(ImageHeart, TimeSpan.Zero, TimeSeekOrigin.BeginTime);

                _datxferStoryboard.Pause(ImageDataXfer);
                _datxferStoryboard.Seek(ImageDataXfer, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!object.ReferenceEquals(null, _vm))
            {
                _vm.StopNotification -= StopUpdatesHandler;
                _vm.DataReceived -= HeartRateReceived;
            }
        }
    }
}
