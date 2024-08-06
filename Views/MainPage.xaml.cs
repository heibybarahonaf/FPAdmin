using Microsoft.Maui.Controls;
using FPAdmin.ViewModels;

namespace FPAdmin.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Configura el BindingContext para usar el ViewModel
            BindingContext = new MainPageViewModel();

        }
    }
}
