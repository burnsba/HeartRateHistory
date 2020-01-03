using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.HotConfig
{
    /// <summary>
    /// Single setting from the settings.json file. This is the UI item base class.
    /// </summary>
    public abstract class ConfigSettingBase : IConfigSetting
    {
        /// <summary>
        /// Underlying setting.
        /// </summary>
        protected Setting _settingsItem = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSettingBase"/> class.
        /// </summary>
        /// <param name="item">Item source.</param>
        public ConfigSettingBase(Setting item)
        {
            _settingsItem = item.Clone();
        }

        /// <inheritdoc />
        public string CurrentValue
        {
            get
            {
                return _settingsItem.CurrentValue;
            }

            set
            {
                _settingsItem.CurrentValue = value;
                IsModified = true;
            }
        }

        /// <inheritdoc />
        public string Display
        {
            get
            {
                return _settingsItem.Display;
            }

            set
            {
                _settingsItem.Display = value;
            }
        }

        /// <inheritdoc />
        public InputTypes InputType
        {
            get
            {
                return InputTypesConverter.StringToInputTypes(_settingsItem.Input);
            }

            set
            {
                _settingsItem.Input = InputTypesConverter.InputTypeToString(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this setting has been changed.
        /// </summary>
        public bool IsModified { get; set; } = false;

        /// <inheritdoc />
        public string Key
        {
            get
            {
                return _settingsItem.Key;
            }

            set
            {
                _settingsItem.Key = value;
            }
        }

        /// <inheritdoc />
        public abstract void Dispose();

        /// <summary>
        /// Converts back to a json setttings item.
        /// </summary>
        /// <returns>Underlying setting object.</returns>
        public virtual Setting ToSettingsItem()
        {
            return _settingsItem;
        }
    }
}
