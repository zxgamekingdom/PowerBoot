#nullable enable
using SetPropertyControl.SetPropertyControlLogics.Attributes;
using SetPropertyControl.SetPropertyControlLogics.Options.Args;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SetPropertyControl.SetPropertyControlLogics.Options
{
    public class SetPropertyUserControlOptions
    {
        public Action<AutoGenControlArgs, FrameworkElement>? AutoGenControl;

        public Func<GetCustomControlArgs, (bool isHideTitle, UIElement uiElement)?>?
            GetCustomControl;

        public Func<GetCustomControlAttributeArgs, CustomControlAttribute?>?
            GetCustomControlAttribute;

        public Func<GetIgnoreAttributeArgs, IgnoreAttribute?>? GetIgnoreAttribute;
        public Func<GetOrderAttributeArgs, OrderAttribute?>? GetOrderAttribute;

        public Func<GetPropertyNameAttributeArgs, PropertyNameAttribute?>?
            GetPropertyNameAttribute;

        public Func<GetTitleControlArgs, TextBlock>? GetTitleControl;

        public Func<GetTitleMenuItemAttributeArgs, IEnumerable<TitleMenuItemAttribute>>?
            GetTitleMenuItemAttribute;

        public Func<GetTitleMenuItemCommandAttributeArgs,
               IEnumerable<TitleMenuItemCommandAttribute>>?
            GetTitleMenuItemCommandAttribute;
    }
}