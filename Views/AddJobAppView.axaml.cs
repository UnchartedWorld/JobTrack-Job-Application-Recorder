using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Job_Application_Recorder.ViewModels;

namespace Job_Application_Recorder.Views;

public partial class AddJobAppView : UserControl
{
    public AddJobAppView()
    {
        InitializeComponent();
        DataContext = new AddJobAppViewModel();
    }
}