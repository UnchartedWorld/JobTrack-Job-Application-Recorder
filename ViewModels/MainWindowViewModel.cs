using ReactiveUI;

namespace Job_Application_Recorder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;
    
    public LandingPageViewModel LandingPage { get; }
    public AddJobAppViewModel AddJobPage { get; }

    public MainWindowViewModel()
    {
        LandingPage = new LandingPageViewModel();
        AddJobPage = new AddJobAppViewModel();

        _contentViewModel = LandingPage;
    }

    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
    }
}
