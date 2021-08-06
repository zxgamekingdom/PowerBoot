using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SetPropertyControl.SetPropertyControlLogics.Tools
{
    internal static class SetPropertyUserControlAutoGenControlTools
    {
        internal static FrameworkElement GenControl(PropertyInfo propertyInfo)
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

        private static FrameworkElement GenBoolControl(PropertyInfo propertyInfo)
        {
            var contentControl = new CheckBox
            {
                VerticalContentAlignment = VerticalAlignment.Center
            };
            (bool get, bool set) rwAuth = GetPropertyRwAuth(propertyInfo);
            contentControl.SetBinding(ToggleButton.IsCheckedProperty,
                GetBinding(propertyInfo, rwAuth));
            if (rwAuth.set is false) contentControl.IsEnabled = false;
            return contentControl;
        }
        private static FrameworkElement GenOtherControl(PropertyInfo propertyInfo)
        {
            var contentControl = new TextBox
            {
                VerticalContentAlignment = VerticalAlignment.Center
            };
            (bool get, bool set) rwAuth = GetPropertyRwAuth(propertyInfo);
            contentControl.SetBinding(TextBox.TextProperty,
                GetBinding(propertyInfo, rwAuth));
            if (rwAuth.set is false)
            {
                contentControl.IsReadOnly = true;
            }

            return contentControl;
        }

        /// <summary>
        /// 通过属性的读写权限获取绑定
        /// </summary>
        /// <param name="propertyInfo"> </param>
        /// <param name="tuple"> </param>
        private static Binding GetBinding(PropertyInfo propertyInfo,
            (bool get, bool set) tuple)
        {
            return new(propertyInfo.Name)
            {
                Mode = tuple switch
                {
                    (true, true) => BindingMode.TwoWay,
                    (true, false) => BindingMode.OneWay,
                    (false, true) => BindingMode.OneWayToSource,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };
        }
        /// <summary>
        /// 获取属性的读写是否是公开的
        /// </summary>
        /// <param name="propertyInfo"> </param>
        private static (bool get, bool set) GetPropertyRwAuth(PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetMethod?.IsPublic ?? false,
                propertyInfo.SetMethod?.IsPublic ?? false);
        }
    }
}