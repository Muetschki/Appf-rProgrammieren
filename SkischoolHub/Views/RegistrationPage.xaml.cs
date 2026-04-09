using SkischoolHub.ViewModels;

namespace SkischoolHub.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage(RegistrationViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}