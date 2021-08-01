using System;

namespace SetPropertyControl.SetPropertyControlLogics.Attributes
{
    public partial class SetPropertyUserControl
    {
        /// <summary>
        /// 属性自定义显示控件<para/>
        /// <remarks>优先级低于<seealso cref="CustomControlAttribute"/></remarks>
        /// </summary>
        [AttributeUsage(AttributeTargets.Property,
            Inherited = false,
            AllowMultiple = false)]
        public sealed class CustomControlKeyAttribute : Attribute
        {
            /// <summary>
            /// 在<seealso cref="SetPropertyControlLogics.SetPropertyUserControl"/>.<seealso cref="SetPropertyControlLogics.SetPropertyUserControl.PropertyCustomControlInfos"/>中保存的控件
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// 是否隐藏自动生成的标题
            /// </summary>
            public bool IsHidePropertyTitle { get; }

            public CustomControlKeyAttribute(string key,
                bool isHidePropertyTitle = false)
            {
                Key = key;
                IsHidePropertyTitle = isHidePropertyTitle;
            }
        }
    }
}
