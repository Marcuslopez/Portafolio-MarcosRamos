using ClassDataMRL.Interfaces;
using ClassDomainMRL.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClassDataMRL.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(IConfiguration configuration)
        {



            try
            {
                //Código donde se utiliza o inicializa por primera vez SqlConnection
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            catch (System.TypeInitializationException e)
            {
                // Registrar o inspeccionar la excepción interna para identificar el problema.
                Console.WriteLine(e.InnerException.ToString());
                throw;
            }

        }
  
        public IEnumerable<Cliente> Listar()
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var lista = new List<Cliente>();


                using var cmd = new SqlCommand("sp_Clientes",cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", "OpList");

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Cliente
                    {
                        IdCliente = (int)dr["IdCliente"],
                        Nombre = dr["Nombre"].ToString(),
                        Email = dr["Email"].ToString(),
                        Telefono = dr["Telefono"].ToString(),
                        FechaCreacion = (DateTime)dr["FechaCreacion"],
                        Activo = (bool)dr["Activo"]
                    });
                }


                return lista;
            }
        }

        public Cliente ObtenerPorId(int idCliente)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                Cliente cliente = null;

                using var cmd = new SqlCommand("sp_Clientes",cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", "OpGet");
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    cliente = new Cliente
                    {
                        IdCliente = (int)dr["IdCliente"],
                        Nombre = dr["Nombre"].ToString(),
                        Email = dr["Email"].ToString(),
                        Telefono = dr["Telefono"].ToString(),
                        FechaCreacion = (DateTime)dr["FechaCreacion"],
                        Activo = (bool)dr["Activo"]
                    };
                }

                return cliente;
            }
        }




        public void Guardar(Cliente cliente, string operacion)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                using var cmd = new SqlCommand("sp_Clientes",cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operacion", operacion);
                cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@FechaCreacion", "");
                cmd.Parameters.AddWithValue("@Activo", "");
                cmd.ExecuteNonQuery();

            }
        }  

    }



}
