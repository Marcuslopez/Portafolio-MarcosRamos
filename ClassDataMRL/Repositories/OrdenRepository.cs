using ClassDataMRL.Interfaces;
using ClassDomainMRL.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassPortafolioMRL.Repositories
{
    public class OrdenRepository : IOrdenRepository
    {
        public int InsertarOrden(int idCliente,decimal total,SqlConnection connection,SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand("sp_Ordenes", connection, transaction))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Operacion", "OpAdd");
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@Total", total);

                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }


        }

        public IEnumerable<Orden> ListarOrdenes(SqlConnection cn)
        {
            var lista = new List<Orden>();

            using (var cmd = new SqlCommand("sp_Ordenes", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", "OpList");

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Orden
                        {
                            IdOrden = Convert.ToInt32(dr["IdOrden"]),
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            FechaOrden = Convert.ToDateTime(dr["FechaOrden"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            Estado = dr["Estado"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public Orden ObtenerPorId(int idOrden, SqlConnection cn)
        {
            using (var cmd = new SqlCommand("sp_Ordenes", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", "OpGet");
                cmd.Parameters.AddWithValue("@IdOrden", idOrden);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Orden
                        {
                            IdOrden = Convert.ToInt32(dr["IdOrden"]),
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            FechaOrden = Convert.ToDateTime(dr["FechaOrden"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            Estado = dr["Estado"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void CambiarEstado(int idOrden, string estado, SqlConnection cn)
        {
            

            using (SqlCommand cmd = new SqlCommand("sp_Ordenes", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Operacion", "OpModEstado");
                cmd.Parameters.AddWithValue("@IdOrden", idOrden);
                cmd.Parameters.AddWithValue("@Estado", estado);

                if (cn.State != ConnectionState.Open) cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
