using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BurnsBac.Mvvm;
using HeartRateHistory.HotConfig;
using HeartRateHistory.MessageBus;
using HeartRateHistory.Windows;

namespace HeartRateHistory.ViewModels
{
    /// <summary>
    /// View model for skin config window.
    /// </summary>
    public class ConfigViewModel : WindowViewModelBase, IDisposable
    {
        /// <summary>
        /// Notification event for when the settings have changed on disk.
        /// </summary>
        public IMessageBusNotification SettingsChangedNotification;

        private SettingsCollection _settingSource = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigViewModel"/> class.
        /// </summary>
        public ConfigViewModel()
        {
            _settingSource = SettingsCollection.FromFile(SharedConfig.SettingsFileName);

            if (!object.ReferenceEquals(null, _settingSource))
            {
                try
                {
                    SettingItems = _settingSource.Items.Select(x => SettingsItemConverter(x)).ToList();
                }
                catch (Exception ex)
                {
                    SettingItems = new List<IConfigSetting>();

                    Workspace.RecreateSingletonWindow<ErrorWindow>(new ErrorWindowViewModel(ex)
                    {
                        HeaderMessage = "Error loading config settings",
                    });

                    return;
                }
            }

            CancelCommand = new RelayCommand<ICloseable>(CloseWindow);

            OkCommand = new RelayCommand<ICloseable>(w =>
            {
                SaveChanges();
                CloseWindow(w);
            });
        }

        /// <summary>
        /// Gets or sets cancel button command.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Gets or sets skin currently being configured.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets ok button command.
        /// </summary>
        public ICommand OkCommand { get; set; }

        /// <summary>
        /// Gets or sets list of settings items.
        /// </summary>
        public List<IConfigSetting> SettingItems { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var item in SettingItems)
            {
                item.Dispose();
            }
        }

        /// <summary>
        /// Writes config settings to settings json file.
        /// </summary>
        public void SaveChanges()
        {
            bool anyChanges = false;
            foreach (var uiitem in SettingItems)
            {
                var settingItem = _settingSource.Items.Where(x => x.Key == uiitem.Key && uiitem.IsModified).FirstOrDefault();

                if (!object.ReferenceEquals(null, settingItem))
                {
                    settingItem.CurrentValue = uiitem.CurrentValue ?? string.Empty;
                    anyChanges = true;
                }
            }

            if (anyChanges)
            {
                _settingSource.SaveChanges();
                MessageBus.MessageBus.Notify(this, nameof(SettingsChangedNotification), new EventArgs());
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Converts a json setting item to better typed object.
        /// </summary>
        /// <param name="item">Item to convert.</param>
        /// <returns>Config item.</returns>
        private IConfigSetting SettingsItemConverter(Setting item)
        {
            InputTypes type = InputTypesConverter.StringToInputTypes(item.Input);

            if (type == InputTypes.Textbox)
            {
                return new SkinConfigSettingTextboxViewModel(item);
            }
            else if (type == InputTypes.Dropdown)
            {
                return new SkinConfigSettingDropdownViewModel(item);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
