using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.HotConfig
{
    /// <summary>
    /// For a single setting from the settings.json file that can have multiple values, this
    /// is a container for one of those multiple values.
    /// </summary>
    public class ConfigSettingItem
    {
        /// <summary>
        /// Gets or sets the primary key, as an int. Optional.
        /// </summary>
        public int KeyInt { get; set; }

        /// <summary>
        /// Gets or sets the data source key. This (selected value) should end up getting saved as <see cref="IConfigSetting.CurrentValue"/>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the display text, e.g., in a combo box.
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// Gets or sets optional data object associated with this value.
        /// </summary>
        public object Data { get; set; }
    }
}
