using Microsoft.Extensions.DependencyInjection;

namespace SkischoolHub
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}