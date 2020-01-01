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
        Image _gifImage;

        public MainWindow()
        {
            InitializeComponent();

            var aaa = Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();

            var dataxferSource = new BitmapImage(new Uri($"pack://application:,,,/HeartRateHistory;component/img/dataxfer.gif"));
            ImageBehavior.SetAnimatedSource(ImageDataXfer, dataxferSource);

            var heartSource = new BitmapImage(new Uri($"pack://application:,,,/HeartRateHistory;component/img/heart.gif"));
            ImageBehavior.SetAnimatedSource(ImageHeart, heartSource);

            _vm = new MainViewModel();

            _vm.AppendDataMethod = HeartRateSlideChart.AppendData;

            DataContext = _vm;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        var controller = ImageBehavior.GetAnimationController(_gifImage);
        //        controller.Pause();
        //        controller.GotoFrame(0);
        //    });
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        ImageBehavior.SetRepeatBehavior(_gifImage, System.Windows.Media.Animation.RepeatBehavior.Forever);
        //        var controller = ImageBehavior.GetAnimationController(_gifImage);
        //        controller.Play();
        //    });
        //}

        //// fast
        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //    var controller = ImageBehavior.GetAnimationController(_gifImage);
        //    controller.ChangeDurationScale(controller.Duration / 2);
        //}

        //// slow
        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    var controller = ImageBehavior.GetAnimationController(_gifImage);
        //    controller.ChangeDurationScale(controller.Duration * 2);
        //}
    }
}
