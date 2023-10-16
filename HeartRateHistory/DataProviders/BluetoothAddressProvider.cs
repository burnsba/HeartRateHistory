using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BurnsBac.HotConfig;
using BurnsBac.HotConfig.DataSource;
using BurnsBac.WindowsHardware.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace HeartRateHistory.DefaultConfigDataProviders
{
    /// <summary>
    /// Scans for connected bluetooth devices.
    /// </summary>
    public class BluetoothAddressProvider : IConfigDataProviderPoll, IDisposable
    {
        private List<BluetoothDeviceInfo> _bluetoothDevices = new List<BluetoothDeviceInfo>();
        private bool _currentlyScanning = false;
        private System.Timers.Timer _scanTimer;
        private HashSet<ulong> _seenDeviceAddressess = new HashSet<ulong>();
        private BluetoothLEAdvertisementWatcher? _watcher = null;
        private Thread _worker;
        private bool _workerStop = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothAddressProvider"/> class.
        /// </summary>
        public BluetoothAddressProvider()
        {
            DataItems = new ObservableCollection<ConfigSettingItem>();

            if (false)
            {
                // Create Bluetooth Listener
                _watcher = new BluetoothLEAdvertisementWatcher();

                _watcher.ScanningMode = BluetoothLEScanningMode.Active;

                // Only activate the watcher when we're recieving values >= -80
                _watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

                // Stop watching if the value drops below -90 (user walked away)
                _watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

                // Register callback for when we see an advertisements
                _watcher.Received += OnAdvertisementReceived;

                // Wait 5 seconds to make sure the device is really out of range
                _watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
                _watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(1000);
            }

            if (false)
            {
                _workerStop = false;

                _worker = new Thread(ThreadMain);
                _worker.IsBackground = true;
                _worker.Start();
            }
        }

        /// <inheritdoc />
        public ObservableCollection<ConfigSettingItem> DataItems { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Starts scanning for bluetooth devices.
        /// </summary>
        public void Start()
        {
            if (_currentlyScanning)
            {
                return;
            }

            _currentlyScanning = true;

            if (object.ReferenceEquals(null, _scanTimer))
            {
                _scanTimer = new System.Timers.Timer();
                _scanTimer.Interval = 20000;
                _scanTimer.AutoReset = false;
                _scanTimer.Elapsed += (s, e) => { Stop(); };
                _scanTimer.Start();
            }
            else
            {
                _scanTimer.Stop();
                _scanTimer.Start();
            }
        }

        /// <summary>
        /// Stops scanning for bluetooth devices.
        /// </summary>
        public void Stop()
        {
            if (!object.ReferenceEquals(null, _watcher))
            {
                _watcher.Stop();
            }

            _workerStop = true;

            if (!object.ReferenceEquals(null, _scanTimer))
            {
                _scanTimer.Stop();
            }

            _currentlyScanning = false;
        }

        /// <summary>
        /// Event handler when a new advertisement is received.
        /// </summary>
        /// <param name="watcher">Watcher.</param>
        /// <param name="eventArgs">Args.</param>
        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            if (_seenDeviceAddressess.Contains(eventArgs.BluetoothAddress))
            {
                return;
            }

            if (!string.IsNullOrEmpty(eventArgs.Advertisement.LocalName))
            {
                var info = new BluetoothDeviceInfo()
                {
                    Address = eventArgs.BluetoothAddress,
                    LocalName = eventArgs.Advertisement.LocalName,
                    ServiceUuids = new List<Guid>(eventArgs.Advertisement.ServiceUuids), // probably empty
                };

                _bluetoothDevices.Add(info);
                _seenDeviceAddressess.Add(eventArgs.BluetoothAddress);

                DataItems.Add(new ConfigSettingItem()
                {
                    Data = info,
                    Key = info.Address.ToString(),
                    Display = info.LocalName,
                });
            }

            // Tell the user we see an advertisement and print some properties
            System.Diagnostics.Debug.WriteLine(String.Format($"Advertisement: ({eventArgs.AdvertisementType.ToString()})"));
            System.Diagnostics.Debug.WriteLine(String.Format("  BT_ADDR: {0}", eventArgs.BluetoothAddress));
            System.Diagnostics.Debug.WriteLine(String.Format("  FR_NAME: {0}", eventArgs.Advertisement.LocalName));
            System.Diagnostics.Debug.WriteLine(String.Format("  ServiceUuids: {0}", String.Join(",", eventArgs.Advertisement.ServiceUuids.Select(x => x.ToString()))));
            System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        private void ThreadMain()
        {
            //DeviceInformationCollection btlecollection = DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true)).GetAwaiter().GetResult();

            //foreach (var btledi in btlecollection)
            //{
            //    if (_workerStop)
            //    {
            //        break;
            //    }

            //    var btled = BluetoothLEDevice.FromIdAsync(btledi.Id).GetAwaiter().GetResults();

            //    AddBluetooLEDevice(btled);
            //}

            _currentlyScanning = false;
        }

        private void AddBluetooLEDevice(BluetoothLEDevice device)
        {
            if (_seenDeviceAddressess.Contains(device.BluetoothAddress))
            {
                return;
            }

            if (!string.IsNullOrEmpty(device.Name))
            {
                var info = new BluetoothDeviceInfo()
                {
                    Address = device.BluetoothAddress,
                    LocalName = device.Name,
                    ServiceUuids = new List<Guid>(), // probably empty
                };

                _bluetoothDevices.Add(info);
                _seenDeviceAddressess.Add(device.BluetoothAddress);

                DataItems.Add(new ConfigSettingItem()
                {
                    Data = info,
                    Key = info.Address.ToString(),
                    Display = info.LocalName,
                });
            }

            // Tell the user we see an advertisement and print some properties
            System.Diagnostics.Debug.WriteLine(String.Format($"Advertisement: (none)"));
            System.Diagnostics.Debug.WriteLine(String.Format("  BT_ADDR: {0}", device.BluetoothAddress));
            System.Diagnostics.Debug.WriteLine(String.Format("  FR_NAME: {0}", device.Name));
            System.Diagnostics.Debug.WriteLine(String.Format("  ServiceUuids: {0}", string.Empty));
            System.Diagnostics.Debug.WriteLine(string.Empty);
        }
    }
}
