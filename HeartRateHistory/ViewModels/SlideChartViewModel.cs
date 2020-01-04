using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using BurnsBac.HotConfig;
using BurnsBac.WindowsAppToolkit.Mvvm;
using BurnsBac.WindowsAppToolkit.Services.MessageBus;
using BurnsBac.WindowsAppToolkit.ViewModels;
using HeartRateHistory.Models;

namespace HeartRateHistory.ViewModels
{
    /// <summary>
    /// View model for slide chart control.
    /// </summary>
    public class SlideChartViewModel : ViewModelBase
    {
        /// <summary>
        /// Default height for chart area.
        /// </summary>
        public const int InitialSeriesPanelHeight = 220;

        private SolidColorBrush _backgroundColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

        private string _backgroundColorString = null;

        private SolidColorBrush _backgroundDataLineColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

        private string _backgroundDataLineColorString = null;

        private SolidColorBrush _backgroundDataLineLabelColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

        private string _backgroundDataLineLabelColorString = null;

        private double _containerDisplayHeight = InitialSeriesPanelHeight;

        private object _dataLock = new object();

        private List<SlideChartDataPoint> _archiveDataSeries = new List<SlideChartDataPoint>();

        private int _dataValueDisplayWidth = 8;

        private int _maxItemsDisplayed;
        private int _maxItemsInMemory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideChartViewModel"/> class.
        /// </summary>
        /// <param name="settingsSource">Settings file data.</param>
        public SlideChartViewModel(SettingsCollection settingsSource)
        {
            LoadConfig(settingsSource);

            VisibleDataSeries = new ObservableCollection<SlideChartDataPoint>();

            MessageBus.Subscribe<ConfigViewModel, SlideChartViewModel>(nameof(ConfigViewModel.SettingsChangedNotification), this, SettingsChangeHandler);

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

        /// <summary>
        /// Gets or sets actual height of chart area.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the width of each individual rectangle in the chart area.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum number of rectangles to be shown in the chart area.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the max number of data points to keep in memory.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the collection used to generate rectangles rendered on the chart.
        /// </summary>
        public ObservableCollection<SlideChartDataPoint> VisibleDataSeries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format string of the date used in when saving data to file.
        /// </summary>
        private string DateTimeFormat { get; set; } = "yyyyMMdd-HHmmss";

        /// <summary>
        /// Adds a data point to the collection in memory. This is also
        /// added to the collection used to render the rectangles in the chart area.
        /// </summary>
        /// <param name="data">Data point to add.</param>
        /// <returns>True always.</returns>
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

        /// <summary>
        /// Removes all data points from memory and from the chart rendering collection.
        /// </summary>
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

        /// <summary>
        /// Reads the config file form disk again and updates properties accordingly.
        /// </summary>
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

        /// <summary>
        /// Called when the window is resized, the existing data source items in the chart area
        /// are removed and added again from memory.
        /// </summary>
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

        /// <summary>
        /// Writes the collection of data points in memory to file.
        /// </summary>
        /// <param name="filename">Filename to write to.</param>
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

        /// <summary>
        /// If the settings reload shortens the length of items in memory, drop
        /// the oldest items until under bound.
        /// </summary>
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

        /// <summary>
        /// Loads settings from object and sets properties.
        /// </summary>
        /// <param name="settingSource">Settings collection to read from.</param>
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

        /// <summary>
        /// Loads settings from disk.
        /// </summary>
        private void ReadConfig()
        {
            var settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            LoadConfig(settingSource);
        }

        private void SettingsChangeHandler(object sender, EventArgs args)
        {
            ReadConfig();
        }
    }
}