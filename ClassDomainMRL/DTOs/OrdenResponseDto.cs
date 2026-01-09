using ClassDomainMRL.DTOs.ClassDomainMRL.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDomainMRL.DTOs
{
    public class OrdenResponseDto
    {
        public int IdOrden { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaOrden { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public List<OrdenDetalleResponseDto> Detalles { get; set; }
            = new List<OrdenDetalleResponseDto>();
    }
}
