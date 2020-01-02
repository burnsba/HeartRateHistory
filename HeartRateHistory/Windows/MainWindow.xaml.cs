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
using WpfAnimatedGif;

namespace HeartRateHistory.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        private ImageAnimationController _imageDataXferController = null;
        private ImageAnimationController _imageHeartController = null;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel();

            _vm.PlayOnceImageDataXfer = PlayOnceImageDataXfer;
            _vm.StopAnimations = StopAnimations;
            _vm.ChangeHeartRateImageBpm = ChangeHeartRateImageBpm;

            DataContext = _vm;
        }

        private ImageAnimationController ImageDataXferController
        {
            get
            {
                if (object.ReferenceEquals(null, _imageDataXferController))
                {
                    _imageDataXferController = ImageBehavior.GetAnimationController(ImageDataXfer);
                }

                return _imageDataXferController;
            }
        }

        private ImageAnimationController ImageHeartController
        {
            get
            {
                if (object.ReferenceEquals(null, _imageHeartController))
                {
                    _imageHeartController = ImageBehavior.GetAnimationController(ImageHeart);
                }

                return _imageHeartController;
            }
        }

        private void PlayOnceImageDataXfer()
        {
            Dispatcher.Invoke(() =>
            {
                ImageDataXferController.Pause();
                ImageDataXferController.GotoFrame(0);
                ImageBehavior.SetRepeatBehavior(ImageDataXfer, new System.Windows.Media.Animation.RepeatBehavior(0));
                ImageDataXferController.Play();
            });
        }

        private void StopAnimations()
        {
            Dispatcher.Invoke(() =>
            {
                ImageDataXferController.Pause();
                ImageDataXferController.GotoFrame(0);

                ImageHeartController.Pause();
                ImageHeartController.GotoFrame(0);
            });
        }

        private void ChangeHeartRateImageBpm(int bpm)
        {
            if (bpm < 1)
            {
                return;
            }

            int durationms = (int)(60000.0 / (double)bpm);

            Dispatcher.Invoke(() =>
            {
                ImageHeartController.ChangeDurationFlat(new TimeSpan(0, 0, 0, 0, durationms));
            });
        }
    }
}
