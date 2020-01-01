using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HeartRateHistory.HotConfig
{
    /// <summary>
    /// Single setting from the settings.json file. This is the serializable implementation.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Gets or sets settings key name.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets text to show to user.
        /// </summary>
        [JsonProperty("display")]
        public string Display { get; set; }

        /// <summary>
        /// Gets or sets the type of input.
        /// </summary>
        [JsonProperty("input")]
        public string Input { get; set; }

        /// <summary>
        /// Gets or sets the datasource type name.
        /// </summary>
        [JsonProperty("datasource")]
        public string Datasource { get; set; }

        /// <summary>
        /// Gets or sets the datasource assembly name.
        /// </summary>
        [JsonProperty("assembly")]
        public string DatasourceAssembly { get; set; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        [JsonProperty("currentValue")]
        public string CurrentValue { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Key;
        }

        /// <summary>
        /// Creates a copy of the current item.
        /// </summary>
        /// <returns>Copy.</returns>
        public Setting Clone()
        {
            return new Setting()
            {
                Key = Key,
                Display = Display,
                Input = Input,
                Datasource = Datasource,
                CurrentValue = CurrentValue,
            };
        }
    }
}
