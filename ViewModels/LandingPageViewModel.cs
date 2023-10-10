using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Job_Application_Recorder.Services;
using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public class LandingPageViewModel : ViewModelBase
{
    private IEnumerable<string>? _selectedJSONFile;
    public ICommand SelectFileCommand { get; } 

    /// <summary>
    /// Gets or sets a singular file.
    /// </summary>
    public IEnumerable<string>? SelectedJSONFile
    {
        get => _selectedJSONFile;
        set => this.RaiseAndSetIfChanged(ref _selectedJSONFile, value);
    }

    public LandingPageViewModel()
    {
        SelectFileCommand = ReactiveCommand.CreateFromTask(SelectFileAsync);
    }

    public async Task SelectFileAsync()
    {
        SelectedJSONFile = await this.OpenFileDialogAsync("Open JSON File…");
    }
    
}