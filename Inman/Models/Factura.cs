namespace Inman.Models;

public class Factura
{
    public string CodigoFactura { get; set; }
    public int IdCliente { get; set; }
    public int PorcentajeImpuesto { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public decimal DescuentoTotal { get; set; }
    public decimal ImpuestoTotal { get; set; }
}