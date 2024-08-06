using Firebase.Database;
using Firebase.Database.Query;
using FPAdmin.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using FPAdmin.Views;

namespace FPAdmin.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        private FirebaseClient client = new FirebaseClient("https://flowersparadise-7ba1b-default-rtdb.firebaseio.com/");
        private ObservableCollection<Producto> _productosOriginales;
        private string _filtro;

        public ObservableCollection<Producto> Lista { get; set; } = new ObservableCollection<Producto>();

        public string Filtro
        {
            get => _filtro;
            set
            {
                if (_filtro != value)
                {
                    _filtro = value;
                    OnPropertyChanged();
                    FiltrarLista();
                }
            }
        }

        public ICommand NuevoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }

        public MainPageViewModel()
        {
            NuevoCommand = new Command(async () => await OnNuevoCommand());
            EditarCommand = new Command<Producto>(async (producto) => await OnEditarCommand(producto));
            EliminarCommand = new Command<string>(async (nombre) => await OnEliminarCommand(nombre));

            // Suscribirse al mensaje de actualización
            MessagingCenter.Subscribe<NuevoProductoPage>(this, "ProductoAgregado", (sender) => CargarLista());
            MessagingCenter.Subscribe<EditarProductoViewModel>(this, "ProductoActualizado", (sender) => CargarLista());

            CargarLista();
        }

        private async void CargarLista()
        {
            var productos = await client
                .Child("products")
                .OnceAsync<Producto>();

            _productosOriginales = new ObservableCollection<Producto>(productos.Select(p => p.Object));
            Lista.Clear();

            foreach (var producto in _productosOriginales)
            {
                Lista.Add(producto);
            }
        }

        private void FiltrarLista()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                Lista.Clear();
                foreach (var producto in _productosOriginales)
                {
                    Lista.Add(producto);
                }
            }
            else
            {
                var filtro = Filtro.ToLower();
                var filteredList = _productosOriginales.Where(x => x.Nombre.ToLower().Contains(filtro)).ToList();
                Lista.Clear();
                foreach (var item in filteredList)
                {
                    Lista.Add(item);
                }
            }
        }

        private async Task OnNuevoCommand()
        {
            // Navega a la página de nuevo producto
            await Shell.Current.GoToAsync(nameof(NuevoProductoPage));
        }

        private async Task OnEditarCommand(Producto producto)
        {
            await Shell.Current.GoToAsync($"{nameof(EditarProductoPage)}?name={producto.Nombre}");

        }


        private async Task OnEliminarCommand(string nombre) 
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Estás seguro de que deseas eliminar este producto?", "Sí", "No");
            if (confirm)
            {
                var productoParaEliminar = _productosOriginales.FirstOrDefault(p => p.Nombre == nombre);
                if (productoParaEliminar != null)
                {
                    var productos = await client
                        .Child("products")
                        .OnceAsync<Producto>();

                    var productoFirebase = productos.FirstOrDefault(p => p.Object.Nombre == nombre);
                    if (productoFirebase != null)
                    {
                        await client
                            .Child("products")
                            .Child(productoFirebase.Key)
                            .DeleteAsync();

                        Lista.Remove(productoParaEliminar);
                        _productosOriginales.Remove(productoParaEliminar);
                    }
                }
            }
        }
    }
}
