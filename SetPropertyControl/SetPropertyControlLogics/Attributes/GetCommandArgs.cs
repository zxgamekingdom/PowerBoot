#nullable enable
using System;
using System.Reflection;

namespace SetPropertyControl.SetPropertyControlLogics.Attributes
{
    public class GetCommandArgs
    {
        public object Instance { get; set; } = null!;
        public PropertyInfo PropertyInfo { get; set; } = null!;
        public Type Type { get; set; } = null!;
    }
}