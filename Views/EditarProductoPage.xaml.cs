using FPAdmin.Models;
using FPAdmin.ViewModels;
using Microsoft.Maui.Controls;

namespace FPAdmin.Views
{
    [QueryProperty(nameof(ProductName), "name")]
    public partial class EditarProductoPage : ContentPage
    {
        private readonly EditarProductoViewModel _viewModel;

        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged();
                LoadProduct();
            }
        }

        private string _productName;

        public EditarProductoPage()
        {
            InitializeComponent();
            _viewModel = new EditarProductoViewModel();
            BindingContext = _viewModel;
        }

        private async void LoadProduct()
        {
            if (!string.IsNullOrEmpty(ProductName))
            {
                await _viewModel.LoadProductAsync(ProductName);
            }
        }

    }
}

