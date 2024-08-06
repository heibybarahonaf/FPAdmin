using Firebase.Database;
using Firebase.Storage;
using Firebase.Database.Query;
using FPAdmin.Models;

namespace FPAdmin.Views;

public partial class NuevoProductoPage : ContentPage
{
	FirebaseClient client = new FirebaseClient("https://flowersparadise-7ba1b-default-rtdb.firebaseio.com/");
	public List<Categoria> Categorias { get; set; }
    private string UrlFoto { get; set; }

	public NuevoProductoPage()
	{
		InitializeComponent();
		CargarCategorias();
		BindingContext = this;
	}

	public void CargarCategorias()
	{
		var categorias = client.Child("Categorias").OnceAsync<Categoria>();
		Categorias = categorias.Result.Select(x => x.Object).ToList();
	}

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {
        // Create a new product
        var nuevoProducto = new Producto
        {
            Categoria = categoriaEntry.Text,
            Nombre = nombreEntry.Text,
            Descripcion = descripcionEntry.Text,
            Precio = int.Parse(PrecioEntry.Text),
            Foto = UrlFoto
        };

        // Save the product to Firebase
        await client.Child("products").PostAsync(nuevoProducto);

        // Notify the main page about the new product
        MessagingCenter.Send(this, "ProductoAgregado");

        // Navigate back to the previous page
        await Shell.Current.GoToAsync("..");

    }

    private async void seleccionarButton_Clicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.PickPhotoAsync();
        if (foto != null) 
        {
            var stream = await foto.OpenReadAsync();
            UrlFoto = await new FirebaseStorage("flowersparadise-7ba1b.appspot.com")
                            .Child("photos")
                            .Child(DateTime.Now.ToString("ddMMyyhhmmss") + foto.FileName)
                            .PutAsync(stream);
            fotoImage.Source = UrlFoto;
        }
    }
}