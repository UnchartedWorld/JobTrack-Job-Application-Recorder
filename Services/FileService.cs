using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Job_Application_Recorder.Services;

public class FileService : IFileService
{
    private readonly Window targetWindow;

    public FileService(Window target)
    {
        targetWindow = target;
    }

    public async Task<IStorageFile?> OpenFileAsync()
    {
        var file = await targetWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Job Applications JSON file",
            AllowMultiple = false,
            FileTypeFilter = new FilePickerFileType[]
            {
                new("CSV File")
                {
                    Patterns = new[] {"*.json"},
                    AppleUniformTypeIdentifiers = new[] {"public.json"},
                    MimeTypes = new[] { "application/json" }
                }
            }
        });

        Console.WriteLine("File name: " + file[0].Name);
        return file[0];
    }

    public async Task<IStorageFile?> SaveFileAsync()
    {
        return await targetWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save Job Application as JSON",
            FileTypeChoices = new FilePickerFileType[]
            {
                new("CSV File")
                {
                    Patterns = new[] {"*.json"},
                    AppleUniformTypeIdentifiers = new[] {"public.json"},
                    MimeTypes = new[] { "application/json" }
                }
            }
        });
    }
}