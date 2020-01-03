﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using BurnsBac.Mvvm;
using BurnsBac.WindowsHardware.Bluetooth.Characteristics;
using BurnsBac.WindowsHardware.Bluetooth.Sensors;
using HeartRateHistory.HotConfig;
using HeartRateHistory.Models;
using HeartRateHistory.Windows;

namespace HeartRateHistory.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string ConnectingText = "Connecting";
        private const string ConnectText = "Connect";
        private const string DataSeriesSaveFileDefault = "heart-";
        private const string DisconnectText = "Disconnect";
        private const string NoDataHeartRate = "---";
        private const string NoDataTimeSinceLastUpdate = "---";
        private const string PauseText = "Pause";
        private const string ResumeText = "Resume";
        private ulong _bluetoothDeviceAddress;
        private Timer _connectionWatchdogTimer;
        private string _currentHeartRate = NoDataHeartRate;
        private System.Windows.Media.FontFamily _currentHeartRateFontFamily = new System.Windows.Media.FontFamily("Segoe UI");
        private int _currentHeartRateFontSize = 32;
        private string _dataSeriesSaveFile;
        private Timer _heartPumpTimer;
        private LowEnergyHeartrateSensor _heartRateSensor;
        private bool _isConnected = false;
        private bool _isConnecting = false;
        private bool _isPaused = false;

        private int _lastReadValue = -1;
        private string _pauseResumeText = PauseText;
        private SettingsCollection _settingsSource;
        private DateTime _timeSinceLastUpdate = DateTime.MinValue;
        private string _timeSinceLastUpdateText = NoDataTimeSinceLastUpdate;
        private Timer _timeSinceUpdateTimer;

        public MainViewModel()
        {
            ReadConfig();

            ShowAppConfigWindowCommand = new CommandHandler(() => Workspace.CreateSingletonWindow<ConfigWindow>(this));

            PauseResumeCommand = new CommandHandler(PauseResumeCommandAction, x => IsConnected);
            ConnectDisconnectCommand = new CommandHandler(ConnectDisconnectCommandAction, x => GetCanConnectDisconnect());
            ReconnectCommand = new CommandHandler(() => { }, x => IsConnected);

            ResetCommand = new CommandHandler(() => SlideChartViewModel.Clear());
            SaveCommand = new CommandHandler(() => SlideChartViewModel.WriteValuesToFile(DataSeriesSaveFile));

            _heartPumpTimer = new Timer();
            _heartPumpTimer.AutoReset = true;
            _heartPumpTimer.Interval = 5000;
            _heartPumpTimer.Elapsed += HeartPumpTimer_Elapsed;

            _timeSinceUpdateTimer = new Timer();
            _timeSinceUpdateTimer.AutoReset = true;
            _timeSinceUpdateTimer.Interval = 1000;
            _timeSinceUpdateTimer.Elapsed += TimeSinceUpdateTimer_Elapsed;

            _connectionWatchdogTimer = new Timer();
            _connectionWatchdogTimer.AutoReset = false;
            _connectionWatchdogTimer.Interval = 5000;
            _connectionWatchdogTimer.Elapsed += ConnectionWatchdogTimer_Elapsed;

            SlideChartViewModel = new SlideChartViewModel(_settingsSource);
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
                OnPropertyChanged(nameof(CanConnect));
            }
        }

        public bool CanConnect
        {
            get
            {
                return _bluetoothDeviceAddress > 0 && !IsConnecting;
            }
        }

        public Action<int> ChangeHeartRateImageBpm { get; set; }

        public ICommand ConnectDisconnectCommand { get; set; }

        public string ConnectDisconnectText
        {
            get
            {
                if (IsConnected)
                {
                    return DisconnectText;
                }
                else if (IsConnecting)
                {
                    return ConnectingText;
                }
                else
                {
                    return ConnectText;
                }
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

        public string DataSeriesSaveFile
        {
            get
            {
                return _dataSeriesSaveFile;
            }

            set
            {
                _dataSeriesSaveFile = value + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";

                OnPropertyChanged(nameof(DataSeriesSaveFile));
            }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(CanConnect));
                OnPropertyChanged(nameof(ConnectDisconnectText));
                OnPropertyChanged(nameof(PauseResumeText));

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsConnecting
        {
            get
            {
                return _isConnecting;
            }

            set
            {
                _isConnecting = value;
                OnPropertyChanged(nameof(IsConnecting));
                OnPropertyChanged(nameof(CanConnect));
                OnPropertyChanged(nameof(ConnectDisconnectText));
                OnPropertyChanged(nameof(PauseResumeText));

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand PauseResumeCommand { get; set; }

        public string PauseResumeText
        {
            get
            {
                if (_isPaused)
                {
                    return ResumeText;
                }
                else
                {
                    return PauseText;
                }
            }
        }

        public Action PlayOnceImageDataXfer { get; set; }

        public ICommand ReconnectCommand { get; set; }

        public ICommand ResetCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand ShowAppConfigWindowCommand { get; set; }

        public SlideChartViewModel SlideChartViewModel { get; set; }

        public Action StopAnimations { get; set; }

        public string TimeSinceLastUpdate
        {
            get
            {
                return _timeSinceLastUpdateText;
            }

            set
            {
                _timeSinceLastUpdateText = value;
                OnPropertyChanged(nameof(TimeSinceLastUpdate));
            }
        }

        public void NotifyReloadConfig()
        {
            ReadConfig();

            SlideChartViewModel.NotifyReloadConfig();
        }

        public async void Start()
        {
            if (IsConnected || !object.ReferenceEquals(null, _heartRateSensor))
            {
                return;
            }

            IsConnecting = true;

            System.Diagnostics.Debug.WriteLine($"Starting {nameof(LowEnergyHeartrateSensor)}");

            _heartRateSensor = new LowEnergyHeartrateSensor(BluetoothDeviceAddress);
            _heartRateSensor.HeartRateReceivedEvent += InputEventMapper;

            try
            {
                await _heartRateSensor.FindCharacteristic();
            }
            catch (BurnsBac.WindowsHardware.Bluetooth.Error.CharacteristicNotFoundException ex)
            {
                _heartRateSensor.HeartRateReceivedEvent -= InputEventMapper;
                _heartRateSensor.Dispose();
                _heartRateSensor = null;
                IsConnecting = false;

                Workspace.CreateSingletonWindow<ErrorWindow>(new ErrorWindowViewModel(ex));
                return;
            }

            await _heartRateSensor.EnableNotifications();

            _timeSinceUpdateTimer.Start();
            _heartPumpTimer.Start();
            _connectionWatchdogTimer.Start();
            _isPaused = false;

            IsConnecting = false;
            IsConnected = true;
        }

        public async void Stop()
        {
            if (object.ReferenceEquals(null, _heartRateSensor))
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Stopping {nameof(LowEnergyHeartrateSensor)}");

            _connectionWatchdogTimer.Stop();

            await _heartRateSensor.DisableNotifications();

            _heartRateSensor.HeartRateReceivedEvent -= InputEventMapper;
            _heartRateSensor.Dispose();
            _heartRateSensor = null;

            StopAnimations();

            _timeSinceUpdateTimer.Stop();
            _heartPumpTimer.Stop();
            _lastReadValue = -1;

            TimeSinceLastUpdate = NoDataTimeSinceLastUpdate;
            CurrentHeartRate = NoDataHeartRate;

            IsConnected = false;
        }

        private void ConnectDisconnectCommandAction()
        {
            if (!IsConnected)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void ConnectionWatchdogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine("ConnectionWatchdogTimer_Elapsed");
            _connectionWatchdogTimer.Start();
        }

        private bool GetCanConnectDisconnect()
        {
            if (!IsConnected)
            {
                // Not connected. Can you connect?
                return CanConnect;
            }
            else
            {
                // Connected. Can you disconnect?
                return true;
            }
        }

        private void HeartPumpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ChangeHeartRateImageBpm(_lastReadValue);
        }

        /// <summary>
        /// Accepts <see cref="BurnsBac.WindowsHardware.Bluetooth.Characteristics.HeartRateMeasurement"/>.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="state">Event args.</param>
        private void InputEventMapper(object sender, HeartRateMeasurement state)
        {
            if (!IsConnected)
            {
                return;
            }

            if (!_isPaused)
            {
                var dp = new SlideChartDataPoint(DateTime.Now, state.HeartRate);
                SlideChartViewModel.AppendData(dp);
            }

            _connectionWatchdogTimer.Stop();
            _connectionWatchdogTimer.Start();

            CurrentHeartRate = state.HeartRate.ToString();

            PlayOnceImageDataXfer();

            _timeSinceLastUpdate = DateTime.Now;
            _lastReadValue = state.HeartRate;
        }

        private void PauseResumeCommandAction()
        {
            _isPaused = !_isPaused;
            OnPropertyChanged(nameof(PauseResumeText));
        }

        /// <summary>
        /// Reads app.config and sets global settings.
        /// </summary>
        private void ReadConfig()
        {
            _settingsSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            CurrentHeartRateFontFamily = new System.Windows.Media.FontFamily(_settingsSource.Items.First(x => x.Key == SharedConfig.CurrentHeartRateFontFamilyKey).CurrentValue);
            CurrentHeartRateFontSize = int.Parse(_settingsSource.Items.First(x => x.Key == SharedConfig.CurrentHeartRateFontSizeKey).CurrentValue);
            BluetoothDeviceAddress = ulong.Parse(_settingsSource.Items.First(x => x.Key == SharedConfig.BluetoothDeviceAddressKey).CurrentValue);
            DataSeriesSaveFile = _settingsSource.Items.First(x => x.Key == SharedConfig.DataSeriesSaveFileKey).CurrentValue;
        }

        private void TimeSinceUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_timeSinceLastUpdate > DateTime.MinValue)
            {
                var ts = (DateTime.Now - _timeSinceLastUpdate).TotalSeconds;

                if (ts < 1000)
                {
                    TimeSinceLastUpdate = Math.Round(ts, 2).ToString("000.00") + " sec";
                }
                else
                {
                    TimeSinceLastUpdate = ((int)ts).ToString() + " sec";
                }
            }
            else
            {
                TimeSinceLastUpdate = NoDataTimeSinceLastUpdate;
            }
        }
    }
}
