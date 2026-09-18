using System.Windows;

namespace demo05;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Productos_Click(object sender, RoutedEventArgs e)
    {
        new ProductosWindow { Owner = this }.ShowDialog();
    }

    private void Categorias_Click(object sender, RoutedEventArgs e)
    {
        new CategoriasWindow { Owner = this }.ShowDialog();
    }

    private void Proveedores_Click(object sender, RoutedEventArgs e)
    {
        new ProveedoresWindow { Owner = this }.ShowDialog();
    }
}