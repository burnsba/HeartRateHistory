using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HeartRateHistory.Error
{
    /// <summary>
    /// Exception when reading skin xml file.
    /// </summary>
    public class InvalidConfiguration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfiguration"/> class.
        /// </summary>
        public InvalidConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfiguration"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public InvalidConfiguration(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfiguration"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidConfiguration(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfiguration"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected InvalidConfiguration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
