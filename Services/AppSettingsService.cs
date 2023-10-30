using System;
using System.IO;
using System.Text.Json;

namespace Job_Application_Recorder.Services;
public class AppSettingsService
{
    private static readonly string AppDataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JobApplicationRecorder");
    // CurrentDirectory is for dev purposes. Later on, change it to reflect a user's actual app folder per system.
    private static readonly string AppSettingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

    /// <summary>
    /// Loads AppSettings from a JSON file.
    /// </summary>
    /// <returns>Either returns an AppSettings populated by a JSON file, or an empty AppSettings.</returns>
    public static AppSettings LoadAppSettings()
    {
        try
        {
            string settingsJSON = File.ReadAllText(AppSettingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(settingsJSON) ?? new AppSettings();
        }
        catch (FileNotFoundException)
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// Saves updated values to JSON file as needed i.e. updated file path and so on.
    /// </summary>
    /// <param name="appSettings">AppSettings object that will contained updated values.</param>
    public static void SaveAppSettings(AppSettings appSettings)
    {
        string settingsJSON = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AppSettingsFilePath, settingsJSON);
    }

}