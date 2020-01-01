using System;
using System.Collections.Generic;
using System.Text;

namespace HeartRateHistory
{
    /// <summary>
    /// Application context.
    /// </summary>
    public static class Workspace
    {
        /// <summary>
        /// Shows a window of the given type. If a window with the same type already exists,
        /// it will be shown. If no window with the given type exists a new one will
        /// be created and shown.
        /// </summary>
        /// <typeparam name="T">Type of window to create.</typeparam>
        /// <param name="args">Window constructor arguments.</param>
        public static void CreateSingletonWindow<T>(params object[] args)
        {
            if (!typeof(System.Windows.Window).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException($"{typeof(T).FullName} must be System.Windows.Window");
            }

            foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            {
                if (w.GetType() == typeof(T))
                {
                    w.Show();
                    w.Activate();
                    return;
                }
            }

            var window = (System.Windows.Window)Activator.CreateInstance(typeof(T), args);
            window.Show();
        }

        /// <summary>
        /// Shows a window of the given type. If a window with the same type already exists,
        /// it will be closed and recreated. If no window with the given type exists a new one will
        /// be created and shown.
        /// </summary>
        /// <typeparam name="T">Type of window to create.</typeparam>
        /// <param name="args">Window constructor arguments.</param>
        public static void RecreateSingletonWindow<T>(params object[] args)
        {
            if (!typeof(System.Windows.Window).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException($"{typeof(T).FullName} must be System.Windows.Window");
            }

            foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            {
                if (w.GetType() == typeof(T))
                {
                    w.Close();
                    break;
                }
            }

            var window = (System.Windows.Window)Activator.CreateInstance(typeof(T), args);
            window.Show();
        }
    }
}
