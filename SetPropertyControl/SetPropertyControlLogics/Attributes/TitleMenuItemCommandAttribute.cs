using System;
using System.Reflection;
using System.Windows.Input;

namespace SetPropertyControl.SetPropertyControlLogics.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public abstract class TitleMenuItemCommandAttribute : Attribute
    {
        public abstract string HeardName { get; }

        public abstract ICommand GetCommand(PropertyInfo propertyInfo,
            GetCommandArgs commandArgs);
    }
}