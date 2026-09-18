using System.Data;
using Microsoft.Data.SqlClient;

namespace demo05;

public sealed class NeptunoRepository
{
    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=Neptuno;Trusted_Connection=True;TrustServerCertificate=True;";

    public Task<List<Producto>> ListarProductosAsync() =>
        ExecuteAsync("dbo.usp_ListarProductos", LeerProductosAsync);

    public Task<List<Categoria>> ListarCategoriasAsync() =>
        ExecuteAsync("dbo.usp_ListarCategorias", LeerCategoriasAsync);

    public Task<List<Proveedor>> ListarProveedoresAsync() =>
        ExecuteAsync("dbo.usp_ListarProveedores", LeerProveedoresAsync);

    public Task<List<Proveedor>> BuscarProveedoresAsync(string nombreContacto, string ciudad) =>
        ExecuteAsync(
            "dbo.usp_BuscarProveedores",
            LeerProveedoresAsync,
            new SqlParameter("@nombrecontacto", SqlDbType.VarChar, 30) { Value = nombreContacto },
            new SqlParameter("@ciudad", SqlDbType.VarChar, 15) { Value = ciudad });

    public Task<List<DetallePedido>> ListarDetallesPedidosAsync(DateTime fechaDesde, DateTime fechaHasta) =>
        ExecuteAsync(
            "dbo.usp_ListarDetallesPedidosPorFecha",
            LeerDetallesAsync,
            new SqlParameter("@fechaDesde", SqlDbType.Date) { Value = fechaDesde.Date },
            new SqlParameter("@fechaHasta", SqlDbType.Date) { Value = fechaHasta.Date });

    private static async Task<List<T>> ExecuteAsync<T>(
        string procedureName,
        Func<SqlDataReader, Task<List<T>>> readRows,
        params SqlParameter[] parameters)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await using var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddRange(parameters);

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        return await readRows(reader);
    }

    private static async Task<List<Producto>> LeerProductosAsync(SqlDataReader reader)
    {
        var rows = new List<Producto>();
        while (await reader.ReadAsync())
        {
            rows.Add(new Producto
            {
                IdProducto = reader.GetInt32(0),
                NombreProducto = GetString(reader, 1),
                IdProveedor = GetNullableInt(reader, 2),
                IdCategoria = GetNullableInt(reader, 3),
                CantidadPorUnidad = GetString(reader, 4),
                PrecioUnidad = GetNullableDecimal(reader, 5),
                UnidadesEnExistencia = GetNullableInt16(reader, 6),
                UnidadesEnPedido = GetNullableInt16(reader, 7),
                NivelNuevoPedido = GetNullableInt16(reader, 8),
                Suspendido = GetNullableInt16(reader, 9),
                CategoriaProducto = GetString(reader, 10)
            });
        }
        return rows;
    }

    private static async Task<List<Categoria>> LeerCategoriasAsync(SqlDataReader reader)
    {
        var rows = new List<Categoria>();
        while (await reader.ReadAsync())
        {
            rows.Add(new Categoria
            {
                IdCategoria = reader.GetInt32(0),
                NombreCategoria = reader.GetString(1),
                Descripcion = GetString(reader, 2),
                Activo = GetNullableBool(reader, 3),
                CodigoCategoria = GetString(reader, 4)
            });
        }
        return rows;
    }

    private static async Task<List<Proveedor>> LeerProveedoresAsync(SqlDataReader reader)
    {
        var rows = new List<Proveedor>();
        while (await reader.ReadAsync())
        {
            rows.Add(new Proveedor
            {
                IdProveedor = reader.GetInt32(0),
                NombreCompania = reader.GetString(1),
                NombreContacto = GetString(reader, 2),
                CargoContacto = GetString(reader, 3),
                Direccion = GetString(reader, 4),
                Ciudad = GetString(reader, 5),
                Region = GetString(reader, 6),
                CodigoPostal = GetString(reader, 7),
                Pais = GetString(reader, 8),
                Telefono = GetString(reader, 9),
                Fax = GetString(reader, 10),
                PaginaPrincipal = GetString(reader, 11)
            });
        }
        return rows;
    }

    private static async Task<List<DetallePedido>> LeerDetallesAsync(SqlDataReader reader)
    {
        var rows = new List<DetallePedido>();
        while (await reader.ReadAsync())
        {
            rows.Add(new DetallePedido
            {
                IdPedido = GetNullableInt(reader, 0),
                IdProducto = GetNullableInt(reader, 1),
                PrecioUnidad = reader.GetDecimal(2),
                Cantidad = reader.GetInt32(3),
                Descuento = reader.GetDecimal(4),
                IdCliente = reader.GetString(5),
                IdEmpleado = reader.GetInt32(6),
                FechaPedido = GetNullableDate(reader, 7),
                FechaEntrega = GetNullableDate(reader, 8),
                FechaEnvio = GetNullableDate(reader, 9),
                FormaEnvio = GetNullableInt(reader, 10),
                Cargo = GetNullableDecimal(reader, 11),
                Destinatario = GetString(reader, 12),
                DireccionDestinatario = GetString(reader, 13),
                CiudadDestinatario = GetString(reader, 14),
                RegionDestinatario = GetString(reader, 15),
                CodigoPostalDestinatario = GetString(reader, 16),
                PaisDestinatario = GetString(reader, 17)
            });
        }
        return rows;
    }

    private static string? GetString(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

    private static int? GetNullableInt(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);

    private static short? GetNullableInt16(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetInt16(ordinal);

    private static decimal? GetNullableDecimal(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);

    private static bool? GetNullableBool(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);

    private static DateTime? GetNullableDate(SqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
}

public sealed class Producto
{
    public int IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public int? IdProveedor { get; set; }
    public int? IdCategoria { get; set; }
    public string? CantidadPorUnidad { get; set; }
    public decimal? PrecioUnidad { get; set; }
    public short? UnidadesEnExistencia { get; set; }
    public short? UnidadesEnPedido { get; set; }
    public short? NivelNuevoPedido { get; set; }
    public short? Suspendido { get; set; }
    public string? CategoriaProducto { get; set; }
}

public sealed class Categoria
{
    public int IdCategoria { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool? Activo { get; set; }
    public string? CodigoCategoria { get; set; }
}

public sealed class Proveedor
{
    public int IdProveedor { get; set; }
    public string NombreCompania { get; set; } = string.Empty;
    public string? NombreContacto { get; set; }
    public string? CargoContacto { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Region { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Pais { get; set; }
    public string? Telefono { get; set; }
    public string? Fax { get; set; }
    public string? PaginaPrincipal { get; set; }
}

public sealed class DetallePedido
{
    public int? IdPedido { get; set; }
    public int? IdProducto { get; set; }
    public decimal PrecioUnidad { get; set; }
    public int Cantidad { get; set; }
    public decimal Descuento { get; set; }
    public string IdCliente { get; set; } = string.Empty;
    public int IdEmpleado { get; set; }
    public DateTime? FechaPedido { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public int? FormaEnvio { get; set; }
    public decimal? Cargo { get; set; }
    public string? Destinatario { get; set; }
    public string? DireccionDestinatario { get; set; }
    public string? CiudadDestinatario { get; set; }
    public string? RegionDestinatario { get; set; }
    public string? CodigoPostalDestinatario { get; set; }
    public string? PaisDestinatario { get; set; }
}