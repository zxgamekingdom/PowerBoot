using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace PowerBoot.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public App()
        {
            AllocConsole();
            string[] args = Environment.GetCommandLineArgs();
        }
    }
}
