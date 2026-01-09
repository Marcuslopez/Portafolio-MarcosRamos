using ClassDomainMRL.Entities;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace ClassDataMRL.Interfaces
{
    public interface IOrdenDetalleRepository
    {
        void InsertarDetalle(int idOrden, int idProducto, int cantidad, decimal precio, SqlConnection cn, SqlTransaction tx);

        IEnumerable<OrdenDetalle> ListarPorOrden(int idOrden, SqlConnection cn);
    }
}
