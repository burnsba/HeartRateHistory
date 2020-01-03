using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using HeartRateHistory.HotConfig;
using HeartRateHistory.ViewModels;

namespace HeartRateHistory.DataTemplateSelectors
{
    /// <summary>
    /// Xaml data template selector for skin config items.
    /// </summary>
    public class ConfigSettingTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Resolves object based on InputType property.
        /// </summary>
        /// <param name="obj">Object to resolve.</param>
        /// <param name="container">Container.</param>
        /// <returns>Data template from xaml.</returns>
        public override DataTemplate SelectTemplate(object obj, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && obj != null && obj is IConfigSetting)
            {
                var item = obj as IConfigSetting;

                if (item.InputType == InputTypes.Textbox)
                {
                    return element.FindResource("ConfigTextboxDataTemplate") as DataTemplate;
                }
                else if (item.InputType == InputTypes.Dropdown)
                {
                    return element.FindResource("ConfigComboDataTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
