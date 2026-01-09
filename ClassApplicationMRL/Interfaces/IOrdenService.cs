using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassDomainMRL.DTOs;

namespace ClassApplicationMRL.Interfaces
{
    public interface IOrdenService
    {
        int CrearOrden(OrdenCreateDto dto);

        IEnumerable<OrdenResponseDto> ObtenerOrdenes();

        OrdenResponseDto ObtenerOrdenPorId(int idOrden);

        void CambiarEstado(int idOrden, string nuevoEstado);


    }
}
