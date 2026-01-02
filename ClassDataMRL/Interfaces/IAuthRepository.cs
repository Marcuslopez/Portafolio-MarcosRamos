using ClassDataMRL.Repositories;
using ClassDomainMRL.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassDataMRL.Interfaces
{
    public interface IAuthRepository
    {
        // Devuelve datos del usuario si son credenciales correctas; null si no.
        (int IdUsuario, string Nombre, string Email, int IdRol, string Rol)? Login(string email, string passwordHash);
    }
}
