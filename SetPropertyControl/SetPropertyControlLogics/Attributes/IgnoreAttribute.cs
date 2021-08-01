#nullable enable
using System;

namespace SetPropertyControl.SetPropertyControlLogics.Attributes
{
    public partial class SetPropertyUserControl
    {
        /// <summary>
        /// 是否忽略属性的显示
        /// </summary>
        [AttributeUsage(AttributeTargets.Property,
            Inherited = false,
            AllowMultiple = false)]
        public sealed class IgnoreAttribute : Attribute
        {
        }
    }
}
