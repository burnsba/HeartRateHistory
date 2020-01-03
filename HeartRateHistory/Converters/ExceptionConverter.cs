using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.Converters
{
    /// <summary>
    /// Converts exception.
    /// </summary>
    public static class ExceptionConverter
    {
        /// <summary>
        /// Converts exception to string in the usual format.
        /// </summary>
        /// <param name="ex">Exception to convert.</param>
        /// <returns>Contents of exception and all inner exceptions.</returns>
        public static string DefaultToString(Exception ex)
        {
            var sb = new StringBuilder();

            var currentEx = ex;
            var currentDepth = "Exception";

            do
            {
                sb.AppendLine("Current depth:");
                sb.AppendLine(currentDepth);
                sb.AppendLine();
                sb.AppendLine("Message:");
                sb.AppendLine(currentEx.Message);
                sb.AppendLine();
                sb.AppendLine("Type:");
                sb.AppendLine(currentEx.GetType().FullName);
                sb.AppendLine();
                sb.AppendLine("Stack trace:");
                sb.AppendLine(currentEx.StackTrace);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("==================================================");
                sb.AppendLine();

                currentEx = currentEx.InnerException;
                currentDepth += ".InnerException";
            }
            while (currentEx != null);

            return sb.ToString();
        }
    }
}
