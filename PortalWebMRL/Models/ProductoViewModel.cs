namespace PortalWebMRL.Models
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public int IdCategoria { get; set; }
        public bool Activo { get; set; }
    }
}
