//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using System.Text;
//////using System.Threading.Tasks;
//////using BurnsBac.Mvvm;
//////using HeartRateHistory.Models;

//////namespace HeartRateHistory.ViewModels
//////{
//////    public class SlideChartViewModel : ViewModelBase
//////    {
//////        private SlideChart _slideChart;

//////        public string DataNotAvailable { get; set; } = "---";

//////        public string Display
//////        {
//////            get
//////            {
//////                if (!object.ReferenceEquals(null, _slideChart))
//////                {
//////                    if (!object.ReferenceEquals(null, _slideChart.MostRecent))
//////                    {
//////                        return _slideChart.MostRecent.Value.ToString();
//////                    }
//////                }

//////                return DataNotAvailable;
//////            }
//////        }

//////        public bool CollectActive { get; set; } = true;

//////        public int LastUpdateAgeSec
//////        {
//////            get
//////            {
//////                return _slideChart.LastUpdateAgeSec;
//////            }
//////        }

//////        public SlideChartViewModel()
//////        {
//////            _slideChart = new SlideChart()
//////            {
//////                MaxDataItems = Config.MaxNumberDataInMemory
//////            };
//////        }

//////        public void AppendData(DateTime captureTime, int value)
//////        {
//////            if (!CollectActive)
//////            {
//////                return;
//////            }

//////            var dp = new SlideChartDataPoint(captureTime, value);
//////            _slideChart.AppendDate(dp);
//////            OnPropertyChanged(nameof(Display));
//////            OnPropertyChanged(nameof(LastUpdateAgeSec));
//////            OnPropertyChanged(new SlideChartDataPointChangedEventArgs(nameof(SlideChart.DataSeries), dp));
//////        }

//////        public void AppendDataNow(int value)
//////        {
//////            if (!CollectActive)
//////            {
//////                return;
//////            }

//////            var dp = new SlideChartDataPoint(DateTime.Now, value);
//////            _slideChart.AppendDate(dp);
//////            OnPropertyChanged(nameof(Display));
//////            OnPropertyChanged(nameof(LastUpdateAgeSec));
//////            OnPropertyChanged(new SlideChartDataPointChangedEventArgs(nameof(SlideChart.DataSeries), dp));
//////        }

//////        public void SaveData()
//////        {
//////            var filename = DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".csv";
//////            _slideChart.WriteValuesToFile(filename);
//////        }

//////        public void Reset()
//////        {
//////            _slideChart.Clear();
//////        }

//////        public void ClearMostRecent()
//////        {
//////            _slideChart.ClearMostRecent();
//////        }
//////    }
//////}
