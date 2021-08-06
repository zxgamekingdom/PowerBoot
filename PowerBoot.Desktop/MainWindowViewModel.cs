using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using SetPropertyControl.SetPropertyControlLogics.Attributes;
using ShellLink;

namespace PowerBoot.Desktop
{
    public class MainWindowViewModel : ViewModelBase
    {
        public LnkInfoViewModel LnkInfo { get; } = new();
        public RegLocalMachineInfoViewModel RegLocalMachineInfo { get; } = new();
        public RegCurrentUserInfoViewModel RegCurrentUserInfo { get; } = new();

        public class LnkInfoViewModel : ViewModelBase, IDoAndCancelCommand
        {
            public LnkInfoViewModel()
            {
                CommandDo = new ActionCommand(() =>
                {
                    string startupFolder =
                        Environment.GetFolderPath(Environment.SpecialFolder
                            .CommonStartup);
                    switch (Args)
                    {
                        case null:
                            Shortcut.CreateShortcut(ProgramPath)
                                .WriteToFile(Path.Combine(startupFolder,
                                    $"{LnkName}.lnk"));
                            break;
                        default:
                            Shortcut.CreateShortcut(ProgramPath, Args)
                                .WriteToFile(Path.Combine(startupFolder,
                                    $"{LnkName}.lnk"));
                            break;
                    }
                });
                CommandCancel = new ActionCommand(() =>
                {
                    string startupFolder =
                        Environment.GetFolderPath(Environment.SpecialFolder
                            .CommonStartup);
                    string path = Path.Combine(startupFolder, $"{LnkName}.lnk");
                    var fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        fileInfo.IsReadOnly = false;
                        fileInfo.Delete();
                    }
                });
            }

            [PropertyName("快捷方式名")]
            [Order(0)]
            public string? LnkName { get; set; }

            [PropertyName("程序路径")]
            [Order(1)]

            public string? ProgramPath { get; set; }

            [PropertyName("启动参数")]
            [Order(2)]
            public string? Args { get; set; }

            [Custom]
            public ICommand CommandDo { get; }

            [IgnoreAttribute]
            public ICommand CommandCancel { get; }
        }

        public class RegCurrentUserInfoViewModel : ViewModelBase, IDoAndCancelCommand
        {
            [PropertyName("程序路径")]
            [Order(0)]
            public string? ProgramPath { get; set; }

            [PropertyName("注册表名")]
            [Order(1)]
            public string? RegName { get; set; }

            [PropertyName("启动参数")]
            [Order(2)]
            public string? Args { get; set; }

            [Custom]
            public ICommand CommandDo { get; }

            [IgnoreAttribute]
            public ICommand CommandCancel { get; }

            public RegCurrentUserInfoViewModel()
            {
                CommandDo = new ActionCommand(() =>
                {
                    using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                        true)!;
                    object? value = registryKey!.GetValue(RegName);
                    if (value is not null &&
                        MessageBox.Show($@"已存在Key""{RegName}"",值为""{value}"",是否替换!",
                            "信息",
                            MessageBoxButton.YesNo) is MessageBoxResult.No)
                    {
                        return;
                    }

                    registryKey.SetValue(RegName,
                        (Args == null ? ProgramPath : $"{ProgramPath} {Args}") ??
                        throw new InvalidOperationException(),
                        RegistryValueKind.String);
                });
                CommandCancel = new ActionCommand(() =>
                {
                    using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                        true)!;
                    registryKey!.DeleteValue(ProgramPath!, false);
                });
            }
        }

        public class RegLocalMachineInfoViewModel : ViewModelBase, IDoAndCancelCommand
        {
            [PropertyName("程序路径")]
            [Order(0)]
            public string? ProgramPath { get; set; }

            [PropertyName("注册表名")]
            [Order(1)]
            public string? RegName { get; set; }

            [PropertyName("启动参数")]
            [Order(2)]
            public string? Args { get; set; }

            [Custom]
            public ICommand CommandDo { get; }

            [IgnoreAttribute]
            public ICommand CommandCancel { get; }

            public RegLocalMachineInfoViewModel()
            {
                CommandDo = new ActionCommand(() =>
                {
                    using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                        true)!;
                    object? value = registryKey!.GetValue(RegName);
                    if (value is not null &&
                        MessageBox.Show($@"已存在Key""{RegName}"",值为""{value}"",是否替换!",
                            "信息",
                            MessageBoxButton.YesNo) is MessageBoxResult.No)
                    {
                        return;
                    }

                    registryKey.SetValue(RegName,
                        (Args == null ? ProgramPath : $"{ProgramPath} {Args}") ??
                        throw new InvalidOperationException(),
                        RegistryValueKind.String);
                });
                CommandCancel = new ActionCommand(() =>
                {
                    using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                        true)!;
                    registryKey!.DeleteValue(ProgramPath!, false);
                });
            }
        }
    }
}
