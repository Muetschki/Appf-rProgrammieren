using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using SkischoolHub.Services;

namespace SkischoolHub.ViewModels;

public partial class RegistrationViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

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

    public RegistrationViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        HasError = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Bitte alle Felder ausfüllen.";
            HasError = true;
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Passwort muss mindestens 6 Zeichen haben.";
            HasError = true;
            return;
        }

        IsBusy = true;

        try
        {
            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };

            var success = await _apiService.RegisterUserAsync(user);

            if (!success)
            {
                ErrorMessage = "Registrierung fehlgeschlagen.";
                HasError = true;
                return;
            }

            await Shell.Current.DisplayAlert("Erfolg", "Registrierung erfolgreich.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Fehler bei der Registrierung: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}