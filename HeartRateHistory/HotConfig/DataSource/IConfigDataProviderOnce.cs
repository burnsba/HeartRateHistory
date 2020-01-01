using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.HotConfig.DataSource
{
    /// <summary>
    /// Interface for skin json settings dropdown source. Data is only queried and supplied once.
    /// </summary>
    public interface IConfigDataProviderOnce : IConfigDataProvider
    {
        /// <summary>
        /// Retrieves data to show end user.
        /// </summary>
        /// <returns>Key value pairs to be passed to dropdown.</returns>
        Dictionary<string, string> FetchData();
    }
}
