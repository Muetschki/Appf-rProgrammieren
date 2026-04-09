using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkischoolHub.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private async Task LogoutAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Logout",
            "Möchten Sie sich wirklich abmelden?",
            "Ja",
            "Nein");

        if (confirm)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}