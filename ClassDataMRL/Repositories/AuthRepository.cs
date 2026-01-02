using ClassDataMRL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;


namespace ClassDataMRL.Repositories
{
    
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public (int IdUsuario, string Nombre, string Email, int IdRol, string Rol)? Login(string email, string passwordHash)
        {
            SqlConnection cn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("dbo.sp_Login", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read()) return null;

            return (
                IdUsuario: (int)dr["IdUsuario"],
                Nombre: dr["Nombre"].ToString() ?? "",
                Email: dr["Email"].ToString() ?? "",
                IdRol: (int)dr["IdRol"],
                Rol: dr["Rol"].ToString() ?? ""
            );
        }
    }
}
