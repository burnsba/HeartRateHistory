using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HeartRateHistory.Error
{
    /// <summary>
    /// Exception when reading skin xml file, can not ui display type to known type.
    /// </summary>
    public class UiNotSupported : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UiNotSupported"/> class.
        /// </summary>
        public UiNotSupported()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UiNotSupported"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public UiNotSupported(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UiNotSupported"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public UiNotSupported(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UiNotSupported"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected UiNotSupported(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
