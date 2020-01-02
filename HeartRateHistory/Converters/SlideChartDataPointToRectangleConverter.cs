using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using HeartRateHistory.Models;
using HeartRateHistory.ViewModels;

namespace HeartRateHistory.Converters
{
    /// <summary>
    /// Converts bool to visibility status.
    /// </summary>
    [ValueConversion(typeof(SlideChartDataPoint), typeof(System.Windows.Shapes.Rectangle))]
    public class SlideChartDataPointToRectangleConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts SlideChartDataPoint to Rectangle.
        /// </summary>
        /// <param name="value">Value to convert (bool).</param>
        /// <param name="targetType">Type to convert to (Visibility).</param>
        /// <param name="parameter">Convert parameter.</param>
        /// <param name="culture">Convert culture.</param>
        /// <returns>
        /// Visibility status.
        /// </returns>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(System.Windows.Shapes.Rectangle))
            {
                throw new InvalidOperationException("The target must be a System.Windows.Shapes.Rectangle");
            }

            var scdp = (SlideChartDataPoint)value;
            var vm = (SlideChartViewModel)parameter;

            var areaHeight = vm.ContainerDisplayHeight;
            double rectHeight = double.NaN;

            if (areaHeight > SlideChartViewModel.InitialSeriesPanelHeight)
            {
                var scaleFactor = areaHeight / (double)SlideChartViewModel.InitialSeriesPanelHeight;
                rectHeight = scaleFactor * (double)scdp.Value;
            }
            else
            {
                rectHeight = scdp.Value;
            }

            var r = new Rectangle();
            r.Fill = HeartRateRgbConverter.FromInt(scdp.Value);
            r.Stroke = Brushes.Black;
            r.Width = vm.DataValueDisplayWidth;
            r.Height = rectHeight;
            r.VerticalAlignment = VerticalAlignment.Bottom;
            r.Tag = scdp;

            return r;
        }

        /// <summary>
        /// NotSupported.
        /// </summary>
        /// <param name="value">Value to convert (System.Windows.Shapes.Rectangle).</param>
        /// <param name="targetType">Type to convert to (SlideChartDataPoint).</param>
        /// <param name="parameter">Convert parameter.</param>
        /// <param name="culture">Convert culture.</param>
        /// <returns>
        /// bool.
        /// </returns>
        /// <remarks>
        /// Converts System.Windows.Shapes.Rectangle to SlideChartDataPoint.
        /// </remarks>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
