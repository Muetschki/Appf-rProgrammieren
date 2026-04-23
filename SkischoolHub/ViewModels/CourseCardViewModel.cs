using CommunityToolkit.Mvvm.ComponentModel;

namespace SkischoolHub.ViewModels;

public partial class CourseCardViewModel : ObservableObject
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set
        {
            var fixedTime = new DateTime(value.Year, value.Month, value.Day, 9, 0, 0);
            if (SetProperty(ref _date, fixedTime))
            {
                OnPropertyChanged(nameof(DateText));
            }
        }
    }

    public decimal Price { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(ActionText))]
    [NotifyPropertyChangedFor(nameof(CanBook))]
    private bool isBooked;

    public string DateText => Date.ToString("dd.MM.yyyy HH:mm");
    public string PriceText => $"{Price:0.00} €";
    public string StatusText => IsBooked ? "Bereits gebucht" : "Verfügbar";
    public string ActionText => IsBooked ? "Stornieren" : "Jetzt buchen";
    public bool CanBook => true;
}
