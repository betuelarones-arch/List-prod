using System.Windows;

namespace demo05;

public partial class ProductosWindow : Window
{
    private readonly NeptunoRepository repository = new();

    public ProductosWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await EjecutarConsultaAsync(async () =>
        {
            ProductosGrid.ItemsSource = await repository.ListarProductosAsync();
        });
    }

    private static async Task EjecutarConsultaAsync(Func<Task> consulta)
    {
        try { await consulta(); }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo listar productos:\n{ex.Message}", "Error de base de datos",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}