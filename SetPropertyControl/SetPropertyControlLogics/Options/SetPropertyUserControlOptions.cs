#nullable enable
using System;
using System.Reflection;
using System.Windows;

namespace SetPropertyControl.SetPropertyControlLogics.Options
{
    public class SetPropertyUserControlOptions
    {
        public Action<PropertyInfo, FrameworkElement>? AutoGenControlAction;

        public Func<PropertyInfo, (bool isHideTitle, UIElement uiElement)?>?
            GetCustomControl;

        public Func<PropertyInfo,
                Attributes.SetPropertyUserControl.CustomControlAttribute?>?
            GetCustomControlAttribute;

        public Func<PropertyInfo,
                Attributes.SetPropertyUserControl.CustomControlKeyAttribute?>?
            GetCustomControlKeyAttribute;

        public Func<PropertyInfo, Attributes.SetPropertyUserControl.IgnoreAttribute?>?
            GetIgnoreAttribute;

        public Func<PropertyInfo, Attributes.SetPropertyUserControl.OrderAttribute?>?
            GetOrderAttribute;

        public Func<PropertyInfo, Attributes.SetPropertyUserControl.PropertyNameAttribute?>?
            GetPropertyNameAttribute;
    }
}
