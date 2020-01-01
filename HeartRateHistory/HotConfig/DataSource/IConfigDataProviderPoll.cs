using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HeartRateHistory.HotConfig.DataSource
{
    /// <summary>
    /// Interface for skin json settings dropdown source. Data is acquired through some kind of ongoing process. New items
    /// are added to the collection as discovered.
    /// </summary>
    public interface IConfigDataProviderPoll : IConfigDataProvider, IDisposable
    {
        /// <summary>
        /// Gets or sets the observable collection of data items.
        /// </summary>
        ObservableCollection<ConfigSettingItem> DataItems { get; set; }

        /// <summary>
        /// Begins acquiring data to populate items collection.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops acquiring items to populate collection.
        /// </summary>
        void Stop();
    }
}
