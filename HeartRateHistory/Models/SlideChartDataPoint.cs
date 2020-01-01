using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartRateHistory.Models
{
    public class SlideChartDataPoint
    {
        public DateTime CaptureTime { get; private set; }

        public int Value { get; private set; }

        public SlideChartDataPoint(DateTime captureTime, int value)
        {
            CaptureTime = captureTime;
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{CaptureTime.ToString("yyyyMMdd-hhmmss")}: {Value}";
        }
    }
}
