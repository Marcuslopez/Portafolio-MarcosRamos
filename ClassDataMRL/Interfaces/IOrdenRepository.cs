using ClassDomainMRL.Entities;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace ClassDataMRL.Interfaces
{
    public interface IOrdenRepository
    {

        int InsertarOrden(int idCliente, decimal total, SqlConnection cn, SqlTransaction tx);
        IEnumerable<Orden> ListarOrdenes(SqlConnection cn);
        Orden ObtenerPorId(int idOrden, SqlConnection cn);
        void CambiarEstado(int idOrden, string estado, SqlConnection cn);

    }


}
