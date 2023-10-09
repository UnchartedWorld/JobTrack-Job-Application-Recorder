using Avalonia.Controls;
using Job_Application_Recorder.ViewModels;

namespace Job_Application_Recorder.Views;

public partial class LandingPageView : UserControl
{
    public LandingPageView()
    {
        InitializeComponent();
        DataContext = new LandingPageViewModel();
    }
}