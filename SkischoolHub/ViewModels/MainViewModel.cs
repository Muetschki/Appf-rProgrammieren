using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using SkischoolHub.Services;
using System.Collections.ObjectModel;

namespace SkischoolHub.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IUserSessionService _userSessionService;

    public ObservableCollection<CourseCardViewModel> Courses { get; } = [];

    [ObservableProperty]
    private string welcomeText = "Willkommen";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool isBusy;

    public MainViewModel(IApiService apiService, IUserSessionService userSessionService)
    {
        _apiService = apiService;
        _userSessionService = userSessionService;
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy) return;

        var currentUser = _userSessionService.CurrentUser;
        if (currentUser is null)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;
        WelcomeText = $"Willkommen, {currentUser.FirstName}!";

        try
        {
            var courses = await _apiService.GetSkiCoursesAsync();
            var bookings = await _apiService.GetUserBookingsAsync(currentUser.Id);
            var bookedCourseIds = bookings.Select(x => x.SkiCourseId).ToHashSet();

            Courses.Clear();
            foreach (var course in courses.OrderBy(x => x.Date))
            {
                Courses.Add(new CourseCardViewModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Date = course.Date,
                    Price = course.Price,
                    IsBooked = bookedCourseIds.Contains(course.Id)
                });
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Kurse konnten nicht geladen werden: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task BookCourseAsync(CourseCardViewModel? course)
    {
        if (course is null || IsBusy) return;

        var currentUser = _userSessionService.CurrentUser;
        if (currentUser is null)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var success = course.IsBooked
                ? await _apiService.CancelCourseBookingAsync(currentUser.Id, course.Id)
                : await _apiService.BookCourseAsync(currentUser.Id, course.Id);

            if (!success)
            {
                ErrorMessage = course.IsBooked
                    ? "Stornierung fehlgeschlagen. Bitte erneut versuchen."
                    : "Buchung fehlgeschlagen. Bitte erneut versuchen.";
                HasError = true;
                return;
            }

            course.IsBooked = !course.IsBooked;

            if (course.IsBooked)
                await Shell.Current.DisplayAlert("Erfolg", "Skikurs erfolgreich gebucht.", "OK");
            else
                await Shell.Current.DisplayAlert("Erfolg", "Buchung erfolgreich storniert.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Logout", "Möchten Sie sich wirklich abmelden?", "Ja", "Nein");
        if (confirm)
        {
            _userSessionService.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task NavigateToPrivateLessonsAsync()
    {
        await Shell.Current.GoToAsync("//PrivateLessonPage");
    }
}