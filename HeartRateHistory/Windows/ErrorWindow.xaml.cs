using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BurnsBac.Mvvm;
using HeartRateHistory.ViewModels;

namespace HeartRateHistory.Windows
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml .
    /// </summary>
    public partial class ErrorWindow : Window, ICloseable
    {
        private ErrorWindowViewModel _vm = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        /// <param name="vm">ViewModel for error window.</param>
        public ErrorWindow(ErrorWindowViewModel vm)
        {
            InitializeComponent();

            _vm = vm;

            DataContext = _vm;
        }
    }
}
