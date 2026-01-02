using Microsoft.AspNetCore.Mvc;
using ClassDataMRL.Interfaces;
using ClassDomainMRL.Entities;
using Microsoft.AspNetCore.Authorization;





namespace WebAPIMRL.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;

        public ProductsController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // GET: Admin o Vendedor
        [Authorize(Policy = "SalesOrAdmin")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_productoRepository.Listar());
        }

        // GET by Id: Admin o Vendedor
        [Authorize(Policy = "SalesOrAdmin")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var producto = _productoRepository.ObtenerPorId(id);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        // POST: solo Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult Post([FromBody] Producto producto)
        {
            _productoRepository.Guardar(producto, "OpAdd");
            return Ok();
        }

        // PUT: solo Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Producto producto)
        {
            producto.IdProducto = id;
            _productoRepository.Guardar(producto, "OpMod");
            return Ok();
        }

        // DELETE: solo Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _productoRepository.Guardar(new Producto { IdProducto = id }, "OpDel");
            return Ok();
        }
    }
}
