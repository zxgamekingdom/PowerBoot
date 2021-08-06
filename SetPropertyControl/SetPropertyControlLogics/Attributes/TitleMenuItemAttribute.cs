using System;

namespace SetPropertyControl.SetPropertyControlLogics.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class TitleMenuItemAttribute : Attribute
    {
        public TitleMenuItemAttribute(string heardName, string commandName)
        {
            HeardName = heardName;
            CommandName = commandName;
        }
        public string CommandName { get; }
        public string HeardName { get; }
    }
}