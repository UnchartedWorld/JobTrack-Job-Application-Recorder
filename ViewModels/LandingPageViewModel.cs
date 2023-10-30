using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Job_Application_Recorder.Services;
using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public class LandingPageViewModel : ViewModelBase
{
    private IEnumerable<string>? _selectedJSONFile;
    private IEnumerable<string>? _selectedFileLocation;
    public ICommand SelectFileCommand { get; } 
    public ICommand SaveFileCommand { get; }

    /// <summary>
    /// Gets or sets a singular file.
    /// </summary>
    public IEnumerable<string>? SelectedJSONFile
    {
        get => _selectedJSONFile;
        set => this.RaiseAndSetIfChanged(ref _selectedJSONFile, value);
    }

    public IEnumerable<string>? SelectedFileLocation
    {
        get => _selectedFileLocation;
        set => this.RaiseAndSetIfChanged(ref _selectedFileLocation, value);
    }

    public LandingPageViewModel()
    {
        SelectFileCommand = ReactiveCommand.CreateFromTask(SelectFileAsync);
        SaveFileCommand = ReactiveCommand.CreateFromTask(CreateNewFileAsync);
    }

    public async Task SelectFileAsync()
    {
        SelectedJSONFile = await this.OpenFileDialogAsync("Open JSON File…");
    }

    public async Task CreateNewFileAsync()
    {
        SelectedFileLocation = await this.CreateFileDialogAsync("Save JSON file…");
    }
    
}