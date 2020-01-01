using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.HotConfig
{
    /// <summary>
    /// Single setting from the settings.json file. This is the UI item interface.
    /// </summary>
    public interface IConfigSetting : IDisposable
    {
        /// <summary>
        /// Gets or sets settings key name.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets or sets text to show to user.
        /// </summary>
        string Display { get; }

        /// <summary>
        /// Gets or sets the type of input.
        /// </summary>
        InputTypes InputType { get; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        string CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value has changed.
        /// </summary>
        bool IsModified { get; set; }
    }
}
