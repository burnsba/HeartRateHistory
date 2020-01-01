using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HeartRateHistory.Error
{
    /// <summary>
    /// Exception when reading skin xml file, can not resolve hardware in xml to known type.
    /// </summary>
    public class GenericHardwareNotSupported : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericHardwareNotSupported"/> class.
        /// </summary>
        public GenericHardwareNotSupported()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericHardwareNotSupported"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public GenericHardwareNotSupported(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericHardwareNotSupported"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public GenericHardwareNotSupported(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericHardwareNotSupported"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected GenericHardwareNotSupported(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
