#nullable enable
using System.Windows;
using System.Windows.Markup;

namespace SetPropertyControl.SetPropertyControlLogics
{
    [ContentProperty(nameof(Content))]
    public class PropertyCustomControlInfo
    {
        public string Key { get; set; } = null!;
        public FrameworkElement? Content { get; set; }
    }
}
