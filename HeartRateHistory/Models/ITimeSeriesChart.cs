using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.Models
{
    public interface ITimeSeriesChart
    {
        int MaxDataItems { get; set; }

        bool AppendData(SlideChartDataPoint data);

        void RemoveOlderThan(int ageInSeconds);

        void WriteValuesToFile(string filename);

        void Clear();
    }
}
