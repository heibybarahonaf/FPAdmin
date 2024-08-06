using Firebase.Database;
using Firebase.Database.Query;
using FPAdmin.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Firebase.Storage;

namespace FPAdmin.ViewModels
{
    public class EditarProductoViewModel : BindableObject
    {
        private readonly FirebaseClient _client = new FirebaseClient("https://flowersparadise-7ba1b-default-rtdb.firebaseio.com/");
        private Producto _producto;

        public Producto Producto
        {
            get => _producto;
            set
            {
                _producto = value;
                OnPropertyChanged();
            }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand SeleccionarCommand { get; set; }
        private string UrlFoto { get; set; }

        public EditarProductoViewModel()
        {
            GuardarCommand = new Command(async () => await OnGuardarCommand());
            CancelarCommand = new Command(async () => await OnCancelarCommand());
            SeleccionarCommand = new Command(async () => await OnSeleccionarCommand());
        }

        public async Task LoadProductAsync(string name)
        {
            // Cargar el producto desde Firebase
            var productos = await _client.Child("products").OnceAsync<Producto>();
            var producto = productos.FirstOrDefault(p => p.Object.Nombre == name);
            if (producto != null)
            {
                Producto = producto.Object;
            }
        }

        private async Task OnGuardarCommand()
        {
            try
            {
                if (Producto != null)
                {
                    var productos = await _client.Child("products").OnceAsync<Producto>();
                    var productoFirebase = productos.FirstOrDefault(p => p.Object.Nombre == Producto.Nombre);

                    if (productoFirebase != null)
                    {
                        await _client.Child("products").Child(productoFirebase.Key).PutAsync(Producto);

                        // Enviar mensaje de notificación
                        MessagingCenter.Send(this, "ProductoActualizado");

                        await Shell.Current.GoToAsync(".."); // Navegar hacia atrás
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Producto no encontrado en la base de datos", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El producto es nulo", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }


        private async Task OnCancelarCommand()
        {
            await Shell.Current.GoToAsync(".."); // Navegar hacia atrás
        }

        private async Task OnSeleccionarCommand()
        {
            try
            {
                var foto = await MediaPicker.PickPhotoAsync();
                if (foto != null)
                {
                    var stream = await foto.OpenReadAsync();
                    UrlFoto = await new FirebaseStorage("flowersparadise-7ba1b.appspot.com")
                                    .Child("photos")
                                    .Child(DateTime.Now.ToString("ddMMyyhhmmss") + foto.FileName)
                                    .PutAsync(stream);

                    // Actualiza el Producto con la URL de la foto
                    Producto.Foto = UrlFoto;

                    OnPropertyChanged(nameof(Producto)); // Notificar que Producto ha cambiado
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error al seleccionar la foto: {ex.Message}", "OK");
            }
        }

    }

}


