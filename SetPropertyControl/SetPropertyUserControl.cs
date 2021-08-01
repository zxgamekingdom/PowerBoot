#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using SetPropertyControl.SetPropertyControlLogics;
using SetPropertyControl.SetPropertyControlLogics.Options;

namespace SetPropertyControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SetPropertyControl"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SetPropertyControl;assembly=SetPropertyControl"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:SetPropertyUserControl/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class SetPropertyUserControl : Control
    {
        static SetPropertyUserControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SetPropertyUserControl),
                new FrameworkPropertyMetadata(typeof(SetPropertyUserControl)));
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header),
                typeof(string),
                typeof(SetPropertyUserControl),
                new PropertyMetadata(default(string)));

        public override void OnApplyTemplate()
        {
            var contentPresenter =
                (ContentPresenter) Template.FindName("PART_ContentPresenter", this);
            contentPresenter.Content = Grid;
            base.OnApplyTemplate();
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

        public List<PropertyCustomControlInfo> PropertyCustomControlInfos { get; } =
            new();

        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
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

        public Grid Grid { get; } = new();

        public SetPropertyUserControlOptions? Options { get; set; }

        /// <summary>
        /// 将ViewModel对象的属性进行排序
        /// <remarks>
        ///首先将有顺序特性的属性插入到没有顺序特性的属性集合中
        /// </remarks>
        /// </summary>
        /// <param name="propertyInfos"></param>
        private PropertyInfo[] OrderPropertyInfos(PropertyInfo[] propertyInfos)
        {
            (PropertyInfo propertyInfo,
                SetPropertyControlLogics.Attributes.SetPropertyUserControl.
                OrderAttribute orderAttribute)[] array = (
                    from propertyInfo in propertyInfos
                    let orderAttribute =
                        propertyInfo
                            .GetCustomAttribute<SetPropertyControlLogics.Attributes.
                                SetPropertyUserControl.OrderAttribute>() ??
                        Options?.GetOrderAttribute?.Invoke(propertyInfo)
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

        /// <summary>
        /// 自动生成没有指定<seealso cref="SetPropertyControlLogics.Attributes.SetPropertyUserControl.CustomControlKeyAttribute"/>,<seealso cref="SetPropertyControlLogics.Attributes.SetPropertyUserControl.CustomControlAttribute"/>的属性的显示控制
        /// </summary>
        /// <param name="propertyInfo"></param>
        private (bool isHideTitle, FrameworkElement frameworkElement) AutoGenControl(
            PropertyInfo propertyInfo)
        {
            FrameworkElement contentControl =
                SetPropertyUserControlAutoGenControlTools.GenControl(propertyInfo);
            Options?.AutoGenControlAction?.Invoke(propertyInfo, contentControl);
            return (false, contentControl);
        }

        /// <summary>
        /// 创建用于显示属性的控件
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="customControlAttribute"></param>
        /// <param name="customControlKeyAttribute"></param>
        private (bool isHideTitle, UIElement uiElement) GetControl(
            PropertyInfo propertyInfo,
            SetPropertyControlLogics.Attributes.SetPropertyUserControl.
                CustomControlAttribute? customControlAttribute,
            SetPropertyControlLogics.Attributes.SetPropertyUserControl.
                CustomControlKeyAttribute? customControlKeyAttribute)
        {
            return
                GetCustomControlFromAttribute(customControlAttribute, propertyInfo) ??
                GetCustomControlFromKey(customControlKeyAttribute) ??
                Options?.GetCustomControl?.Invoke(propertyInfo) ??
                AutoGenControl(propertyInfo);
        }

        private (bool isHideTitle, FrameworkElement frameworkElement)?
            GetCustomControlFromAttribute(
                SetPropertyControlLogics.Attributes.SetPropertyUserControl.
                    CustomControlAttribute? customControlAttribute,
                PropertyInfo propertyInfo)
        {
            return customControlAttribute is null ?
                null :
                (customControlAttribute.IsHidePropertyTitle,
                    customControlAttribute.GetCustomControl(propertyInfo) ??
                    throw new NullReferenceException($@"特性""{
                        customControlAttribute.GetType().FullName}""的{
                            nameof(customControlAttribute.GetCustomControl)
                        }()得到的结果为null"));
        }

        private (bool isHideTitle, FrameworkElement frameworkElement)?
            GetCustomControlFromKey(
                SetPropertyControlLogics.Attributes.SetPropertyUserControl.
                    CustomControlKeyAttribute? customControlKeyAttribute)
        {
            return customControlKeyAttribute is null ?
                null :
                (customControlKeyAttribute.IsHidePropertyTitle,
                    PropertyCustomControlInfos.SingleOrDefault(info =>
                            info.Key == customControlKeyAttribute.Key)
                        ?.Content ??
                    throw new NotPropertyCustomControlKeyException($@"找不到Key为""{
                        customControlKeyAttribute.Key}""的自定义属性显示控件"));
        }

        /// <summary>
        /// 初始化<seealso cref="_grid"/>的布局
        /// </summary>
        /// <param name="valueTuples"></param>
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
            InitGrid();
            var selectPropertyInfos = SelectPropertyInfos(type);
            PropertyInfo[] orderPropertyInfos = OrderPropertyInfos(selectPropertyInfos);
            InitGridLayout(selectPropertyInfos);
            InitGridChildren(orderPropertyInfos);
        }

        private void InitGrid()
        {
            Grid.Children.Clear();
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
        }

        private PropertyInfo[] SelectPropertyInfos(Type type)
        {
            PropertyInfo[] propertyInfos =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return propertyInfos.Where(info =>
                {
                    SetPropertyControlLogics.Attributes.SetPropertyUserControl.
                        IgnoreAttribute? attribute =
                            info.GetCustomAttribute<SetPropertyControlLogics.Attributes.
                                SetPropertyUserControl.IgnoreAttribute>() ??
                            Options?.GetIgnoreAttribute?.Invoke(info);
                    return attribute is null;
                })
                .ToArray();
        }

        /// <summary>
        /// 生成并布局<seealso cref="_grid"/>的所有子控件
        /// </summary>
        /// <param name="orderPropertyInfos"></param>
        private void InitGridChildren(PropertyInfo[] orderPropertyInfos)
        {
            for (var index = 0; index < orderPropertyInfos.Length; index++)
            {
                PropertyInfo propertyInfo = orderPropertyInfos[index];
                var customControlAttribute =
                    propertyInfo
                        .GetCustomAttribute<SetPropertyControlLogics.Attributes.
                            SetPropertyUserControl.CustomControlAttribute>() ??
                    Options?.GetCustomControlAttribute?.Invoke(propertyInfo);
                var customControlKeyAttribute = propertyInfo
                        .GetCustomAttribute<SetPropertyControlLogics.Attributes.
                            SetPropertyUserControl.CustomControlKeyAttribute>() ??
                    Options?.GetCustomControlKeyAttribute?.Invoke(propertyInfo);
                (bool isHideTitle, var uiElement) = GetControl(propertyInfo,
                    customControlAttribute,
                    customControlKeyAttribute);
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
                if (isHideTitle is false)
                {
                    var titleControl = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = propertyInfo
                                .GetCustomAttribute<
                                    SetPropertyControlLogics.Attributes.
                                    SetPropertyUserControl.PropertyNameAttribute>()
                                ?.PropertyName ??
                            Options?.GetPropertyNameAttribute?.Invoke(propertyInfo)
                                ?.PropertyName ??
                            propertyInfo.Name
                    };
                    Grid.SetColumn(titleControl, 0);
                    Grid.SetRow(titleControl, index);
                    Grid.Children.Add(titleControl);
                }
            }
        }
    }
}
