using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BurnsBac.Mvvm;
using HeartRateHistory.HotConfig;
using HeartRateHistory.Windows;

namespace HeartRateHistory.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string PauseText = "Pause";
        private const string ResumeText = "Resume";
        private const string ConnectText = "Connect";
        private const string DisconnectText = "Disconnect";

        private const string NoDataHeartRate = "---";
        private const string NoDataTimeSinceLastUpdate = "---";

        private bool _currentlyConnected = false;

        private string _pauseResetText = PauseText;
        private string _connectDisconnectText = ConnectText;
        private string _currentHeartRate = NoDataHeartRate;
        private string _timeSinceLastUpdate = NoDataTimeSinceLastUpdate;
        private int _currentHeartRateFontSize = 32;
        private System.Windows.Media.FontFamily _currentHeartRateFontFamily = new System.Windows.Media.FontFamily("Segoe UI");
        private ulong _bluetoothDeviceAddress;

        public MainViewModel()
        {
            ReadConfig();

            ShowAppConfigWindowCommand = new CommandHandler(() => Workspace.CreateSingletonWindow<ConfigWindow>(this));

            PauseResumeCommand = new CommandHandler(() => { }, x => _currentlyConnected);
            ConnectDisconnectCommand = new CommandHandler(() => { }, x => !_currentlyConnected);
            ReconnectCommand = new CommandHandler(() => { }, x => _currentlyConnected);
        }

        public ICommand ShowAppConfigWindowCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand PauseResumeCommand { get; set; }

        public ICommand ConnectDisconnectCommand { get; set; }

        public ICommand ReconnectCommand { get; set; }

        public string ConnectDisconnectText
        {
            get
            {
                return _connectDisconnectText;
            }

            set
            {
                _connectDisconnectText = value;
                OnPropertyChanged(nameof(ConnectDisconnectText));
            }
        }

        public string CurrentHeartRate
        {
            get
            {
                return _currentHeartRate;
            }

            set
            {
                _currentHeartRate = value;
                OnPropertyChanged(nameof(CurrentHeartRate));
            }
        }

        public string PauseResetText
        {
            get
            {
                return _pauseResetText;
            }

            set
            {
                _pauseResetText = value;
                OnPropertyChanged(nameof(PauseResetText));
            }
        }

        public string TimeSinceLastUpdate
        {
            get
            {
                return _timeSinceLastUpdate;
            }

            set
            {
                _timeSinceLastUpdate = value;
                OnPropertyChanged(nameof(TimeSinceLastUpdate));
            }
        }

        public int CurrentHeartRateFontSize
        {
            get
            {
                return _currentHeartRateFontSize;
            }

            set
            {
                _currentHeartRateFontSize = value;
                OnPropertyChanged(nameof(CurrentHeartRateFontSize));
            }
        }

        public System.Windows.Media.FontFamily CurrentHeartRateFontFamily
        {
            get
            {
                return _currentHeartRateFontFamily;
            }

            set
            {
                _currentHeartRateFontFamily = value;
                OnPropertyChanged(nameof(CurrentHeartRateFontFamily));
            }
        }

        public ulong BluetoothDeviceAddress
        {
            get
            {
                return _bluetoothDeviceAddress;
            }

            set
            {
                _bluetoothDeviceAddress = value;
                OnPropertyChanged(nameof(BluetoothDeviceAddress));
            }
        }

        public void NotifyReloadConfig()
        {
            ReadConfig();
        }

        /// <summary>
        /// Reads app.config and sets global settings.
        /// </summary>
        private void ReadConfig()
        {
            var settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            CurrentHeartRateFontFamily = new System.Windows.Media.FontFamily(settingSource.Items.First(x => x.Key == SharedConfig.CurrentHeartRateFontFamilyKey).CurrentValue);
            CurrentHeartRateFontSize = int.Parse(settingSource.Items.First(x => x.Key == SharedConfig.CurrentHeartRateFontSizeKey).CurrentValue);
            BluetoothDeviceAddress = ulong.Parse(settingSource.Items.First(x => x.Key == SharedConfig.BluetoothDeviceAddressKey).CurrentValue);
        }
    }
}
