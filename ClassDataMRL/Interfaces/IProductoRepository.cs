using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassDomainMRL.Entities;

namespace ClassDataMRL.Interfaces

{
    public interface IProductoRepository
    {
        IEnumerable<Producto> Listar();
        Producto ObtenerPorId(int idProducto);
        void Guardar(Producto producto, string operacion);
    }



}
