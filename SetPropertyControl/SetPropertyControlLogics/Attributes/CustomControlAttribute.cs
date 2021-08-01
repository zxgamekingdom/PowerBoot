#nullable enable
using System;
using System.Reflection;
using System.Windows;

namespace SetPropertyControl.SetPropertyControlLogics.Attributes
{
    public partial class SetPropertyUserControl
    {
        /// <summary>
        /// 属性自定义显示控件<para/>
        /// <remarks>优先级高于<seealso cref="CustomControlKeyAttribute"/></remarks>
        /// </summary>
        [AttributeUsage(AttributeTargets.Property,
            Inherited = false,
            AllowMultiple = false)]
        public abstract class CustomControlAttribute : Attribute
        {
            /// <summary>
            /// 是否隐藏自动生成的标题
            /// </summary>
            public abstract bool IsHidePropertyTitle { get; }

            /// <summary>
            /// 控件
            /// </summary>
            public abstract FrameworkElement? GetCustomControl(
                PropertyInfo propertyInfo);
        }
    }
}
