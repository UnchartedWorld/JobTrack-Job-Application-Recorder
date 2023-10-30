using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

public static class FileService
{
    /// <summary>
    /// Stores a file in a desired location based on passed data.
    /// </summary>
    /// <param name="createFile">Represents the created file.</param>
    /// <param name="data">Represents the data to be stored in the file.</param>
    /// <returns></returns>
    public static async Task CreateToJSONFileAsync(IStorageFile createFile, object data)
    {
        JsonSerializerOptions serializerOptions = new()
        {
            WriteIndented = true
        };

        string fileContents = JsonSerializer.Serialize(data, serializerOptions);

        using Stream? writeStream = await createFile.OpenWriteAsync();
        using StreamWriter streamWriter = new(writeStream);

        await streamWriter.WriteAsync(fileContents);
    }

    public static async Task SaveToJSONFileAsync(IStorageFile saveFile, object data)
    {
        JsonSerializerOptions serializerOptions = new()
        {
            WriteIndented = true
        };
    }
}