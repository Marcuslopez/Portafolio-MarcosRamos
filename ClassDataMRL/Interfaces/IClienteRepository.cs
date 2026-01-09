using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ClassDomainMRL.Entities;

namespace ClassDataMRL.Interfaces
{
    public interface IClienteRepository
    {

        IEnumerable<Cliente> Listar();
        Cliente ObtenerPorId(int idCliente);
        void Guardar(Cliente cliente, string operacion);

    }
}

