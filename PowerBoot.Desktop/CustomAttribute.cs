using System.Windows;
using SetPropertyControl.SetPropertyControlLogics.Attributes;
using SetPropertyControl.SetPropertyControlLogics.Options.Args;

namespace PowerBoot.Desktop
{
    public class CustomAttribute : CustomControlAttribute
    {
        public override bool IsHidePropertyTitle => true;

        public override FrameworkElement? GetCustomControl(
            CustomControlAttributeArgs args)
        {
            return new DoAndCancelCommandUserControl();
        }
    }
}
