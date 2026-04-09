using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkischoolHub.Services;
using Models;

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

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Bitte füllen Sie alle Felder aus.";
            HasError = true;
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Das Passwort muss mindestens 6 Zeichen lang sein.";
            HasError = true;
            return;
        }

        try
        {
            IsBusy = true;

            User newUser = new()
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };

            bool success = await _apiService.RegisterUserAsync(newUser);
            if (success)
            {
                await Shell.Current.DisplayAlert("Erfolg", "Die Registrierung war erfolgreich.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                ErrorMessage = "Die Registrierung ist fehlgeschlagen.";
                HasError = true;
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = "Keine Verbindung zum Server möglich. Bitte überprüfen Sie, ob die API läuft.";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"Verbindungsfehler: {ex.Message}");
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