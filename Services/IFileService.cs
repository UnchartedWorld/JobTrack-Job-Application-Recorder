using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Job_Application_Recorder.Services;

public interface IFileService
{
    public Task<IStorageFile?> OpenFileAsync();
    public Task<IStorageFile?> SaveFileAsync();
}