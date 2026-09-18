using System.Windows;

namespace demo05;

public partial class ProveedoresWindow : Window
{
    private readonly NeptunoRepository repository = new();

    public ProveedoresWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            FechaDesdePicker.SelectedDate = DateTime.Today.AddYears(-1);
            FechaHastaPicker.SelectedDate = DateTime.Today;
            await EjecutarConsultaAsync(async () =>
            {
                ProveedoresGrid.ItemsSource = await repository.ListarProveedoresAsync();
            });
        };
    }

    private async void BuscarProveedores_Click(object sender, RoutedEventArgs e)
    {
        await EjecutarConsultaAsync(async () =>
        {
            BusquedaProveedoresGrid.ItemsSource =
                await repository.BuscarProveedoresAsync(NombreContactoTextBox.Text.Trim(), CiudadTextBox.Text.Trim());
        });
    }

    private async void DetallesPedidos_Click(object sender, RoutedEventArgs e)
    {
        if (FechaDesdePicker.SelectedDate is not DateTime fechaDesde ||
            FechaHastaPicker.SelectedDate is not DateTime fechaHasta || fechaDesde > fechaHasta)
        {
            MessageBox.Show("Seleccione un intervalo de fechas valido.", "Datos incompletos",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await EjecutarConsultaAsync(async () =>
        {
            DetallesPedidosGrid.ItemsSource = await repository.ListarDetallesPedidosAsync(fechaDesde, fechaHasta);
        });
    }

    private static async Task EjecutarConsultaAsync(Func<Task> consulta)
    {
        try { await consulta(); }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo ejecutar la consulta:\n{ex.Message}", "Error de base de datos",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}