//////using System;
//////using System.Collections.Generic;
//////using System.ComponentModel;
//////using System.Linq;
//////using System.Text;
//////using System.Threading.Tasks;

//////namespace HeartRateHistory.Models
//////{
//////    public class SlideChartDataPointChangedEventArgs : PropertyChangedEventArgs
//////    {
//////        public SlideChartDataPoint Value { get; set; }

//////        public SlideChartDataPointChangedEventArgs(string propertyName)
//////            :base(propertyName)
//////        {

//////        }

//////        public SlideChartDataPointChangedEventArgs(string propertyName, SlideChartDataPoint value)
//////            :base(propertyName)
//////        {
//////            Value = value;
//////        }
//////    }
//////}
