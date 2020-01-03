using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory
{
    /// <summary>
    /// Settings helper.
    /// </summary>
    public static class SharedConfig
    {
        /// <summary>
        /// Name of file containing settings.
        /// </summary>
        public const string SettingsFileName = "settings.json";

        /// <summary>
        /// Settings key for <see cref="ViewModels.MainViewModel.CurrentHeartRateFontFamily"/>.
        /// </summary>
        public const string CurrentHeartRateFontFamilyKey = "HeartRateHistory.CurrentHeartRateFontFamily";

        /// <summary>
        /// Settings key for <see cref="ViewModels.MainViewModel.CurrentHeartRateFontSize"/>.
        /// </summary>
        public const string CurrentHeartRateFontSizeKey = "HeartRateHistory.CurrentHeartRateFontSize";

        /// <summary>
        /// Settings key for <see cref="ViewModels.MainViewModel.BluetoothDeviceAddress"/>.
        /// </summary>
        public const string BluetoothDeviceAddressKey = "HeartRateHistory.DeviceAddress";

        /// <summary>
        /// Settings key for <see cref="ViewModels.MainViewModel.DataSeriesSaveFile"/>.
        /// </summary>
        public const string DataSeriesSaveFileKey = "HeartRateHistory.DataSeriesSaveFile";

        /// <summary>
        /// Settings key for DateTimeFormat.
        /// </summary>
        public const string DateTimeFormatKey = "HeartRateHistory.DateTimeFormat";

        /// <summary>
        /// Settings key for <see cref="ViewModels.SlideChartViewModel.DataValueDisplayWidth"/>.
        /// </summary>
        public const string DataValueDisplayWidthKey = "HeartRateHistory.DataValueDisplayWidth";

        /// <summary>
        /// Settings key for <see cref="Converters.HeartRateRgbConverter.ColorIntervals"/>.
        /// </summary>
        public const string ColorIntervalsKey = "HeartRateHistory.ColorIntervals";

        /// <summary>
        /// Settings key for <see cref="Converters.HeartRateRgbConverter.MinCutoff"/>.
        /// </summary>
        public const string HeartRateGreenFloorKey = "HeartRateHistory.HeartRateGreenFloor";

        /// <summary>
        /// Settings key for <see cref="Converters.HeartRateRgbConverter.MaxCutoff"/>.
        /// </summary>
        public const string HeartRateRedCeilingKey = "HeartRateHistory.HeartRateRedCeiling";

        /// <summary>
        /// Settings key for <see cref="ViewModels.SlideChartViewModel.BackgroundColor"/>.
        /// </summary>
        public const string SlideChartBackgroundColorKey = "HeartRateHistory.SlideChartBackgroundColor";

        /// <summary>
        /// Settings key for <see cref="ViewModels.SlideChartViewModel.BackgroundDataLineColor"/>.
        /// </summary>
        public const string SlideChartDataLineColorKey = "HeartRateHistory.SlideChartDataLineColor";

        /// <summary>
        /// Settings key for <see cref="ViewModels.SlideChartViewModel.BackgroundDataLineLabelColor"/>.
        /// </summary>
        public const string SlideChartDataLineLabelColorKey = "HeartRateHistory.SlideChartDataLineLabelColor";

        /// <summary>
        /// Settings key for <see cref="ViewModels.SlideChartViewModel.MaxItemsDisplayed"/>.
        /// </summary>
        public const string MaxItemsDisplayedKey = "HeartRateHistory.MaxItemsDisplayed";

        /// <summary>
        /// Settings key for <see cref="ViewModels.SlideChartViewModel.MaxItemsInMemory"/>.
        /// </summary>
        public const string MaxItemsInMemoryKey = "HeartRateHistory.MaxItemsInMemory";
    }
}
