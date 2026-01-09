using System.ComponentModel.DataAnnotations;

namespace PortalWebMRL.Models
{
    public class ProductoCreateViewModel
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int IdCategoria { get; set; }
    }
}
