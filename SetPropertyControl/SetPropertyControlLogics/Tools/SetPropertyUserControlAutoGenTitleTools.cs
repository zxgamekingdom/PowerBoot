#nullable enable
using SetPropertyControl.SetPropertyControlLogics.Attributes;
using SetPropertyControl.SetPropertyControlLogics.DataInfos;
using SetPropertyControl.SetPropertyControlLogics.Options;
using SetPropertyControl.SetPropertyControlLogics.Options.Args;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SetPropertyControl.SetPropertyControlLogics.Tools
{
    internal static class SetPropertyUserControlAutoGenTitleTools
    {
        internal static TextBlock GenTitleControl(PropertyInfo propertyInfo,
            SourceInstanceContext sourceInstanceContext,
            SetPropertyUserControlOptions? options)
        {
            var args = new GetPropertyNameAttributeArgs
            {
                PropertyInfo = propertyInfo,
                SourceInstance = sourceInstanceContext.Instance,
                SourceInstanceType = sourceInstanceContext.Type
            };
            return new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text =
                    propertyInfo.GetCustomAttribute<PropertyNameAttribute>()
                        ?.PropertyName ??
                    sourceInstanceContext.ViewModelControlOptions
                        ?.GetPropertyNameAttribute?.Invoke(args)
                        ?.PropertyName ??
                    options?.GetPropertyNameAttribute?.Invoke(args)?.PropertyName ??
                    propertyInfo.Name
            };
        }
        internal static void TitleAddMenuItem(PropertyInfo propertyInfo,
                     SourceInstanceContext sourceInstanceContext,
            TextBlock titleControl,
            SetPropertyUserControlOptions? controlOptions)
        {
            ApplyTitleMenuItemAttribute(propertyInfo,
                sourceInstanceContext,
                titleControl,
                controlOptions);
            ApplyTitleMenuItemCommandAttribute(propertyInfo,
                sourceInstanceContext,
                titleControl,
                controlOptions);
        }

        private static void ApplyTitleMenuItemAttribute(PropertyInfo propertyInfo,
            SourceInstanceContext sourceInstanceContext,
            TextBlock titleControl,
            SetPropertyUserControlOptions? controlOptions)
        {
            var titleMenuItemAttributes = new List<TitleMenuItemAttribute>();
            titleMenuItemAttributes.AddRange(propertyInfo
                .GetCustomAttributes<TitleMenuItemAttribute>());
            var args = new GetTitleMenuItemAttributeArgs()
            {
                PropertyInfo = propertyInfo,
                SourceInstance = sourceInstanceContext.Instance,
                SourceInstanceType = sourceInstanceContext.Type
            };
            titleMenuItemAttributes.AddRange(
                sourceInstanceContext.ViewModelControlOptions?.GetTitleMenuItemAttribute
                    ?.Invoke(args) ??
                Enumerable.Empty<TitleMenuItemAttribute>());
            titleMenuItemAttributes.AddRange(
                controlOptions?.GetTitleMenuItemAttribute?.Invoke(args) ??
                Enumerable.Empty<TitleMenuItemAttribute>());
            foreach (TitleMenuItemAttribute menuItemAttribute in
                titleMenuItemAttributes)
            {
                titleControl.ContextMenu ??= new ContextMenu();
                var menuItem = new MenuItem { Header = menuItemAttribute.HeardName };
                menuItem.SetBinding(MenuItem.CommandProperty,
                    new Binding(menuItemAttribute.CommandName));
                titleControl.ContextMenu.Items.Add(menuItem);
            }
        }
        private static void ApplyTitleMenuItemCommandAttribute(
                    PropertyInfo propertyInfo,
            SourceInstanceContext sourceInstanceContext,
            TextBlock titleControl,
            SetPropertyUserControlOptions? controlOptions)
        {
            var titleMenuItemCommandAttributes =
                new List<TitleMenuItemCommandAttribute>();
            titleMenuItemCommandAttributes.AddRange(propertyInfo
                .GetCustomAttributes<TitleMenuItemCommandAttribute>());
            var args = new GetTitleMenuItemCommandAttributeArgs()
            {
                PropertyInfo = propertyInfo,
                SourceInstance = sourceInstanceContext.Instance,
                SourceInstanceType = sourceInstanceContext.Type
            };
            titleMenuItemCommandAttributes.AddRange(
                sourceInstanceContext.ViewModelControlOptions
                    ?.GetTitleMenuItemCommandAttribute?.Invoke(args) ??
                Enumerable.Empty<TitleMenuItemCommandAttribute>());
            titleMenuItemCommandAttributes.AddRange(
                controlOptions?.GetTitleMenuItemCommandAttribute?.Invoke(args) ??
                Enumerable.Empty<TitleMenuItemCommandAttribute>());
            foreach (TitleMenuItemCommandAttribute titleMenuItemCommandAttribute in
                titleMenuItemCommandAttributes)
            {
                titleControl.ContextMenu ??= new ContextMenu();
                var menuItem = new MenuItem
                {
                    Header = titleMenuItemCommandAttribute.HeardName,
                    Command = titleMenuItemCommandAttribute.GetCommand(propertyInfo,
                        new GetCommandArgs
                        {
                            Instance = sourceInstanceContext.Instance,
                            PropertyInfo = propertyInfo,
                            Type = sourceInstanceContext.Type
                        })
                };
                titleControl.ContextMenu.Items.Add(menuItem);
            }
        }
    }
}