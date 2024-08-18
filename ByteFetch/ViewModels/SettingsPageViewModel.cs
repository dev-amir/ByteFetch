using Avalonia;
using Avalonia.Styling;
using ByteFetch.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ByteFetch.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private const string _dark = "Dark";
    private const string _light = "Light";
    public string[] AppThemes { get; } = [_dark, _light];
    [ObservableProperty]
    private string _currentAppTheme;
    public SettingsPageViewModel()
    {
        CurrentAppTheme = ConfigurationServices.Get("Theme");
    }

    partial void OnCurrentAppThemeChanged(string value)
    {
        var newTheme = GetThemeVariant(value);
        Application.Current.RequestedThemeVariant = newTheme;
        ConfigurationServices.Update("Theme", CurrentAppTheme);
    }

    private static ThemeVariant? GetThemeVariant(string value)
        => value switch
        {
            _light => ThemeVariant.Light,
            _dark => ThemeVariant.Dark,
            _ => null
        };
}
