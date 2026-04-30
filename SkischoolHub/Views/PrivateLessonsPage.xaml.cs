using SkischoolHub.ViewModels;

namespace SkischoolHub.Views;

public partial class PrivateLessonPage : ContentPage
{
    public PrivateLessonPage(PrivateLessonViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PrivateLessonViewModel vm)
            await vm.LoadDataCommand.ExecuteAsync(null);
    }
}