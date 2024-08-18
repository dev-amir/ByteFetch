using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System;
using ByteFetch.Config;

namespace ByteFetch;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        ConfigurationServices.Update("Height", "600");
        ConfigurationServices.Update("Width", "600");
        var exePath = Environment.ProcessPath;
        Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true
        });
        Environment.Exit(0);
    }
}