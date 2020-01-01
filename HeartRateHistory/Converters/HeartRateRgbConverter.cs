using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HeartRateHistory.Converters
{
    public static class HeartRateRgbConverter
    {
        private static SolidColorBrush Min = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 0, byte.MaxValue, 0));
        private static SolidColorBrush Max = new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
        private static List<SolidColorBrush> IntervalValues = null;
        private static int[] IntervalIndexMap;

        private static int ColorIntervals = 0; // set in Setup()
        private static int MinCutoff = int.MaxValue; // set in Setup()
        private static int MaxCutoff = int.MinValue; // set in Setup()
        private static int CutoffRange = 0; // set in Setup()

        public static System.Windows.Media.Brush FromInt(int val)
        {
            if (object.ReferenceEquals(null, IntervalValues))
            {
                Setup();
                BuildIntervalValues();
            }

            if (val < MinCutoff)
            {
                return Min;
            }
            else if (val > MaxCutoff)
            {
                return Max;
            }

            var scaledValue = (int)((((decimal)val - (decimal)MinCutoff) / (decimal)CutoffRange) * (decimal)byte.MaxValue);

            if (scaledValue < 3)
            {
                return Min;
            }
            else if (scaledValue > byte.MaxValue - 3)
            {
                return Max;
            }
            else
            {
                var colorIndex = IntervalIndexMap[scaledValue];

                return IntervalValues[colorIndex];
            }
        }

        private static void Setup()
        {
            ColorIntervals = Config.NumberColorIntervals;

            MinCutoff = Config.HeartRateGreenFloor;
            MaxCutoff = Config.HeartRateRedCeiling;
            CutoffRange = MaxCutoff - MinCutoff;
        }

        private static void BuildIntervalValues()
        {
            IntervalValues = new List<SolidColorBrush>();
            IntervalIndexMap = new int[byte.MaxValue];

            var stepSize = byte.MaxValue / ColorIntervals;
            var stepCount = byte.MaxValue / (ColorIntervals * 2);

            // Build green to yellow
            for (int step = stepSize; step < byte.MaxValue; step += stepSize)
            {
                var b = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte)step, byte.MaxValue, 0));
                IntervalValues.Add(b);
            }

            // Build yellow to red
            for (int step = byte.MaxValue - stepSize; step > 0; step -= stepSize)
            {
                var b = new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte)step, 0));
                IntervalValues.Add(b);
            }

            // scaled values to indeces
            for (int i=0, currentIndexMap=0; i<byte.MaxValue; i++)
            {
                IntervalIndexMap[i] = currentIndexMap;
                if (i > 0 && i % stepCount == 0 && currentIndexMap + 1 < IntervalValues.Count)
                {
                    currentIndexMap++;
                }
            }
        }
    }
}
