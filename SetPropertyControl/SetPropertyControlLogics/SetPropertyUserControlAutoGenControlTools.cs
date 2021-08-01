using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SetPropertyControl.SetPropertyControlLogics
{
    internal static class SetPropertyUserControlAutoGenControlTools
    {
        public static FrameworkElement GenControl(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(bool))
            {
                return GenBoolControl(propertyInfo);
            }
            else
            {
                return GenOtherControl(propertyInfo);
            }
        }

        private static FrameworkElement GenOtherControl(PropertyInfo propertyInfo)
        {
            var contentControl = new TextBox
            {
                VerticalContentAlignment = VerticalAlignment.Center
            };
            contentControl.SetBinding(TextBox.TextProperty,
                new Binding
                {
                    Path = new PropertyPath(propertyInfo.Name),
                    Mode = BindingMode.TwoWay,
                });
            return contentControl;
        }

        private static FrameworkElement GenBoolControl(PropertyInfo propertyInfo)
        {
            var contentControl = new CheckBox
            {
                VerticalContentAlignment = VerticalAlignment.Center
            };
            contentControl.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding
                {
                    Path = new PropertyPath(propertyInfo.Name),
                    Mode = BindingMode.TwoWay,
                });
            return contentControl;
        }
    }
}
