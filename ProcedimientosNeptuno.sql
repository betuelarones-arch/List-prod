USE [Neptuno]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_ListarProductos]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT idproducto, nombreProducto, idProveedor, idCategoria,
           cantidadPorUnidad, precioUnidad, unidadesEnExistencia,
           unidadesEnPedido, nivelNuevoPedido, suspendido, categoriaProducto
    FROM dbo.productos
    ORDER BY nombreProducto;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_ListarCategorias]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT idcategoria, nombrecategoria, descripcion, Activo, CodCategoria
    FROM dbo.categorias
    ORDER BY nombrecategoria;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_ListarProveedores]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT idProveedor, nombreCompania, nombrecontacto, cargocontacto,
           direccion, ciudad, region, codPostal, pais, telefono, fax,
           paginaprincipal
    FROM dbo.proveedores
    ORDER BY nombreCompania;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_BuscarProveedores]
    @nombrecontacto varchar(30),
    @ciudad varchar(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT idProveedor, nombreCompania, nombrecontacto, cargocontacto,
           direccion, ciudad, region, codPostal, pais, telefono, fax,
           paginaprincipal
    FROM dbo.proveedores
    WHERE nombrecontacto LIKE '%' + ISNULL(@nombrecontacto, '') + '%'
      AND ciudad LIKE '%' + ISNULL(@ciudad, '') + '%'
    ORDER BY nombreCompania;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_ListarDetallesPedidosPorFecha]
    @fechaDesde date,
    @fechaHasta date
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.idpedido, d.idproducto, d.preciounidad, d.cantidad, d.descuento,
           p.idcliente, p.idempleado, p.fechapedido, p.fechaentrega,
           p.fechaenvio, p.formaenvio, p.cargo, p.destinatario,
           p.direcciondestinatario, p.ciudaddestinatario,
           p.regiondestinatario, p.codpostaldestinatario, p.paisdestinatario
    FROM dbo.detallesdepedidos AS d
    INNER JOIN dbo.Pedidos AS p ON p.idpedido = d.idpedido
    WHERE p.fechapedido >= @fechaDesde
      AND p.fechapedido <= @fechaHasta
    ORDER BY p.fechapedido, d.idpedido, d.idproducto;
END
GO