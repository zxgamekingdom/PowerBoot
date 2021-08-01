using System.Reflection;
using System.Windows;
using SetPropertyControl.SetPropertyControlLogics.Attributes;

namespace PowerBoot.Desktop
{
    public class CustomAttribute : SetPropertyUserControl.CustomControlAttribute
    {
        public override bool IsHidePropertyTitle => true;

        public override FrameworkElement GetCustomControl(PropertyInfo propertyInfo)
        {
            return new DoAndCancelCommandUserControl();
        }
    }
}
