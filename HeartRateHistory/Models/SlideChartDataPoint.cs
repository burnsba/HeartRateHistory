using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartRateHistory.Models
{
    /// <summary>
    /// Single data item to be drawn on a chart.
    /// </summary>
    public class SlideChartDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlideChartDataPoint"/> class.
        /// </summary>
        /// <param name="captureTime">Time of data acquisition.</param>
        /// <param name="value">Data value.</param>
        public SlideChartDataPoint(DateTime captureTime, int value)
        {
            CaptureTime = captureTime;
            Value = value;
        }

        /// <summary>
        /// Gets time of data acquisition.
        /// </summary>
        public DateTime CaptureTime { get; private set; }

        /// <summary>
        /// Gets data value.
        /// </summary>
        public int Value { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{CaptureTime.ToString("yyyyMMdd-hhmmss")}: {Value}";
        }
    }
}
