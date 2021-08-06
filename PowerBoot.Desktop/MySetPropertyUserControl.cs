using System.Windows;
using System.Windows.Controls;
using SetPropertyControl;
using SetPropertyControl.SetPropertyControlLogics.Options;

namespace PowerBoot.Desktop
{
    public class MySetPropertyUserControl : SetPropertyUserControl
    {
        public MySetPropertyUserControl() : base(new SetPropertyUserControlOptions()
        {
            AutoGenControl = (_, element) =>
            {
                if (element is TextBox box)
                {
                    box.Margin = new Thickness(5);
                }
            }
        })
        {
        }
    }
}
