using ClassDataMRL.Interfaces;
using ClassDomainMRL.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;



namespace ClassDataMRL.Repositories
{

    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            


            try
            {
                // Code where the SqlConnection is first used or initialized
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            catch (System.TypeInitializationException e)
            {
                // Log or inspect the inner exception to find the actual problem
                Console.WriteLine(e.InnerException.ToString());
                throw;
            }


        }

        // CRUD normal (Controllers)
        public IEnumerable<Producto> Listar()
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var lista = new List<Producto>();

                using var cmd = new SqlCommand("sp_Productos",cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", "OpList");


                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = (int)dr["IdProducto"],
                        Nombre = dr["Nombre"].ToString(),
                        Descripcion = dr["Descripcion"].ToString(),
                        Precio = (decimal)dr["Precio"],
                        Stock = (int)dr["Stock"],
                        IdCategoria = (int)dr["IdCategoria"],
                        Activo = (bool)dr["Activo"]
                    });
                }


                return lista;
            }
        }

        public Producto ObtenerPorId(int idProducto)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                Producto producto = null;

                using var cmd = new SqlCommand("sp_Productos",cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Operacion", "OpGet");
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);


                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    producto = new Producto
                    {
                        IdProducto = (int)dr["IdProducto"],
                        Nombre = dr["Nombre"].ToString(),
                        Descripcion = dr["Descripcion"].ToString(),
                        Precio = (decimal)dr["Precio"],
                        Stock = (int)dr["Stock"],
                        IdCategoria = (int)dr["IdCategoria"],
                        Activo = (bool)dr["Activo"]
                    };
                }


                return producto;
            }
        }


        public int Guardar(Producto producto, string operacion)
        {

            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_Productos",cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Operacion", operacion);
                    cmd.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                    cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@Stock", producto.Stock);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            
        }

        public void ActualizarStock(int idProducto,int cantidad,SqlConnection connection,SqlTransaction transaction)
        {
            using var cmd = new SqlCommand("sp_Productos", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Operacion", "OpUpdateStock");
            cmd.Parameters.AddWithValue("@IdProducto", idProducto);
            cmd.Parameters.AddWithValue("@Cantidad", cantidad);

            cmd.ExecuteNonQuery();
        }
    }


}
