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
    }
}
