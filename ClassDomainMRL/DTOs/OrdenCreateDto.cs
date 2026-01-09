using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDomainMRL.DTOs
{
    public class OrdenCreateDto
    {
        public int IdCliente { get; set; }
        public List<OrdenDetalleCreateDto> Detalles { get; set; } = new();
    }
}
