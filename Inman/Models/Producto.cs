namespace Inman.Models;

public class Producto
{
    public string CodigoProducto { get; set; }
    public int IdTipoProducto { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int PorcentajeDescuento { get; set; }
}