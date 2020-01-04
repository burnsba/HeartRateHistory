using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HeartRateHistory.HotConfig;

namespace HeartRateHistory.Converters
{
    /// <summary>
    /// Describes how to convert a heartrate to a color.
    /// </summary>
    public class HeartRateRgbConverter
    {
        private static int _colorIntervals = 0;
        private static int _cutoffRange = 0;
        private static int[] _intervalIndexMap;
        private static List<SolidColorBrush> _intervalValues = null;
        private static bool _isSetup = false;
        private static SolidColorBrush _max = new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
        private static int _maxCutoff = int.MinValue;
        private static SolidColorBrush _min = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 0, byte.MaxValue, 0));
        private static int _minCutoff = int.MaxValue;

        private HeartRateRgbConverter()
        {
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static HeartRateRgbConverter Instance { get; } = new HeartRateRgbConverter();

        /// <summary>
        /// Gets or sets the number of possible colors.
        /// </summary>
        private static int ColorIntervals
        {
            get
            {
                if (!_isSetup)
                {
                    Setup();
                }

                return _colorIntervals;
            }

            set
            {
                _colorIntervals = value;
            }
        }

        /// <summary>
        /// Gets or sets the range (scaled from zero) of available heartrates.
        /// </summary>
        private static int CutoffRange
        {
            get
            {
                if (!_isSetup)
                {
                    Setup();
                }

                return _cutoffRange;
            }

            set
            {
                _cutoffRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the least upper bound of the last color interval heart rate.
        /// </summary>
        private static int MaxCutoff
        {
            get
            {
                if (!_isSetup)
                {
                    Setup();
                }

                return _maxCutoff;
            }

            set
            {
                _maxCutoff = value;
            }
        }

        /// <summary>
        /// Gets or sets the greatest lower bound of the first color interval heart rate.
        /// </summary>
        private static int MinCutoff
        {
            get
            {
                if (!_isSetup)
                {
                    Setup();
                }

                return _minCutoff;
            }

            set
            {
                _minCutoff = value;
            }
        }

        /// <summary>
        /// Gets the appropriate color to render a particular heartrate value.
        /// </summary>
        /// <param name="val">Heartrate value.</param>
        /// <returns>Color brush to render heartrate.</returns>
        public static System.Windows.Media.Brush FromInt(int val)
        {
            if (object.ReferenceEquals(null, _intervalValues))
            {
                Setup();
            }

            if (val < MinCutoff)
            {
                return _min;
            }
            else if (val > MaxCutoff)
            {
                return _max;
            }

            var scaledValue = (int)((((decimal)val - (decimal)MinCutoff) / (decimal)CutoffRange) * (decimal)byte.MaxValue);

            if (scaledValue < 3)
            {
                return _min;
            }
            else if (scaledValue > byte.MaxValue - 3)
            {
                return _max;
            }
            else
            {
                var colorIndex = _intervalIndexMap[scaledValue];

                return _intervalValues[colorIndex];
            }
        }

        /// <summary>
        /// Loads settings and sets intervals.
        /// </summary>
        public static void Setup()
        {
            var settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            // Don't reference the properties, they will trigger this method if not set.
            _colorIntervals = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.ColorIntervalsKey).CurrentValue);
            _minCutoff = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.HeartRateGreenFloorKey).CurrentValue);
            _maxCutoff = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.HeartRateRedCeilingKey).CurrentValue);

            _cutoffRange = _maxCutoff - _minCutoff;

            if (!_isSetup)
            {
                MessageBus.MessageBus.Subscribe<ViewModels.ConfigViewModel, HeartRateRgbConverter>(nameof(ViewModels.ConfigViewModel.SettingsChangedNotification), Instance, SettingsChangeHandler);
            }

            _isSetup = true;

            // can't BuildIntervalValues until _isSetup == true
            BuildIntervalValues();
        }

        /// <summary>
        /// Creates the actual intervals used to determine colors for heartrates.
        /// </summary>
        private static void BuildIntervalValues()
        {
            _intervalValues = new List<SolidColorBrush>();
            _intervalIndexMap = new int[byte.MaxValue];

            var stepSize = byte.MaxValue / ColorIntervals;
            var stepCount = byte.MaxValue / (ColorIntervals * 2);

            // Build green to yellow
            for (int step = stepSize; step < byte.MaxValue; step += stepSize)
            {
                // fade out the intensity a bit by back off from max value.
                var b = new SolidColorBrush(Color.FromArgb(byte.MaxValue - 16, (byte)step, byte.MaxValue - 16, 0));
                _intervalValues.Add(b);
            }

            // Build yellow to red
            for (int step = byte.MaxValue - stepSize; step > 0; step -= stepSize)
            {
                // fade out the intensity a bit by back off from max value.
                var b = new SolidColorBrush(Color.FromArgb(byte.MaxValue - 32, byte.MaxValue - 32, (byte)step, 0));
                _intervalValues.Add(b);
            }

            // scaled values to indeces
            for (int i = 0, currentIndexMap = 0; i < byte.MaxValue; i++)
            {
                _intervalIndexMap[i] = currentIndexMap;
                if (i > 0 && i % stepCount == 0 && currentIndexMap + 1 < _intervalValues.Count)
                {
                    currentIndexMap++;
                }
            }
        }

        private static void SettingsChangeHandler(object sender, EventArgs args)
        {
            Setup();
        }
    }
}
