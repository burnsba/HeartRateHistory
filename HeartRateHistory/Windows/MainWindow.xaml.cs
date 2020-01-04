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
            _heartStoryboard.Begin();
            _heartStoryboard.Pause();

            var dataxferAnimation = new Animation.EmbeddedResourceGif("HeartRateHistory.img.dataxfer_reverse3.gif");
            _datxferStoryboard = dataxferAnimation.MakeWpfImageStoryboard(ImageDataXfer, DataXferGifNaturalDuration);
            _datxferStoryboard.RepeatBehavior = new RepeatBehavior(1);
            _datxferStoryboard.Duration = DataXferGifNaturalDuration;
            _datxferStoryboard.Name = "DatxferStoryboard";
            _datxferStoryboard.Begin();
            _datxferStoryboard.Pause();

            /* Storyboard was being called with the framework element as an argument, as well as true to mark "controllable".
             * i.e.
             *
             *     _heartStoryboard.Begin(ImageHeart, true);
             *
             * I think the problem is that it's assigning a clock to the ImageHeart, but not to the storyboard.
             * When GetCurrentTime is called without arguments, it looks up the clock associated with the storyboard,
             * which is null in this case, because it was never set. The end result is that calling
             *
             *     _heartStoryboard.GetCurrentTime();
             *
             * Results in an exception:
             *
             *     "Cannot perform action because the specified Storyboard was
             *      not applied to this object for interactive control"
             *
             * It seems like there's three options.
             * (1) Call GetCurrentTime with the ImageHeart. Somehow this only sort of works, and still results
             * in warning messages in the console. Animation jerks back to the first frame for some reason
             * when trying to continue at the correct time.
             * (2) Leave all the storyboard calls with explicit parameters, but somehow start the storyboard
             * clock. I'm not sure how to do this.
             * (3) Get rid of all parameters to storyboard calls, just call Begin(). The comment in source
             *https://github.com/dotnet/wpf/blob/ae1790531c3b993b56eba8b1f0dd395a3ed7de75/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Media/Animation/Storyboard.cs#L1196
             * says
             *
             *     "Begins all animations underneath this storyboard, clock tree starts in "shared clocks" mode."
             *
             * This also defaults isControllable to true.
             * This works for me, no more warnings, no more exceptions.
             * */

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
                _datxferStoryboard.Pause();
                _datxferStoryboard.Seek(TimeSpan.Zero, TimeSeekOrigin.BeginTime);
                _datxferStoryboard.Resume();

                if (_timeSincelastUpdate.Elapsed == TimeSpan.Zero || _timeSincelastUpdate.Elapsed > AdjustHeartGifBpmInterval)
                {
                    _timeSincelastUpdate.Stop();

                    currentTime = _heartStoryboard.GetCurrentTime();

                    _heartStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                    _heartStoryboard.Stop();
                    _heartStoryboard.SpeedRatio = scaleFactor;
                    _heartStoryboard.Begin(); // do not call with arguments, see notes in main constructor
                    _heartStoryboard.Seek(currentTime);

                    _timeSincelastUpdate.Restart();
                }
            });
        }

        private void StopUpdatesHandler(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _heartStoryboard.Pause();
                _heartStoryboard.Seek(TimeSpan.Zero, TimeSeekOrigin.BeginTime);

                _datxferStoryboard.Pause();
                _datxferStoryboard.Seek(TimeSpan.Zero, TimeSeekOrigin.BeginTime);
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
