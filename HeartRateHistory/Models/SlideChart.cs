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

        //public List<SlideChartDataPoint> DataSeries { get; set; } = new List<SlideChartDataPoint>();
        private List<SlideChartDataPoint> _dataSeries = new List<SlideChartDataPoint>();

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

        public void AppendData(SlideChartDataPoint data)
        {
            lock (_dataLock)
            {
                while (_dataSeries.Count + 1 > MaxDataItems)
                {
                    _dataSeries.RemoveAt(0);
                }

                _dataSeries.Add(data);
            }

            MostRecent = data;
            LastUpdateTime = DateTime.Now;
        }

        public void RemoveOlderThan(int ageInSeconds)
        {
            var now = DateTime.Now;

            lock (_dataLock)
            {
                _dataSeries.RemoveAll(x => now.Subtract(x.CaptureTime).TotalSeconds > ageInSeconds);

                //for (int i = DataSeries.Count; i > 0; i--)
                //{
                //    if (now.Subtract(DataSeries[i].CaptureTime).TotalSeconds > ageInSeconds)
                //    {
                //        DataSeries.RemoveAt(i);
                //    }
                //}
            }
        }

        public void WriteValuesToFile(string filename)
        {
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            //{
            //    lock (_dataLock)
            //    {
            //        var firstDate = _dataSeries.Min(x => x.CaptureTime);
            //        var lastDate = _dataSeries.Max(x => x.CaptureTime);

            //        var firstline = $"#{firstDate.ToString(Config.DateTimeFormat)} - {lastDate.ToString(Config.DateTimeFormat)}, {_dataSeries.Count()} values.";

            //        sw.WriteLine(firstline);
            //        sw.WriteLine(
            //            string.Join(",", _dataSeries.OrderBy(x => x.CaptureTime).Select(x => x.Value))
            //            );
            //    }
            //}
        }

        public void Clear()
        {
            lock (_dataLock)
            {
                _dataSeries.Clear();
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
