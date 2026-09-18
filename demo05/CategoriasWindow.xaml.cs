using System.Windows;

namespace demo05;

public partial class CategoriasWindow : Window
{
    private readonly NeptunoRepository repository = new();

    public CategoriasWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await EjecutarConsultaAsync(async () =>
        {
            CategoriasGrid.ItemsSource = await repository.ListarCategoriasAsync();
        });
    }

    private static async Task EjecutarConsultaAsync(Func<Task> consulta)
    {
        try { await consulta(); }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo listar categorias:\n{ex.Message}", "Error de base de datos",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}