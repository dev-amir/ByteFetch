using Avalonia.Controls;
using Avalonia.Threading;
using ByteFetch.Config;
using FluentAvalonia.UI.Controls;
using System;
using System.Timers;

namespace ByteFetch.Views;

public partial class MainWindow : Window
{
    private Timer _resizeWindowDebounceTimer;
    private const double _resizeWindowDebounceInterval = 400;
    public MainWindow()
    {
        InitializeComponent();
        this.Height = double.Parse(ConfigurationServices.Get("Height"));
        this.Width = double.Parse(ConfigurationServices.Get("Width"));

        // Resize Window Debounce
        _resizeWindowDebounceTimer = new Timer
        {
            Interval = _resizeWindowDebounceInterval,
            AutoReset = false
        };
        _resizeWindowDebounceTimer.Elapsed += OnResizeWindowsDebounceTimerElapsed;
        this.SizeChanged += OnSizeChanged;

        NavView.SelectionChanged += OnNavigationSelectionChanged;
        NavView.SelectedItem = NavView.MenuItems[0];
        ContentFrame.Navigate(typeof(DownloadPage));
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        _resizeWindowDebounceTimer.Stop();
        _resizeWindowDebounceTimer.Start();
    }

    private void OnResizeWindowsDebounceTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ConfigurationServices.Update("Height", this.Height.ToString());
            ConfigurationServices.Update("Width", this.Width.ToString());
        });
    }

    private void OnNavigationSelectionChanged(object sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem selelctedItem)
        {
            string selectedOption = selelctedItem.Content.ToString();

            switch (selectedOption)
            {
                case "Download":
                    ContentFrame.Navigate(typeof(DownloadPage));
                    break;
                case "Settings":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    break;
            }
        }
    }
}