using ClassApplicationMRL.Interfaces;
using ClassDomainMRL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIMRL.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPIMRL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdenService _ordenService;

        public OrdersController(IOrdenService ordenService)
        {
            _ordenService = ordenService;
        }

        // POST: api/orders
        [HttpPost]
        [Authorize(Policy = "SalesOrAdmin")]
        public IActionResult CrearOrden([FromBody] OrdenCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var idOrden = _ordenService.CrearOrden(dto);

            return Ok(new
            {
                message = "👉 Orden creada correctamente...",
                idOrden
            });
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetOrdenes()
        {
            return Ok(_ordenService.ObtenerOrdenes());
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetOrdenPorId(int id)
        {
            var orden = _ordenService.ObtenerOrdenPorId(id);
            if (orden == null) return NotFound();
            return Ok(orden);
        }

        [Authorize]
        [HttpPut("{id}/status")]
        public IActionResult CambiarEstado(int id, [FromBody] OrdenEstadoUpdateDto dto)
        {
            try
            {
                 _ordenService.CambiarEstado(id, dto.Estado);
                return Ok(new { mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
