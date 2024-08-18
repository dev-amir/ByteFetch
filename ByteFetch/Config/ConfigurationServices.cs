using System.Configuration;

namespace ByteFetch.Config;

public class ConfigurationServices
{
    public static string Get(string key)
        => ConfigurationManager.AppSettings[key];

    public static void Update(string key, string value)
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        if (config.AppSettings.Settings[key] != null)
            config.AppSettings.Settings[key].Value = value;
        else
            config.AppSettings.Settings.Add(key, value);
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }
}
