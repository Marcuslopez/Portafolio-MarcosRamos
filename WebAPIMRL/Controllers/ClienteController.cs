using ClassDataMRL.Interfaces;
using ClassDataMRL.Repositories;
using ClassDomainMRL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPIMRL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {

        private readonly IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }


        // GET: Admin o Vendedor
        [Authorize(Policy = "SalesOrAdmin")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_clienteRepository.Listar());
        }

        // GET by Id: Admin o Vendedor
        [Authorize(Policy = "SalesOrAdmin")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cliente = _clienteRepository.ObtenerPorId(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST: solo Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult Post([FromBody] Cliente cliente)
        {
            _clienteRepository.Guardar(cliente, "OpAdd");
            return Ok();
        }


        // PUT: solo Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Cliente cliente)
        {
            cliente.IdCliente = id;
            _clienteRepository.Guardar(cliente, "OpMod");
            return Ok();
        }

        // DELETE: solo Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
           _clienteRepository.Guardar(new Cliente { IdCliente = id }, "OpDel");
            return Ok();
        }
    }
}
