namespace Inman.Models;

public class FacturaProducto
{
    public int Id { get; set; }
    public string CodigoFactura { get; set; }
    public string CodigoProducto { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; }
}