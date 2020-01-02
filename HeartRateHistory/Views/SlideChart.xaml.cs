using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeartRateHistory.Converters;
using HeartRateHistory.HotConfig;
using HeartRateHistory.Models;
using HeartRateHistory.ViewModels;

namespace HeartRateHistory.Views
{
    /// <summary>
    /// Interaction logic for SlideChart.xaml .
    /// </summary>
    public partial class SlideChart : UserControl
    {
        private Timer _rescaleTimeoutTimer;

        private const int Line150StartHeight = 150;
        private const int Line125StartHeight = 125;
        private const int Line100StartHeight = 100;
        private const int Line75StartHeight = 75;
        private const int Line50StartHeight = 50;
        private const int Line150LabelStartHeight = 139;
        private const int Line125LabelStartHeight = 114;
        private const int Line100LabelStartHeight = 89;
        private const int Line75LabelStartHeight = 64;
        private const int Line50LabelStartHeight = 39;

        public SlideChart()
        {
            InitializeComponent();
        }

        private void SlideChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (object.ReferenceEquals(null, _rescaleTimeoutTimer))
            {
                _rescaleTimeoutTimer = new Timer();
                _rescaleTimeoutTimer.Interval = 1000;
                _rescaleTimeoutTimer.AutoReset = false;
                _rescaleTimeoutTimer.Elapsed += (s, e) => VisibleReloadFromData();
            }
            else
            {
                _rescaleTimeoutTimer.Stop();
            }

            _rescaleTimeoutTimer.Start();
        }

        private void VisibleReloadFromData()
        {
            Dispatcher.Invoke(() =>
            {
                var vm = (SlideChartViewModel)DataContext;
                vm.ContainerDisplayHeight = SlideChartX.ActualHeight;
                vm.VisibleReloadFromData();

                var areaHeight = SlideChartX.ActualHeight;
                var scaleFactor = areaHeight / (double)SlideChartViewModel.InitialSeriesPanelHeight;

                if (areaHeight > SlideChartViewModel.InitialSeriesPanelHeight)
                {
                    AssignBottomMargin(RectLine50, scaleFactor * Line50StartHeight);
                    AssignBottomMargin(RectLine75, scaleFactor * Line75StartHeight);
                    AssignBottomMargin(RectLine100, scaleFactor * Line100StartHeight);
                    AssignBottomMargin(RectLine125, scaleFactor * Line125StartHeight);
                    AssignBottomMargin(RectLine150, scaleFactor * Line150StartHeight);

                    AssignBottomMargin(RectLine50Label, (scaleFactor * Line50StartHeight) - 11);
                    AssignBottomMargin(RectLine75Label, (scaleFactor * Line75StartHeight) - 11);
                    AssignBottomMargin(RectLine100Label, (scaleFactor * Line100StartHeight) - 11);
                    AssignBottomMargin(RectLine125Label, (scaleFactor * Line125StartHeight) - 11);
                    AssignBottomMargin(RectLine150Label, (scaleFactor * Line150StartHeight) - 11);
                }
                else
                {
                    AssignBottomMargin(RectLine50, Line50StartHeight);
                    AssignBottomMargin(RectLine75, Line75StartHeight);
                    AssignBottomMargin(RectLine100, Line100StartHeight);
                    AssignBottomMargin(RectLine125, Line125StartHeight);
                    AssignBottomMargin(RectLine150, Line150StartHeight);

                    AssignBottomMargin(RectLine50Label, Line50LabelStartHeight);
                    AssignBottomMargin(RectLine75Label, Line75LabelStartHeight);
                    AssignBottomMargin(RectLine100Label, Line100LabelStartHeight);
                    AssignBottomMargin(RectLine125Label, Line125LabelStartHeight);
                    AssignBottomMargin(RectLine150Label, Line150LabelStartHeight);
                }
            });
        }

        private void AssignBottomMargin(FrameworkElement element, double size)
        {
            Thickness margin = element.Margin;
            margin.Bottom = size;
            element.Margin = margin;
        }
    }
}
