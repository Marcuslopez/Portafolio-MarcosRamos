namespace PortalWebMRL.Models
{
    public class OrdenDetalleItemViewModel
    {
        public int IdProducto { get; set; }

        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
