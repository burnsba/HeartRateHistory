using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory
{
    public static class SharedConfig
    {
        public const string SettingsFileName = "settings.json";

        public const string CurrentHeartRateFontFamilyKey = "HeartRateHistory.CurrentHeartRateFontFamily";
        public const string CurrentHeartRateFontSizeKey = "HeartRateHistory.CurrentHeartRateFontSize";
        public const string BluetoothDeviceAddressKey = "HeartRateHistory.DeviceAddress";
        public const string DataSeriesSaveFileKey = "HeartRateHistory.DataSeriesSaveFile";
        public const string DateTimeFormatKey = "HeartRateHistory.DateTimeFormat";
        public const string DataValueDisplayWidthKey = "HeartRateHistory.DataValueDisplayWidth";
        public const string ColorIntervalsKey = "HeartRateHistory.ColorIntervals";
        public const string HeartRateGreenFloorKey = "HeartRateHistory.HeartRateGreenFloor";
        public const string HeartRateRedCeilingKey = "HeartRateHistory.HeartRateRedCeiling";
    }
}
