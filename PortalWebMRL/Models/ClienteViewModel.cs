namespace PortalWebMRL.Models
{
    public class ClienteViewModel
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}
