using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using SkischoolHub.Services;
using System.Collections.ObjectModel;

namespace SkischoolHub.ViewModels;

public partial class PrivateLessonViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IUserSessionService _userSessionService;

    public ObservableCollection<SkiTeacher> Teachers { get; } = [];
    public ObservableCollection<PrivateLessonCardViewModel> MyLessons { get; } = [];

    public List<string> TimeSlots { get; } =
    [
        "08:00", "09:00", "10:00", "11:00",
        "12:00", "13:00", "14:00", "15:00", "16:00"
    ];

    [ObservableProperty]
    private SkiTeacher? selectedTeacher;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string selectedTimeSlot = "09:00";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isBookingVisible;

    public PrivateLessonViewModel(IApiService apiService, IUserSessionService userSessionService)
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

        try
        {
            var teachers = await _apiService.GetSkiTeachersAsync();
            Teachers.Clear();
            foreach (var t in teachers)
                Teachers.Add(t);

            var lessons = await _apiService.GetPrivateLessonsAsync(currentUser.Id);
            MyLessons.Clear();
            foreach (var l in lessons)
            {
                MyLessons.Add(new PrivateLessonCardViewModel
                {
                    Id = l.Id,
                    TeacherName = l.SkiTeacher != null
                        ? $"{l.SkiTeacher.FirstName} {l.SkiTeacher.LastName}"
                        : $"Lehrer #{l.SkiTeacherId}",
                    Specialty = l.SkiTeacher?.Specialty ?? string.Empty,
                    LessonDate = l.LessonDate,
                    TimeSlot = l.TimeSlot,
                    Price = l.Price
                });
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Daten konnten nicht geladen werden: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleBookingForm()
    {
        IsBookingVisible = !IsBookingVisible;
    }

    [RelayCommand]
    private async Task BookLessonAsync()
    {
        if (IsBusy) return;

        var currentUser = _userSessionService.CurrentUser;
        if (currentUser is null)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        if (SelectedTeacher is null)
        {
            ErrorMessage = "Bitte einen Lehrer auswählen.";
            HasError = true;
            return;
        }

        if (SelectedDate < DateTime.Today)
        {
            ErrorMessage = "Bitte ein Datum in der Zukunft wählen.";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedTimeSlot))
        {
            ErrorMessage = "Bitte eine Uhrzeit auswählen.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _apiService.BookPrivateLessonAsync(
                currentUser.Id,
                SelectedTeacher.Id,
                SelectedDate,
                SelectedTimeSlot);

            if (result is null)
            {
                ErrorMessage = "Buchung fehlgeschlagen. Dieser Lehrer ist zu diesem Zeitpunkt bereits gebucht.";
                HasError = true;
                return;
            }

            IsBookingVisible = false;
            await Shell.Current.DisplayAlert(
                "Erfolg",
                $"Privatstunde mit {SelectedTeacher.FirstName} {SelectedTeacher.LastName} am {SelectedDate:dd.MM.yyyy} um {SelectedTimeSlot} Uhr gebucht!",
                "OK");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Fehler bei der Buchung: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelLessonAsync(PrivateLessonCardViewModel? lesson)
    {
        if (lesson is null || IsBusy) return;

        var currentUser = _userSessionService.CurrentUser;
        if (currentUser is null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Stornieren",
            $"Privatstunde am {lesson.LessonDate:dd.MM.yyyy} um {lesson.TimeSlot} Uhr wirklich stornieren?",
            "Ja", "Nein");

        if (!confirm) return;

        IsBusy = true;
        try
        {
            var success = await _apiService.CancelPrivateLessonAsync(lesson.Id, currentUser.Id);
            if (success)
                MyLessons.Remove(lesson);
            else
            {
                ErrorMessage = "Stornierung fehlgeschlagen.";
                HasError = true;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}

public partial class PrivateLessonCardViewModel : ObservableObject
{
    public int Id { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public DateTime LessonDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public string DateText => $"{LessonDate:dd.MM.yyyy} um {TimeSlot} Uhr";
    public string PriceText => $"{Price:0.00} €";
}