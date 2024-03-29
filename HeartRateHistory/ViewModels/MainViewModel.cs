﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using BurnsBac.HotConfig;
using BurnsBac.WindowsAppToolkit;
using BurnsBac.WindowsAppToolkit.Mvvm;
using BurnsBac.WindowsAppToolkit.Services.MessageBus;
using BurnsBac.WindowsAppToolkit.ViewModels;
using BurnsBac.WindowsAppToolkit.Windows;
using BurnsBac.WindowsHardware.Bluetooth.Characteristics;
using BurnsBac.WindowsHardware.Bluetooth.Sensors;
using HeartRateHistory.Models;
using HeartRateHistory.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace HeartRateHistory.ViewModels
{
    /// <summary>
    /// View model for main window.
    /// </summary>
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
        private LowEnergyHeartrateSensor _heartRateSensor;
        private bool _isConnected = false;
        private bool _isConnecting = false;
        private bool _isPaused = false;

        private SettingsCollection _settingsSource;
        private int _lastReadValue = -1;
        private DateTime _timeSinceLastUpdate = DateTime.MinValue;
        private string _timeSinceLastUpdateText = NoDataTimeSinceLastUpdate;
        private Timer _timeSinceUpdateTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            ReadConfig();

            BurnsBac.WindowsAppToolkit.Services.MessageBus.MessageBus.Subscribe<ConfigViewModel, MainViewModel>(nameof(ConfigViewModel.SettingsChangedNotification), this, SettingsChangeHandler);

            ShowAppConfigWindowCommand = new CommandHandler(() => Workspace.CreateSingletonWindow<ConfigWindow>(SharedConfig.SettingsFileName));

            PauseResumeCommand = new CommandHandler(PauseResumeCommandAction, x => IsConnected);
            ConnectDisconnectCommand = new CommandHandler(ConnectDisconnectCommandAction, x => GetCanConnectDisconnect());
            ReconnectCommand = new CommandHandler(() => { }, x => IsConnected);

            ResetCommand = new CommandHandler(() => SlideChartViewModel.Clear());
            SaveCommand = new CommandHandler(() => SlideChartViewModel.WriteValuesToFile(DataSeriesSaveFile));

            _timeSinceUpdateTimer = new Timer();
            _timeSinceUpdateTimer.AutoReset = true;

            // Drift a bit off exactly 1 second.
            // (first prime > 1000).
            _timeSinceUpdateTimer.Interval = 1019;
            _timeSinceUpdateTimer.Elapsed += TimeSinceUpdateTimer_Elapsed;

            _connectionWatchdogTimer = new Timer();
            _connectionWatchdogTimer.AutoReset = false;
            _connectionWatchdogTimer.Interval = 5000;
            _connectionWatchdogTimer.Elapsed += ConnectionWatchdogTimer_Elapsed;

            SlideChartViewModel = new SlideChartViewModel(_settingsSource);
        }

        /// <summary>
        /// Events raised when data is received.
        /// </summary>
        public event EventHandler<HeartRateMeasurement> DataReceived;

        /// <summary>
        /// Event raised when user disconnects from bluetooth device (when <see cref="Stop"/> is called).
        /// </summary>
        public event EventHandler StopNotification;

        /// <summary>
        /// Gets or sets the address of the bluetooth device to connect to.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether it is possible to connect.
        /// </summary>
        public bool CanConnect
        {
            get
            {
                return _bluetoothDeviceAddress > 0 && !IsConnecting;
            }
        }

        /// <summary>
        /// Gets or sets the command used by the "Connect/Disconnect" button.
        /// </summary>
        public ICommand ConnectDisconnectCommand { get; set; }

        /// <summary>
        /// Gets the text used by the "Connect/Disconnect" button.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the current rate.
        /// </summary>
        /// <value>
        /// <see cref="int" /> if available, or <see cref="MainViewModel.NoDataHeartRate"/> if not.
        /// </value>
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

        /// <summary>
        /// Gets or sets the time since the last update from the connected device.
        /// </summary>
        /// <value>
        /// <see cref="double" /> if available, or <see cref="MainViewModel.NoDataTimeSinceLastUpdate"/> if not.
        /// </value>
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

        /// <summary>
        /// Gets or sets the font used to render the current heart rate.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the font size used to display the current heart rate.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the file used to save the data collection in memory to disk.
        /// </summary>
        public string DataSeriesSaveFile
        {
            get
            {
                return _dataSeriesSaveFile;
            }

            set
            {
                // TODO: use SlideChartViewModel.DateTimeFormat
                _dataSeriesSaveFile = value + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";

                OnPropertyChanged(nameof(DataSeriesSaveFile));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bluetooth device is currently connected.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether the is an active attempt to connect
        /// to the bluetooth device.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the command for the "Pause/Resume" button.
        /// </summary>
        public ICommand PauseResumeCommand { get; set; }

        /// <summary>
        /// Gets the text for the "Pause/Resume" button.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the command used by the reconnect button (not used).
        /// </summary>
        public ICommand ReconnectCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used by the reset button.
        /// </summary>
        public ICommand ResetCommand { get; set; }

        /// <summary>
        /// Gets or sets the command used by the save button.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to show the settings window.
        /// </summary>
        public ICommand ShowAppConfigWindowCommand { get; set; }

        /// <summary>
        /// Gets or sets the associated viewmodel for the chart.
        /// </summary>
        public SlideChartViewModel SlideChartViewModel { get; set; }

        /// <summary>
        /// Connects to the bluetooth device and starts receiving data. Data is then
        /// forwarded to the chart.
        /// </summary>
        public async void Start()
        {
            if (IsConnected || !object.ReferenceEquals(null, _heartRateSensor))
            {
                return;
            }

            IsConnecting = true;

            System.Diagnostics.Debug.WriteLine($"Starting {nameof(LowEnergyHeartrateSensor)}");

            DeviceInformationCollection btlecollection = DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true)).GetAwaiter().GetResult();

            BluetoothDeviceAddress = 0;

            foreach (var btledi in btlecollection)
            {
                BluetoothLEDevice btled = BluetoothLEDevice.FromIdAsync(btledi.Id).GetAwaiter().GetResult();

                // heart rate sensor
                if (btled.Appearance.Category == 13)
                {
                    BluetoothDeviceAddress = btled.BluetoothAddress;

                    break;
                }
            }

            if (BluetoothDeviceAddress == 0)
            {
                Workspace.CreateSingletonWindow<ErrorWindow>(new ErrorWindowViewModel(new Exception("No low energy bluetooth heart rate sensor found")));
                return;
            }

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

                //Workspace.CreateSingletonWindow<ErrorWindow>(new ErrorWindowViewModel(ex));
                return;
            }
            catch (BurnsBac.WindowsHardware.Bluetooth.Error.ServiceNotFoundException ex)
            {
                // TODO: the DeviceAddress doesn't change if the title of the dropdown is the same.
                // so every time the battery dies, the settings.json bluetooth address
                // goes invalid and cannot be updated from the UI.
                _heartRateSensor.HeartRateReceivedEvent -= InputEventMapper;
                _heartRateSensor.Dispose();
                _heartRateSensor = null;
                IsConnecting = false;

                Workspace.CreateSingletonWindow<ErrorWindow>(new ErrorWindowViewModel(ex));
                return;
            }

            await _heartRateSensor.EnableNotifications();

            // Clear on start, but leave intact after disconnect for save.
            SlideChartViewModel.Clear();

            _timeSinceUpdateTimer.Start();
            _connectionWatchdogTimer.Start();
            _isPaused = false;

            IsConnecting = false;
            IsConnected = true;
        }

        /// <summary>
        /// Stops receiving data and disconnects from the bluetooth device.
        /// </summary>
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

            OnStopNotification();

            _timeSinceUpdateTimer.Stop();
            _lastReadValue = -1;

            TimeSinceLastUpdate = NoDataTimeSinceLastUpdate;
            CurrentHeartRate = NoDataHeartRate;

            IsConnected = false;

            OnStopNotification();
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

        /// <summary>
        /// Watchdog event. Was designed to be used with reconnect command, but currently on hold.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
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

        /// <summary>
        /// Accepts <see cref="BurnsBac.WindowsHardware.Bluetooth.Characteristics.HeartRateMeasurement"/>,
        /// adds data to the chart.
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

            _timeSinceLastUpdate = DateTime.Now;
            _lastReadValue = state.HeartRate;

            OnDataReceived(state);
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

        private void OnDataReceived(HeartRateMeasurement data)
        {
            DataReceived?.Invoke(this, data);
        }

        private void OnStopNotification()
        {
            StopNotification?.Invoke(this, null);
        }

        private void SettingsChangeHandler(object sender, EventArgs args)
        {
            ReadConfig();
        }
    }
}
