using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HeartRateHistory.HotConfig;
using HeartRateHistory.Dto;
using BurnsBac.Mvvm;
using HeartRateHistory.Windows;

namespace HeartRateHistory.ViewModels
{
    /// <summary>
    /// View model for skin config window.
    /// </summary>
    public class ConfigViewModel : WindowViewModelBase, IDisposable
    {
        private SettingsCollection _settingSource = null;

        /// <summary>
        /// Gets or sets list of settings items.
        /// </summary>
        public List<IConfigSetting> SettingItems { get; set; }

        /// <summary>
        /// Gets or sets skin currently being configured.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets ok button command.
        /// </summary>
        public ICommand OkCommand { get; set; }

        /// <summary>
        /// Gets or sets cancel button command.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigViewModel"/> class.
        /// </summary>
        /// <param name="askv">Source info.</param>
        public ConfigViewModel(MainViewModel parent)
        {
            //DisplayName = askv.DisplayName;
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
                parent.NotifyReloadConfig();
                Converters.HeartRateRgbConverter.Setup();
                CloseWindow(w);
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Writes config settings to settings json file.
        /// </summary>
        public void SaveChanges()
        {
            foreach (var uiitem in SettingItems)
            {
                var settingItem = _settingSource.Items.Where(x => x.Key == uiitem.Key && uiitem.IsModified).FirstOrDefault();

                if (!object.ReferenceEquals(null, settingItem))
                {
                    settingItem.CurrentValue = uiitem.CurrentValue ?? string.Empty;
                }
            }

            _settingSource.SaveChanges();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var item in SettingItems)
            {
                item.Dispose();
            }
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
