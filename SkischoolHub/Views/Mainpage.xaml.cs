using SkischoolHub.ViewModels;

namespace SkischoolHub;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}