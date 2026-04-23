using SkischoolHub.ViewModels;

namespace SkischoolHub.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainViewModel vm)
        {
            await vm.LoadDataCommand.ExecuteAsync(null);
        }
    }
}