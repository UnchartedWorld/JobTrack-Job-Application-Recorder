using Job_Application_Recorder.Services;
using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;
    
    public LandingPageViewModel LandingPage { get; }
    public AddJobAppViewModel AddJobPage { get; }
    public ViewJobAppViewModel ViewJobPage { get; }

    public MainWindowViewModel()
    {
        LandingPage = new LandingPageViewModel();
        AddJobPage = new AddJobAppViewModel();
        ViewJobPage = new ViewJobAppViewModel();

        AppSettings appSettings = AppSettingsService.LoadAppSettings();

        if (!string.IsNullOrWhiteSpace(appSettings.LastFilePathUsed))
        {
            _contentViewModel = ViewJobPage;
        }
        else
        {
            _contentViewModel = LandingPage;
        }
    }

    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
    }
}
