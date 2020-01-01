using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.Dto
{
    /// <summary>
    /// Basic drop down list item.
    /// </summary>
    public class DropdownItem
    {
        /// <summary>
        /// Gets or sets text to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets id to use.
        /// </summary>
        public string Id { get; set; }
    }
}
