#nullable enable
using SetPropertyControl.SetPropertyControlLogics.Attributes;
using SetPropertyControl.SetPropertyControlLogics.DataInfos;
using SetPropertyControl.SetPropertyControlLogics.Options;
using SetPropertyControl.SetPropertyControlLogics.Options.Args;
using SetPropertyControl.SetPropertyControlLogics.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SetPropertyControl
{
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class SetPropertyUserControl : Control
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header),
                typeof(string),
                typeof(SetPropertyUserControl),
                new PropertyMetadata(default(string)));

        static SetPropertyUserControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SetPropertyUserControl),
                new FrameworkPropertyMetadata(typeof(SetPropertyUserControl)));
        }

        public SetPropertyUserControl()
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += (_, _) => DataContextChanged -= OnDataContextChanged;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public SetPropertyUserControl(SetPropertyUserControlOptions options) : this()
        {
            Options = options;
        }

        public Grid Grid { get; } = new();

        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public SetPropertyUserControlOptions? Options { get; }

        public override void OnApplyTemplate()
        {
            var contentPresenter =
                (ContentPresenter) Template.FindName("PART_ContentPresenter", this);
            contentPresenter.Content = Grid;
            base.OnApplyTemplate();
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Grid.Arrange(new Rect(arrangeBounds));
            return base.ArrangeOverride(arrangeBounds);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Grid.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// 自动生成没有指定 <seealso cref="CustomControlAttribute" /> 的属性的显示控制
        /// </summary>
        /// <param name="propertyInfo"> </param>
        /// <param name="sourceInstanceContext"> </param>
        private (bool isHideTitle, FrameworkElement frameworkElement) AutoGenControl(
            PropertyInfo propertyInfo,
            SourceInstanceContext sourceInstanceContext)
        {
            FrameworkElement contentControl =
                SetPropertyUserControlAutoGenControlTools.GenControl(propertyInfo);
            var args = new AutoGenControlArgs
            {
                PropertyInfo = propertyInfo,
                SourceInstance = sourceInstanceContext.Instance,
                SourceInstanceType = sourceInstanceContext.Type,
            };
            if (sourceInstanceContext.ViewModelControlOptions != null)
                sourceInstanceContext.ViewModelControlOptions.AutoGenControl?.Invoke(
                    args,
                    contentControl);
            else
                Options?.AutoGenControl?.Invoke(args, contentControl);
            return (false, contentControl);
        }

        /// <summary>
        /// 创建用于显示属性的控件
        /// </summary>
        /// <param name="propertyInfo"> </param>
        /// <param name="customControlAttribute"> </param>
        /// <param name="sourceInstanceContext"> </param>
        private (bool isHideTitle, UIElement uiElement) GetControl(
            PropertyInfo propertyInfo,
            CustomControlAttribute? customControlAttribute,
            SourceInstanceContext sourceInstanceContext)
        {
            var args = new GetCustomControlArgs
            {
                PropertyInfo = propertyInfo,
                SourceInstance = sourceInstanceContext.Instance,
                SourceInstanceType = sourceInstanceContext.Type
            };
            return GetCustomControlFromAttribute(customControlAttribute,
                    propertyInfo,
                    sourceInstanceContext) ??
                sourceInstanceContext.ViewModelControlOptions?.GetCustomControl?.Invoke(
                    args) ??
                Options?.GetCustomControl?.Invoke(args) ??
                AutoGenControl(propertyInfo, sourceInstanceContext);
        }

        private (bool isHideTitle, FrameworkElement frameworkElement)?
            GetCustomControlFromAttribute(
                CustomControlAttribute? customControlAttribute,
                PropertyInfo propertyInfo,
                SourceInstanceContext sourceInstanceContext)
        {
            return customControlAttribute is null ?
                null :
                (customControlAttribute.IsHidePropertyTitle,
                    customControlAttribute.GetCustomControl(
                        new CustomControlAttributeArgs()
                        {
                            PropertyInfo = propertyInfo,
                            SourceInstance = sourceInstanceContext.Instance,
                            SourceInstanceType = sourceInstanceContext.Type
                        }) ??
                    throw new NullReferenceException($@"特性""{
                        customControlAttribute.GetType().FullName}""的{
                            nameof(customControlAttribute.GetCustomControl)
                        }()得到的结果为null"));
        }

        private void GetTitleControl(int index,
            PropertyInfo propertyInfo,
            bool isHideTitle,
            SourceInstanceContext sourceInstanceContext)
        {
            if (isHideTitle is false)
            {
                TextBlock titleControl =
                    SetPropertyUserControlAutoGenTitleTools.GenTitleControl(
                        propertyInfo,
                        sourceInstanceContext,
                        Options);
                var args = new GetTitleControlArgs
                {
                    PropertyInfo = propertyInfo,
                    SourceInstance = sourceInstanceContext.Instance,
                    SourceInstanceType = sourceInstanceContext.Type
                };
                if (sourceInstanceContext.ViewModelControlOptions != null)
                    sourceInstanceContext.ViewModelControlOptions.GetTitleControl
                        ?.Invoke(args);
                else
                    Options?.GetTitleControl?.Invoke(args);
                SetPropertyUserControlAutoGenTitleTools.TitleAddMenuItem(propertyInfo,
                    sourceInstanceContext,
                    titleControl,
                    Options);
                Grid.SetColumn(titleControl, 0);
                Grid.SetRow(titleControl, index);
                Grid.Children.Add(titleControl);
            }
        }

        private void InitGrid()
        {
            Grid.Children.Clear();
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
        }

        /// <summary>
        /// 生成并布局 <seealso cref="Grid" /> 的所有子控件
        /// </summary>
        /// <param name="orderPropertyInfos"> </param>
        /// <param name="sourceInstanceContext"> </param>
        private void InitGridChildren(PropertyInfo[] orderPropertyInfos,
            SourceInstanceContext sourceInstanceContext)
        {
            for (var index = 0; index < orderPropertyInfos.Length; index++)
            {
                PropertyInfo propertyInfo = orderPropertyInfos[index];
                var args = new GetCustomControlAttributeArgs
                {
                    PropertyInfo = propertyInfo,
                    SourceInstance = sourceInstanceContext.Instance,
                    SourceInstanceType = sourceInstanceContext.Type,
                };
                var customControlAttribute =
                    propertyInfo.GetCustomAttribute<CustomControlAttribute>() ??
                    sourceInstanceContext.ViewModelControlOptions
                        ?.GetCustomControlAttribute?.Invoke(args) ??
                    Options?.GetCustomControlAttribute?.Invoke(args);
                (bool isHideTitle, var uiElement) = GetControl(propertyInfo,
                    customControlAttribute,
                    sourceInstanceContext);
                switch (isHideTitle)
                {
                    case true:
                        Grid.SetColumn(uiElement, 0);
                        Grid.SetColumnSpan(uiElement, 2);
                        break;
                    case false:
                        Grid.SetColumn(uiElement, 1);
                        break;
                }

                Grid.SetRow(uiElement, index);
                Grid.Children.Add(uiElement);
                GetTitleControl(index,
                    propertyInfo,
                    isHideTitle,
                    sourceInstanceContext);
            }
        }

        /// <summary>
        /// 初始化 <seealso cref="Grid" /> 的布局
        /// </summary>
        /// <param name="valueTuples"> </param>
        private void InitGridLayout(PropertyInfo[] valueTuples)
        {
            Grid.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});
            Grid.ColumnDefinitions.Add(new ColumnDefinition());
            int count = valueTuples.Length;
            for (int i = 0; i < count; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void OnDataContextChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            object value = e.NewValue;
            Type type = value.GetType();
            var instanceContextInfo = new SourceInstanceContext
            {
                Instance = value, Type = type
            };
            if (value is IGetSetPropertyUserControlOptions o)
                instanceContextInfo.ViewModelControlOptions = o.GetOptions();
            InitGrid();
            var selectPropertyInfos = SelectPropertyInfos(type, instanceContextInfo);
            PropertyInfo[] orderPropertyInfos =
                OrderPropertyInfos(selectPropertyInfos, instanceContextInfo);
            InitGridLayout(selectPropertyInfos);
            InitGridChildren(orderPropertyInfos, instanceContextInfo);
        }

        /// <summary>
        /// 将ViewModel对象的属性进行排序 <remarks> 首先将有顺序特性的属性插入到没有顺序特性的属性集合中 </remarks>
        /// </summary>
        /// <param name="propertyInfos"> </param>
        /// <param name="sourceInstanceContext"> </param>
        private PropertyInfo[] OrderPropertyInfos(PropertyInfo[] propertyInfos,
            SourceInstanceContext sourceInstanceContext)
        {
            (PropertyInfo propertyInfo, OrderAttribute orderAttribute)[] array =
            (
                from propertyInfo in propertyInfos
                let args =
                    new GetOrderAttributeArgs
                    {
                        PropertyInfo = propertyInfo,
                        SourceInstance = sourceInstanceContext.Instance,
                        SourceInstanceType = sourceInstanceContext.Type
                    }
                let orderAttribute =
                    propertyInfo.GetCustomAttribute<OrderAttribute>() ??
                    sourceInstanceContext.ViewModelControlOptions?.GetOrderAttribute
                        ?.Invoke(args) ??
                    Options?.GetOrderAttribute?.Invoke(args)
                select (propertyInfo, orderAttribute)).ToArray();
            List<PropertyInfo> noOrderList = (
                from item in array
                where item.orderAttribute is null
                select item.propertyInfo).ToList();
            (PropertyInfo propertyInfo, int order)[] orderArray =
            (
                from item in array
                where item.orderAttribute is not null
                let order = item.orderAttribute.Order
                select (item.propertyInfo, order)).ToArray();
            foreach (var item in orderArray)
            {
                int i = item.order;
                if (i < 0)
                    throw new ArgumentException(
                        $"属性 {item.propertyInfo.Name} 序号不能小于零,当前的序号为{i}");
                if (i < noOrderList.Count)
                    noOrderList.Insert(i, item.propertyInfo);
                else
                    noOrderList.Add(item.propertyInfo);
            }

            return noOrderList.ToArray();
        }

        private PropertyInfo[] SelectPropertyInfos(Type type,
            SourceInstanceContext sourceInstanceContext)
        {
            PropertyInfo[] propertyInfos =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return propertyInfos.Where(info =>
                {
                    var args = new GetIgnoreAttributeArgs
                    {
                        PropertyInfo = info,
                        SourceInstance = sourceInstanceContext.Instance,
                        SourceInstanceType = sourceInstanceContext.Type
                    };
                    IgnoreAttribute? attribute =
                        info.GetCustomAttribute<IgnoreAttribute>() ??
                        sourceInstanceContext.ViewModelControlOptions
                            ?.GetIgnoreAttribute
                            ?.Invoke(args) ??
                        Options?.GetIgnoreAttribute?.Invoke(args);
                    return attribute is null;
                })
                .ToArray();
        }
    }
}
