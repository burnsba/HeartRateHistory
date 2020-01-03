using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using BurnsBac.Mvvm;
using HeartRateHistory.HotConfig;
using HeartRateHistory.Models;

namespace HeartRateHistory.ViewModels
{
    public class SlideChartViewModel : ViewModelBase
    {
        public const int InitialSeriesPanelHeight = 220;

        private SolidColorBrush _backgroundColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

        private string _backgroundColorString = null;

        private SolidColorBrush _backgroundDataLineColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

        private string _backgroundDataLineColorString = null;

        private SolidColorBrush _backgroundDataLineLabelColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

        private string _backgroundDataLineLabelColorString = null;

        private double _containerDisplayHeight = InitialSeriesPanelHeight;

        private object _dataLock = new object();

        private List<SlideChartDataPoint> _dataSeries = new List<SlideChartDataPoint>();

        private int _dataValueDisplayWidth = 8;

        private int _maxDataItems = 100;

        public SlideChartViewModel(SettingsCollection settingsSource)
        {
            LoadConfig(settingsSource);

            VisibleDataSeries = new ObservableCollection<SlideChartDataPoint>();

#if DEBUG
            //////AppendData(new SlideChartDataPoint(DateTime.Now.AddSeconds(-4), 77));
            //////AppendData(new SlideChartDataPoint(DateTime.Now, 78));
#endif
        }

