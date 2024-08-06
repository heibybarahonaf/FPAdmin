using Microsoft.Extensions.Logging;
using Firebase.Database;
using Firebase.Database.Query;
using FPAdmin.Models;

namespace FPAdmin
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            
    		builder.Logging.AddDebug();
#endif
            Registrar();
            return builder.Build();
        }

        public static void Registrar()
        {
            FirebaseClient client = new FirebaseClient("https://flowersparadise-7ba1b-default-rtdb.firebaseio.com/");
            var categorias = client.Child("Categorias").OnceAsync<Categoria>();
            if(categorias.Result.Count == 0)
            {
                client.Child("Categorias").PostAsync(new Categoria { Nombre = "Arreglos" });
                client.Child("Categorias").PostAsync(new Categoria { Nombre = "Aniversarios" });
            }
        }
    }
}
