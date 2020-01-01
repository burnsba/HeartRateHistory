using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartRateHistory.Models
{
    public class SlideChart
    {
        private object _dataLock = new object();

        public ObservableCollection<SlideChartDataPoint> DataSeries { get; set; } = new ObservableCollection<SlideChartDataPoint>();

        public SlideChartDataPoint MostRecent { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public int LastUpdateAgeSec
        {
            get
            {
                return (int)Math.Abs(DateTime.Now.Subtract(LastUpdateTime).TotalSeconds);
            }
        }

        public int MaxDataItems { get; set; } = 1;

        public void AppendDate(SlideChartDataPoint data)
        {
            lock (_dataLock)
            {
                while (DataSeries.Count + 1 > MaxDataItems)
                {
                    DataSeries.RemoveAt(0);
                }

                DataSeries.Add(data);
            }

            MostRecent = data;
            LastUpdateTime = DateTime.Now;
        }

        public void RemoveOlderThan(int ageInSeconds)
        {
            var now = DateTime.Now;

            lock (_dataLock)
            {
                for (int i = DataSeries.Count; i>0; i--)
                {
                    if (now.Subtract(DataSeries[i].CaptureTime).TotalSeconds > ageInSeconds)
                    {
                        DataSeries.RemoveAt(i);
                    }
                }
            }
        }

        public void WriteValuesToFile(string filename)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            {
                lock (_dataLock)
                {
                    var firstDate = DataSeries.Min(x => x.CaptureTime);
                    var lastDate = DataSeries.Max(x => x.CaptureTime);

                    var firstline = $"#{firstDate.ToString(Config.DateTimeFormat)} - {lastDate.ToString(Config.DateTimeFormat)}, {DataSeries.Count()} values.";

                    sw.WriteLine(firstline);
                    sw.WriteLine(
                        string.Join(",", DataSeries.OrderBy(x => x.CaptureTime).Select(x => x.Value))
                        );
                }
            }
        }

        public void Clear()
        {
            lock (_dataLock)
            {
                DataSeries.Clear();
            }

            MostRecent = null;
            LastUpdateTime = DateTime.MinValue;
        }

        public void ClearMostRecent()
        {
            MostRecent = null;
            LastUpdateTime = DateTime.MinValue;
        }
    }
}
