using System.Windows.Input;

namespace PowerBoot.Desktop
{
    public interface IDoAndCancelCommand
    {
        public ICommand CommandDo { get; }
        public ICommand CommandCancel { get; }
    }
}
