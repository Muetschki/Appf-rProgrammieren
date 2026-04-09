using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkischoolHub.Services;

namespace SkischoolHub.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        HasError = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Bitte E-Mail und Passwort eingeben.";
            HasError = true;
            return;
        }

        IsBusy = true;

        try
        {
            var user = await _apiService.LoginAsync(Email, Password);

            if (user is null)
            {
                ErrorMessage = "Ungültige E-Mail oder Passwort.";
                HasError = true;
                return;
            }

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Fehler beim Anmelden: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToRegistrationAsync()
    {
        await Shell.Current.GoToAsync("//RegistrationPage");
    }
}