        /// <summary>
        /// Gets or sets the main window background color.
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
                _backgroundColorString = $"#{value.Color.R.ToString("X2")}{value.Color.G.ToString("X2")}{value.Color.B.ToString("X2")}";
                OnPropertyChanged(nameof(BackgroundColor));
                OnPropertyChanged(nameof(BackgroundColorString));
            }
        }

        /// <summary>
        /// Gets or sets the main window background color, as a hex code string.
        /// </summary>
        public string BackgroundColorString
        {
            get
            {
                return _backgroundColorString;
            }

            set
            {
                var str = value;

                if (!string.IsNullOrEmpty(str) && str[0] != '#')
                {
                    str = "#" + str;
                }

                _backgroundColorString = str?.ToUpper();
                _backgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                OnPropertyChanged(nameof(BackgroundColorString));
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        /// <summary>
        /// Gets or sets the chart area interval line color.
        /// </summary>
        public SolidColorBrush BackgroundDataLineColor
        {
            get
            {
                return _backgroundDataLineColor;
            }

            set
            {
                _backgroundDataLineColor = value;
                _backgroundDataLineColorString = $"#{value.Color.R.ToString("X2")}{value.Color.G.ToString("X2")}{value.Color.B.ToString("X2")}";
                OnPropertyChanged(nameof(BackgroundDataLineColor));
                OnPropertyChanged(nameof(BackgroundDataLineColorString));
            }
        }

        /// <summary>
        /// Gets or sets the chart area interval line color, as a hex code string.
        /// </summary>
        public string BackgroundDataLineColorString
        {
            get
            {
                return _backgroundDataLineColorString;
            }

            set
            {
                var str = value;

                if (!string.IsNullOrEmpty(str) && str[0] != '#')
                {
                    str = "#" + str;
                }

                _backgroundDataLineColorString = str?.ToUpper();
                _backgroundDataLineColor = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                OnPropertyChanged(nameof(BackgroundDataLineColorString));
                OnPropertyChanged(nameof(BackgroundDataLineColor));
            }
        }

        /// <summary>
        /// Gets or sets the chart area interval line color.
        /// </summary>
        public SolidColorBrush BackgroundDataLineLabelColor
        {
            get
            {
                return _backgroundDataLineLabelColor;
            }

            set
            {
                _backgroundDataLineLabelColor = value;
                _backgroundDataLineLabelColorString = $"#{value.Color.R.ToString("X2")}{value.Color.G.ToString("X2")}{value.Color.B.ToString("X2")}";
                OnPropertyChanged(nameof(BackgroundDataLineLabelColor));
                OnPropertyChanged(nameof(BackgroundDataLineLabelColorString));
            }
        }

        /// <summary>
        /// Gets or sets the chart area data interval label color, as a hex code string.
        /// </summary>
        public string BackgroundDataLineLabelColorString
        {
            get
            {
                return _backgroundDataLineLabelColorString;
            }

            set
            {
                var str = value;

                if (!string.IsNullOrEmpty(str) && str[0] != '#')
                {
                    str = "#" + str;
                }

                _backgroundDataLineLabelColorString = str?.ToUpper();
                _backgroundDataLineLabelColor = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                OnPropertyChanged(nameof(BackgroundDataLineLabelColorString));
                OnPropertyChanged(nameof(BackgroundDataLineLabelColor));
            }
        }

        public double ContainerDisplayHeight
        {
            get
            {
                return _containerDisplayHeight;
            }

            set
            {
                _containerDisplayHeight = value;
                OnPropertyChanged(nameof(ContainerDisplayHeight));
            }
        }

        public int DataValueDisplayWidth
        {
            get
            {
                return _dataValueDisplayWidth;
            }

            set
            {
                _dataValueDisplayWidth = value;
                OnPropertyChanged(nameof(DataValueDisplayWidth));
            }
        }

        public int MaxDataItems
        {
            get
            {
                return _maxDataItems;
            }

            set
            {
                _maxDataItems = value;
                OnPropertyChanged(nameof(MaxDataItems));
            }
        }

        public ObservableCollection<SlideChartDataPoint> VisibleDataSeries
        {
            get;
            set;
        }

        private string DateTimeFormat { get; set; } = "yyyyMMdd-HHmmss";

        public bool AppendData(SlideChartDataPoint data)
        {
            lock (_dataLock)
            {
                while (_dataSeries.Count + 1 > _maxDataItems)
                {
                    _dataSeries.RemoveAt(0);
                }

                _dataSeries.Add(data);

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    while (VisibleDataSeries.Count + 1 > _maxDataItems)
                    {
                        VisibleDataSeries.RemoveAt(0);
                    }

                    VisibleDataSeries.Add(data);
                });
            }

            return true;
        }

        public void Clear()
        {
            lock (_dataLock)
            {
                _dataSeries.Clear();
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                VisibleDataSeries.Clear();
            });
        }

        public void NotifyReloadConfig()
        {
            ReadConfig();
        }

        public void RemoveOlderThan(int ageInSeconds)
        {
            var now = DateTime.Now;

            lock (_dataLock)
            {
                _dataSeries.RemoveAll(x => now.Subtract(x.CaptureTime).TotalSeconds > ageInSeconds);

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = VisibleDataSeries.Count; i >= 0; i--)
                    {
                        if (now.Subtract(VisibleDataSeries[i].CaptureTime).TotalSeconds > ageInSeconds)
                        {
                            VisibleDataSeries.RemoveAt(i);
                        }
                    }
                });
            }
        }

        public void VisibleReloadFromData()
        {
            lock (_dataLock)
            {
                VisibleDataSeries.Clear();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var x in _dataSeries)
                    {
                        VisibleDataSeries.Add(x);
                    }
                });
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

        private void LoadConfig(SettingsCollection settingSource)
        {
            DateTimeFormat = settingSource.Items.First(x => x.Key == SharedConfig.DateTimeFormatKey).CurrentValue;
            DataValueDisplayWidth = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.DataValueDisplayWidthKey).CurrentValue);
            BackgroundColorString = settingSource.Items.First(x => x.Key == SharedConfig.SlideChartBackgroundColorKey).CurrentValue;
            BackgroundDataLineColorString = settingSource.Items.First(x => x.Key == SharedConfig.SlideChartDataLineColorKey).CurrentValue;
            BackgroundDataLineLabelColorString = settingSource.Items.First(x => x.Key == SharedConfig.SlideChartDataLineLabelColorKey).CurrentValue;
        }

        private void ReadConfig()
        {
            var settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            LoadConfig(settingSource);
        }
    }
}