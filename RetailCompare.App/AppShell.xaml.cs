using RetailCompare.App.Views;

namespace RetailCompare.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
        }
    }
}