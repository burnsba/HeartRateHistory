using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BurnsBac.HotConfig;
using BurnsBac.HotConfig.DataSource;
using BurnsBac.HotConfig.Error;
using BurnsBac.WindowsAppToolkit.Dto;

namespace HeartRateHistory.ViewModels
{
    /// <summary>
    /// View model for skin config dropdown setting.
    /// </summary>
    public class SkinConfigSettingDropdownViewModel : ConfigSettingBase, INotifyPropertyChanged
    {
        private IConfigDataProvider _dataProvider = null;
        private bool _isPoll = false;
        private DropdownItem _selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinConfigSettingDropdownViewModel"/> class.
        /// </summary>
        /// <param name="item">Source item.</param>
        public SkinConfigSettingDropdownViewModel(Setting item)
            : base(item)
        {
            try
            {
                _dataProvider = TypeResolver.CreateConfigDataProviderInstance(item.Datasource, item.DatasourceAssembly);
            }
            catch (Exception ex)
            {
                var message = $"Could not resolve datasource. Datasource='{item.Datasource}', DatasourceAssembly='{item.DatasourceAssembly}'.";
                throw new InvalidConfiguration(message, ex);
            }

            var dataProviderType = _dataProvider.GetType();

            if (typeof(IConfigDataProviderOnce).IsAssignableFrom(dataProviderType))
            {
                var onceProvider = (IConfigDataProviderOnce)_dataProvider;
                Items = onceProvider.FetchData().Select(x => new DropdownItem() { Id = x.Key, Text = x.Value }).ToList();
            }
            else if (typeof(IConfigDataProviderPoll).IsAssignableFrom(dataProviderType))
            {
                Items = new List<DropdownItem>();

                _isPoll = true;
                WaitingForFirstPollResult = true;

                var pollProvider = (IConfigDataProviderPoll)_dataProvider;
                pollProvider.DataItems.CollectionChanged += DataProviderPollCollectionChanged;

                pollProvider.Start();
            }
            else
            {
                throw new InvalidConfiguration("Settings dropdown doesn't implement a known interface.");
            }

            SelectedItem = Items.Where(x => x.Id == CurrentValue).FirstOrDefault();

            IsModified = true;
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets list of dropdown data items.
        /// </summary>
        public List<DropdownItem> Items { get; set; }

        /// <summary>
        /// Gets or sets currently selected data item.
        /// </summary>
        public DropdownItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;
                if (!object.ReferenceEquals(null, _selectedItem))
                {
                    _settingsItem.CurrentValue = _selectedItem.Id;
                }
                else
                {
                    _settingsItem.CurrentValue = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the UI should notify the user the application is waiting for results.
        /// </summary>
        public bool ShowScanningInfo
        {
            get
            {
                return WaitingForFirstPollResult == true && _isPoll == true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any poll results have been received yet.
        /// </summary>
        public bool WaitingForFirstPollResult { get; set; } = false;

        /// <inheritdoc />
        public override void Dispose()
        {
            if (object.ReferenceEquals(null, _dataProvider))
            {
                return;
            }

            var dataProviderType = _dataProvider.GetType();

            if (typeof(IConfigDataProviderPoll).IsAssignableFrom(dataProviderType))
            {
                var pollProvider = (IConfigDataProviderPoll)_dataProvider;

                pollProvider.Stop();
                pollProvider.Dispose();
            }
        }

        /// <summary>
        /// Property changed notifier.
        /// </summary>
        /// <param name="property">Name of property that changed.</param>
        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void DataProviderPollCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool notify = false;

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (ConfigSettingItem item in e.NewItems)
                {
                    if (Items.Any(x => string.Compare(x.Id, item.Key, false) == 0))
                    {
                        continue;
                    }

                    WaitingForFirstPollResult = false;
                    Items.Add(new DropdownItem() { Id = item.Key, Text = item.Display });
                    notify = true;
                }
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (ConfigSettingItem item in e.OldItems)
                {
                    if (Items.RemoveAll(x => string.Compare(x.Id, item.Key, false) == 0) > 0)
                    {
                        notify = true;
                    }
                }
            }

            if (notify)
            {
                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(WaitingForFirstPollResult));
                OnPropertyChanged(nameof(ShowScanningInfo));
            }
        }
    }
}
