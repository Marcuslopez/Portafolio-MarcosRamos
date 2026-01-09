namespace PortalWebMRL.Models
{
    public class OrdenDetailsViewModel
    {
        public int IdOrden { get; set; }
        public int IdCliente { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;

        public DateTime FechaOrden { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;

        public List<OrdenDetalleItemViewModel> Detalles { get; set; }
            = new List<OrdenDetalleItemViewModel>();
    }
}
