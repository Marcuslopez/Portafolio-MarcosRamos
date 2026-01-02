using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDomainMRL.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public string Rol { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }
}
