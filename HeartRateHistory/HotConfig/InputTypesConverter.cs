using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory.HotConfig
{
    /// <summary>
    /// Converts <see cref="InputTypes"/>.
    /// </summary>
    public static class InputTypesConverter
    {
        /// <summary>
        /// Converts from <see cref="InputTypes"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="t">Enum to convert.</param>
        /// <returns>String.</returns>
        public static string InputTypeToString(InputTypes t)
        {
            return t.ToString();
        }

        /// <summary>
        /// Converts from <see cref="string"/> to <see cref="InputTypes"/>.
        /// </summary>
        /// <param name="s">String to convert.</param>
        /// <returns>Enum.</returns>
        public static InputTypes StringToInputTypes(string s)
        {
            InputTypes result = InputTypes.Unknown;

            if (Enum.TryParse<InputTypes>(s.ToString(), true, out result))
            {
                return result;
            }

            return InputTypes.Unknown;
        }
    }
}
