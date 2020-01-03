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

        //private List<SlideChartDataPoint> _visibleDataSeries = new List<SlideChartDataPoint>();
        private List<SlideChartDataPoint> _archiveDataSeries = new List<SlideChartDataPoint>();

        private int _dataValueDisplayWidth = 8;

        private int _maxItemsDisplayed;
        private int _maxItemsInMemory;

        public SlideChartViewModel(SettingsCollection settingsSource)
        {
            LoadConfig(settingsSource);

            VisibleDataSeries = new ObservableCollection<SlideChartDataPoint>();

            //////AppendData(new SlideChartDataPoint(DateTime.Now.AddSeconds(-4), 77));
            //////AppendData(new SlideChartDataPoint(DateTime.Now, 78));
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

        public int MaxItemsDisplayed
        {
            get
            {
                return _maxItemsDisplayed;
            }

            set
            {
                _maxItemsDisplayed = value;
                OnPropertyChanged(nameof(MaxItemsDisplayed));
            }
        }

        public int MaxItemsInMemory
        {
            get
            {
                return _maxItemsInMemory;
            }

            set
            {
                _maxItemsInMemory = value;
                OnPropertyChanged(nameof(MaxItemsInMemory));
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
                while (_archiveDataSeries.Count + 1 > _maxItemsInMemory)
                {
                    _archiveDataSeries.RemoveAt(0);
                }

                _archiveDataSeries.Add(data);

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    while (VisibleDataSeries.Count + 1 > _maxItemsDisplayed)
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
                _archiveDataSeries.Clear();
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                VisibleDataSeries.Clear();
            });
        }

        public void NotifyReloadConfig()
        {
            ReadConfig();

            if (MaxItemsInMemory > _archiveDataSeries.Count)
            {
                TruncateArchive();
            }

            if (MaxItemsDisplayed > VisibleDataSeries.Count)
            {
                VisibleReloadFromData();
            }
        }

        public void VisibleReloadFromData()
        {
            lock (_dataLock)
            {
                VisibleDataSeries.Clear();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    int startIndex = Math.Max(0, _archiveDataSeries.Count - _maxItemsDisplayed);
                    int endIndex = _archiveDataSeries.Count;

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        VisibleDataSeries.Add(_archiveDataSeries[i]);
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
                    var firstDate = _archiveDataSeries.Min(x => x.CaptureTime);
                    var lastDate = _archiveDataSeries.Max(x => x.CaptureTime);

                    var firstline = $"#{firstDate.ToString(DateTimeFormat)} - {lastDate.ToString(DateTimeFormat)}, {_archiveDataSeries.Count()} values.";

                    sw.WriteLine(firstline);
                    sw.WriteLine(string.Join(",", _archiveDataSeries.OrderBy(x => x.CaptureTime).Select(x => x.Value)));
                }
            }
        }

        private void TruncateArchive()
        {
            lock (_dataLock)
            {
                while (_archiveDataSeries.Count > _maxItemsInMemory)
                {
                    _archiveDataSeries.RemoveAt(0);
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
            MaxItemsDisplayed = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.MaxItemsDisplayedKey).CurrentValue);
            MaxItemsInMemory = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.MaxItemsInMemoryKey).CurrentValue);
        }

        private void ReadConfig()
        {
            var settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            LoadConfig(settingSource);
        }
    }
}