using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using HeartRateHistory.Error;
using HeartRateHistory.HotConfig.DataSource;

namespace HeartRateHistory
{
    /// <summary>
    /// Core of this library. Instantiates correct hw/ui types from skin xml. Resolves
    /// data sources from skin config json.
    /// </summary>
    public static class TypeResolver
    {

        /// <summary>
        /// List of types of known input handlers.
        /// </summary>
        private static List<Type> _eventSourceTypes = new List<Type>();

        /// <summary>
        /// List of types of known config data providers.
        /// </summary>
        private static List<Type> _configDataProviderTypes = new List<Type>();

        /// <summary>
        /// Whether or not plugins have already been loaded.
        /// </summary>
        private static bool _pluginsLoaded = false;

        /// <summary>
        /// Whether or not data providers have already been loaded.
        /// </summary>
        private static bool _configDataProvidersLoaded = false;

        /// <summary>
        /// Resolves type name and assembly name to a type from the list
        /// of known data provider types.
        /// </summary>
        /// <param name="shortTypeName">Type name without assembly or version.</param>
        /// <param name="assemblyName">Name of hosting assembly.</param>
        /// <returns>Type. First() is called, so this will throw an exception if not found.</returns>
        public static Type GetConfigDataProviderType(string shortTypeName, string assemblyName)
        {
            LoadConfigDataProviders();

            return _configDataProviderTypes
                .Where(x =>
                    x.Assembly.FullName.IndexOf(assemblyName, 0, StringComparison.OrdinalIgnoreCase) >= 0
                    && x.FullName.IndexOf(shortTypeName, 0, StringComparison.OrdinalIgnoreCase) >= 0)
                .First();
        }

        /// <summary>
        /// Creates instance of skin data providor source.
        /// </summary>
        /// <param name="shortTypeName">Type name without assembly or version.</param>
        /// <param name="assemblyName">Name of hosting assembly.</param>
        /// <returns>New instance of data providor.</returns>
        public static IConfigDataProvider CreateConfigDataProviderInstance(string shortTypeName, string assemblyName)
        {
            LoadConfigDataProviders();

            var type = GetConfigDataProviderType(shortTypeName, assemblyName);
            return (IConfigDataProvider)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Loads assemblies from specified directory. Looks for items of type <see cref="IConfigDataProvider"/>.
        /// This can only be performed once.
        /// </summary>
        private static void LoadConfigDataProviders()
        {
            if (_configDataProvidersLoaded)
            {
                return;
            }

            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in types)
            {
                if (typeof(IConfigDataProvider).IsAssignableFrom(type))
                {
                    _configDataProviderTypes.Add(type);
                }
            }

            _configDataProvidersLoaded = true;
        }
    }
}
