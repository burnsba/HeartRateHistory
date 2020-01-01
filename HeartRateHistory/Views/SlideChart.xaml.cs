using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using HeartRateHistory.HotConfig;
using HeartRateHistory.Models;

namespace HeartRateHistory.Views
{
    /// <summary>
    /// Interaction logic for SlideChart.xaml .
    /// </summary>
    public partial class SlideChart : UserControl, ITimeSeriesChart
    {
        private const int InitialSeriesPanelHeight = 220;
        private List<SlideChartDataPoint> _dataSeries = new List<SlideChartDataPoint>();
        private object _dataLock = new object();
        private int _maxDataItems = 100;

        //public delegate bool AppendDataDelegate(SlideChartDataPoint data);

        public SlideChart()
        {
            InitializeComponent();

            //SetValue(AppendDataMethodProperty, (AppendDataDelegate)AppendData);
        }

        //public static readonly DependencyProperty AppendDataMethodProperty =
        //DependencyProperty.Register(nameof(AppendDataMethod), typeof(AppendDataDelegate), typeof(SlideChart),
        //    new PropertyMetadata(null, AppendDataMethod_PropertyChanged));

        public static readonly DependencyProperty MaxDataItemsProperty =
        DependencyProperty.Register(nameof(MaxDataItems), typeof(int), typeof(SlideChart),
            new PropertyMetadata(100, MaxDataItems_PropertyChanged));

        //public AppendDataDelegate AppendDataMethod
        //{
        //    get { return AppendData; }
        //    set { return; }
        //}

        public int MaxDataItems
        {
            get { return (int)GetValue(MaxDataItemsProperty); }

            set
            {
                SetValue(MaxDataItemsProperty, value);
                _maxDataItems = value;
            }
        }

        private string DateTimeFormat { get; set; } = "yyyyMMdd-HHmmss";
        private int DataValueDisplayWidth { get; set; } = 20;

        //protected static void AppendDataMethod_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //  I just put this here for testing: If it's non-null, it'll be called. 
        //    //  I set a breakpoint in the MainWindow method to detect the call. 
        //    int a = 9;
        //}

        protected static void MaxDataItems_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  I just put this here for testing: If it's non-null, it'll be called. 
            //  I set a breakpoint in the MainWindow method to detect the call. 
            int a = 9;
        }

        public bool AppendData(SlideChartDataPoint data)
        {
            lock (_dataLock)
            {
                while (_dataSeries.Count + 1 > _maxDataItems)
                {
                    _dataSeries.RemoveAt(0);
                }

                _dataSeries.Add(data);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                var areaHeight = SeriesPanelContainer.ActualHeight;
                double rectHeight = double.NaN;

                if (areaHeight > InitialSeriesPanelHeight)
                {
                    var scaleFactor = areaHeight / (double)InitialSeriesPanelHeight;
                    rectHeight = scaleFactor * (double)data.Value;
                }
                else
                {
                    rectHeight = data.Value;
                }

                var r = new Rectangle();
                r.Fill = HeartRateRgbConverter.FromInt(data.Value);
                r.Stroke = Brushes.Black;
                r.Width = DataValueDisplayWidth;
                r.Height = rectHeight;
                r.VerticalAlignment = VerticalAlignment.Bottom;
                r.Tag = data;

                SeriesPanel.Children.Add(r);

                while (SeriesPanel.Children.Count > _maxDataItems)
                {
                    SeriesPanel.Children.RemoveAt(0);
                }
            });

            return true;
        }

        public void RemoveOlderThan(int ageInSeconds)
        {
            var now = DateTime.Now;

            lock (_dataLock)
            {
                _dataSeries.RemoveAll(x => now.Subtract(x.CaptureTime).TotalSeconds > ageInSeconds);
            }
        }

        public void WriteValuesToFile(string filename)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            {
                lock (_dataLock)
                {
                    var firstDate = _dataSeries.Min(x => x.CaptureTime);
                    var lastDate = _dataSeries.Max(x => x.CaptureTime);

                    var firstline = $"#{firstDate.ToString(DateTimeFormat)} - {lastDate.ToString(DateTimeFormat)}, {_dataSeries.Count()} values.";

                    sw.WriteLine(firstline);
                    sw.WriteLine(
                        string.Join(",", _dataSeries.OrderBy(x => x.CaptureTime).Select(x => x.Value))
                        );
                }
            }
        }

        public void Clear()
        {
            lock (_dataLock)
            {
                _dataSeries.Clear();
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                while (SeriesPanel.Children.Count > 0)
                {
                    SeriesPanel.Children.RemoveAt(0);
                }
            });
        }

        private void ReadConfig()
        {
            var settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            DateTimeFormat = settingSource.Items.First(x => x.Key == SharedConfig.DateTimeFormatKey).CurrentValue;
            DataValueDisplayWidth = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.DataValueDisplayWidthKey).CurrentValue);
        }
    }
}
