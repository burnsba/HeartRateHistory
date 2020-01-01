using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace HeartRateHistory.HotConfig
{
    /// <summary>
    /// Parent collection from the settings.json file. This is the serializable implementation.
    /// </summary>
    public class SettingsCollection
    {
        private string _path;

        /// <summary>
        /// Gets or sets list of settings.
        /// </summary>
        [JsonProperty("settings")]
        public List<Setting> Items { get; set; }

        /// <summary>
        /// Writes current object back to the settings file.
        /// </summary>
        public void SaveChanges()
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);

            System.IO.File.WriteAllText(_path, output);
        }

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <returns>Parsed settings.</returns>
        public static SettingsCollection FromFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            var text = System.IO.File.ReadAllText(path);

            var settings = JsonConvert.DeserializeObject<SettingsCollection>(text);
            settings._path = path;

            return settings;
        }

        /// <summary>
        /// Converts settings to dictionary.
        /// </summary>
        /// <returns>Dictionary of Key,CurrentValue pairs.</returns>
        public Dictionary<string, string> ToSettingsDictionary()
        {
            return Items.ToDictionary(x => x.Key, x => x.CurrentValue);
        }
    }
}
