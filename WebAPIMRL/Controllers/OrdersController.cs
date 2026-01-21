using ClassApplicationMRL.Interfaces;
using ClassDomainMRL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIMRL.Dtos;
using WebAPIMRL.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPIMRL.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private object GetActor()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            var Nombre = User.FindFirstValue(ClaimTypes.Name);
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var Rol = User.FindFirstValue(ClaimTypes.Role);

            return new { UserId, Nombre, Email, Rol };

        }

        private readonly IOrdenService _ordenService;

        private readonly RealtimeNotifier _notifier;
        public OrdersController(IOrdenService ordenService, RealtimeNotifier notifier)
        {
            _ordenService = ordenService;
            _notifier = notifier;
        }


        // POST: api/orders
        [HttpPost]
        [Authorize(Policy = "SalesOrAdmin")]

 
        public async Task<IActionResult> CrearOrden([FromBody] OrdenCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var idOrden = _ordenService.CrearOrden(dto);



            await _notifier.NotifyAsync(
                        type: "order_created",
                        message: $"Nueva orden creada #{idOrden}",
                        data: new
                      {
                        severity = "success",
                        entity = "order",
                        entityId = idOrden,
                        actor = GetActor(), 
                        clienteId = dto.IdCliente,                        
                        total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
                        items = dto.Detalles.Count
                      }
                    );




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
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] OrdenEstadoUpdateDto dto)
        {
            await _notifier.NotifyAsync(
                    type: "order_status",
                    message: $"Orden #{id} cambió a {dto.Estado}",
                    data: new
                { 
                    severity = dto.Estado == "PAGADA" ? "success" : "warning",
                    entity = "order",
                    entityId = id,
                    actor = GetActor(),
                    estado = dto.Estado
                }
                );
            
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
