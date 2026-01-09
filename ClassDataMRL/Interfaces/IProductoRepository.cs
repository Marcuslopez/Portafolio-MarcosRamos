using ClassDomainMRL.Entities;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;


namespace ClassDataMRL.Interfaces

{
    public interface IProductoRepository
    {        
        IEnumerable<Producto> Listar();
        int Guardar(Producto producto, string operacion);
        Producto ObtenerPorId(int idProducto);

        void ActualizarStock(int idProducto, int cantidad, SqlConnection connection, SqlTransaction transaction);
    }



}
