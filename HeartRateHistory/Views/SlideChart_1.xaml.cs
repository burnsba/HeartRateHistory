using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeartRateHistory.Converters;
using HeartRateHistory.Models;
using HeartRateHistory.ViewModels;

namespace HeartRateHistory.Views
{
    /// <summary>
    /// Interaction logic for SlideChart.xaml .
    /// </summary>
    public partial class SlideChart_1 : UserControl
    {
        //private const int InitialSeriesPanelHeight = 220;

        //private AutoResetEvent _rescaleTimeout = new AutoResetEvent(false);
        //private Timer _rescaleTimeoutTimer;

        ////public SlideChartViewModel ViewModel { get; set; } = new SlideChartViewModel();



        //public Action ReconnectCallback { get; set; }

        //public SlideChartView()
        //{
        //    InitializeComponent();

        //    ViewModel.PropertyChanged += ViewModelPropertyChangedHandler;

        //    this.DataContext = ViewModel;

        //    var bind = new Binding();
        //    bind.Source = ViewModel;
        //    bind.Path = new PropertyPath(nameof(ViewModel.Display));
        //    bind.Mode = BindingMode.OneWay;
        //    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    BindingOperations.SetBinding(DisplayLabelRight, TextBlock.TextProperty, bind);

        //    bind = new Binding();
        //    bind.Source = ViewModel;
        //    bind.Path = new PropertyPath(nameof(ViewModel.LastUpdateAgeSec));
        //    bind.Mode = BindingMode.OneWay;
        //    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    BindingOperations.SetBinding(LastUpdateAgeSeconds, TextBlock.TextProperty, bind);

        //    DisplayLabelRight.FontFamily = new FontFamily(Config.DisplayLabelRightFontFamily);
        //    DisplayLabelRight.FontSize = Config.DisplayLabelRightFontSize;
        //}

        //private void ViewModelPropertyChangedHandler(object obj, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(Models.SlideChart.DataSeries))
        //    {
        //        var args = (SlideChartDataPointChangedEventArgs)e;
        //        if (!object.ReferenceEquals(null, e))
        //        {
        //            var latest = args.Value;

        //            Application.Current.Dispatcher.Invoke(() =>
        //            {

        //                var areaHeight = SeriesPanelContainer.ActualHeight;
        //                double rectHeight = double.NaN;

        //                if (areaHeight > InitialSeriesPanelHeight)
        //                {
        //                    var scaleFactor = areaHeight / (double)InitialSeriesPanelHeight;
        //                    rectHeight = scaleFactor * (double)latest.Value;
        //                }
        //                else
        //                {
        //                    rectHeight = latest.Value;
        //                }

        //                var r = new Rectangle();
        //                r.Fill = HeartRateRgbConverter.FromInt(latest.Value);
        //                r.Stroke = Brushes.Black;
        //                r.Width = Config.DataValueDisplayWidth;
        //                r.Height = rectHeight;
        //                r.VerticalAlignment = VerticalAlignment.Bottom;
        //                r.Tag = latest;

        //                SeriesPanel.Children.Add(r);

        //                while (SeriesPanel.Children.Count > Config.MaxNumberDataShown)
        //                {
        //                    SeriesPanel.Children.RemoveAt(0);
        //                }
        //            });
        //        }
        //    }
        //}

        //public void RemoveOlderThan(int ageInSeconds)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        var now = DateTime.Now;
        //        var toRemove = new List<UIElement>();

        //        foreach (UIElement child in SeriesPanel.Children)
        //        {
        //            var rect = child as Rectangle;
        //            if (!object.ReferenceEquals(null, rect))
        //            {
        //                var data = rect.Tag as SlideChartDataPoint;
        //                if (!object.ReferenceEquals(null, data))
        //                {
        //                    if (now.Subtract(data.CaptureTime).TotalSeconds > Config.MaxDataAgeSec)
        //                    {
        //                        toRemove.Add(child);
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var element in toRemove)
        //        {
        //            SeriesPanel.Children.Remove(element);
        //        }
        //    });
        //}

        //private void Save_Click(object sender, RoutedEventArgs e)
        //{
        //    ViewModel.SaveData();
        //}

        //private void PauseResume_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ViewModel.CollectActive)
        //    {
        //        ViewModel.CollectActive = false;
        //        PauseResume.Content = "Resume";
        //    }
        //    else
        //    {
        //        ViewModel.CollectActive = true;
        //        PauseResume.Content = "Pause";
        //    }
        //}

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        //    if (object.ReferenceEquals(null, _rescaleTimeoutTimer))
        //    {
        //        _rescaleTimeoutTimer = new System.Threading.Timer(DispatchRescaleRectanglesTimer, _rescaleTimeout, 1000, Timeout.Infinite);
        //    }
        //    else
        //    {
        //        _rescaleTimeoutTimer.Change(1000, Timeout.Infinite);
        //    }
        }

        //private void DispatchRescaleRectanglesTimer(object stateInfo)
        //{
        //    AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
        //    _rescaleTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);

        //    DispatchRescaleRectangles();
        //}

        //private void DispatchRescaleRectangles()
        //{
        //    Application.Current.Dispatcher.Invoke(() => RescaleRectangles());
        //}

        //private void RescaleRectangles()
        //{
        //    var areaHeight = SeriesPanelContainer.ActualHeight;
        //    var scaleFactor = areaHeight / (double)InitialSeriesPanelHeight;

        //    if (areaHeight > InitialSeriesPanelHeight)
        //    {
        //        AssignBottomMargin(RectLine50, scaleFactor * 50);
        //        AssignBottomMargin(RectLine75, scaleFactor * 75);
        //        AssignBottomMargin(RectLine100, scaleFactor * 100);
        //        AssignBottomMargin(RectLine125, scaleFactor * 125);
        //        AssignBottomMargin(RectLine150, scaleFactor * 150);
        //    }
        //    else
        //    {
        //        AssignBottomMargin(RectLine50, 50);
        //        AssignBottomMargin(RectLine75, 75);
        //        AssignBottomMargin(RectLine100, 100);
        //        AssignBottomMargin(RectLine125, 125);
        //        AssignBottomMargin(RectLine150, 150);
        //    }

        //    foreach (UIElement child in SeriesPanel.Children)
        //    {
        //        var rect = child as Rectangle;
        //        if (!object.ReferenceEquals(null, rect))
        //        {
        //            var data = rect.Tag as SlideChartDataPoint;
        //            if (!object.ReferenceEquals(null, data))
        //            {
        //                double rectHeight = double.NaN;

        //                if (areaHeight > InitialSeriesPanelHeight)
        //                {

        //                    rectHeight = scaleFactor * (double)data.Value;
        //                }
        //                else
        //                {
        //                    rectHeight = data.Value;
        //                }

        //                rect.Height = rectHeight;
        //            }
        //        }
        //    }
        //}

        //private void AssignBottomMargin(Rectangle rect, double size)
        //{
        //    Thickness margin = rect.Margin;
        //    margin.Bottom = size;
        //    rect.Margin = margin;
        //}

        //private void Reset_Click(object sender, RoutedEventArgs e)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        while (SeriesPanel.Children.Count > 0)
        //        {
        //            SeriesPanel.Children.RemoveAt(0);
        //        }
        //    });

        //    ViewModel.Reset();
        //}

        //private void Reconnect_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!object.ReferenceEquals(null, ReconnectCallback))
        //    {
        //        ReconnectCallback();
        //    }
        //}
    }
}
