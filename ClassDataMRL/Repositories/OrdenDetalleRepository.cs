using ClassDataMRL.Interfaces;
using ClassDomainMRL.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassDataMRL.Repositories
{
    public class OrdenDetalleRepository : IOrdenDetalleRepository
    {
        public void InsertarDetalle(
            int idOrden,
            int idProducto,
            int cantidad,
            decimal precioUnitario,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand("sp_OrdenesDetalle", connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Operacion", "OpAdd");
                cmd.Parameters.AddWithValue("@IdOrden", idOrden);
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", precioUnitario);

                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<OrdenDetalle> ListarPorOrden(int idOrden, SqlConnection cn)
        {
            var lista = new List<OrdenDetalle>();

            using (var cmd = new SqlCommand("sp_OrdenesDetalle", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", "OpList");
                cmd.Parameters.AddWithValue("@IdOrden", idOrden);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new OrdenDetalle
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                            Subtotal = Convert.ToDecimal(dr["Subtotal"])
                        });
                    }
                }
            }
            return lista;
        }
    }
}