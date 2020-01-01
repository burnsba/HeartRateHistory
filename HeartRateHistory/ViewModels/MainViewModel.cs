using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
        private const string PauseText = "Pause";
        private const string ResumeText = "Resume";
        private const string ConnectText = "Connect";
        private const string ConnectingText = "Connecting";
        private const string DisconnectText = "Disconnect";

        private const string NoDataHeartRate = "---";
        private const string NoDataTimeSinceLastUpdate = "---";
        private const string DataSeriesSaveFileDefault = "heart-";

        private bool _isConnected = false;
        private bool _isConnecting = false;

        private string _pauseResetText = PauseText;
        private string _currentHeartRate = NoDataHeartRate;
        private string _timeSinceLastUpdate = NoDataTimeSinceLastUpdate;
        private int _currentHeartRateFontSize = 32;
        private System.Windows.Media.FontFamily _currentHeartRateFontFamily = new System.Windows.Media.FontFamily("Segoe UI");
        private ulong _bluetoothDeviceAddress;
        private string _dataSeriesSaveFile;

        private LowEnergyHeartrateSensor _heartRateSensor;

        public MainViewModel()
        {
            ReadConfig();

            ShowAppConfigWindowCommand = new CommandHandler(() => Workspace.CreateSingletonWindow<ConfigWindow>(this));

            PauseResumeCommand = new CommandHandler(() => { }, x => IsConnected);
            ConnectDisconnectCommand = new CommandHandler(ConnectDisconnectCommandAction, x => GetCanConnectDisconnect());
            ReconnectCommand = new CommandHandler(() => { }, x => IsConnected);

            ResetCommand = new RelayCommand<ITimeSeriesChart>(x => x.Clear());
            SaveCommand = new RelayCommand<ITimeSeriesChart>(x => x.WriteValuesToFile(DataSeriesSaveFile));
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
                OnPropertyChanged(nameof(PauseResetText));

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
                OnPropertyChanged(nameof(PauseResetText));

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool CanConnect
        {
            get
            {
                return _bluetoothDeviceAddress > 0 && !IsConnecting;
            }
        }

        public ICommand ShowAppConfigWindowCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public ICommand PauseResumeCommand { get; set; }

        public ICommand ConnectDisconnectCommand { get; set; }

        public ICommand ReconnectCommand { get; set; }

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
                OnPropertyChanged(nameof(CanConnect));
            }
        }

        public Func<SlideChartDataPoint, bool> AppendDataMethod
        {
            get;
            set;
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
            DataSeriesSaveFile = settingSource.Items.First(x => x.Key == SharedConfig.DataSeriesSaveFileKey).CurrentValue;
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

            await _heartRateSensor.FindCharacteristic();
            await _heartRateSensor.EnableNotifications();

            IsConnecting = false;
            IsConnected = true;
        }

        /// <inheritdoc />
        public async void Stop()
        {
            if (object.ReferenceEquals(null, _heartRateSensor))
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Stopping {nameof(LowEnergyHeartrateSensor)}");

            await _heartRateSensor.DisableNotifications();

            _heartRateSensor.Dispose();
            _heartRateSensor = null;

            IsConnected = false;
        }

        /// <summary>
        /// Accepts <see cref="BurnsBac.WindowsHardware.Bluetooth.Characteristics.HeartRateMeasurement"/> and translates to <see cref="GenericInputEventArgs"/>.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="state">Event args.</param>
        private void InputEventMapper(object sender, HeartRateMeasurement state)
        {
            if (!IsConnected)
            {
                return;
            }
            
            var dp = new SlideChartDataPoint(DateTime.Now, state.HeartRate);
            AppendDataMethod(dp);

            CurrentHeartRate = dp.Value.ToString();

            //if (!AnyEventListeners())
            //{
            //    return;
            //}

            //var genArgs = new GenericInputEventArgs();

            //genArgs.RangeableInputs.Add(new GenericIntRangeableInput(state.HeartRate) { Id = 1 });

            //FireEventHandler(sender, genArgs);
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
    }
}
