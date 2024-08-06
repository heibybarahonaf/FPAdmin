using FPAdmin.Views;

namespace FPAdmin
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Rutas de acceso
            Routing.RegisterRoute(nameof(NuevoProductoPage), typeof(NuevoProductoPage));
            Routing.RegisterRoute(nameof(EditarProductoPage), typeof(EditarProductoPage));
        }
    }
}
