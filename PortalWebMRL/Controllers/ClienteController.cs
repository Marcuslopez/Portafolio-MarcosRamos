using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalWebMRL.Models;
using PortalWebMRL.Services;
using System.Text;
using System.Text.Json;

namespace PortalWebMRL.Controllers
{

    public class ClienteController : Controller
    {

        private readonly ApiClient _apiClient;

        public ClienteController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: ClienteController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1) Leer JWT desde cookie
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            // 2) Llamar API con Bearer token
            var client = _apiClient.CreateClientWithJwt(jwt);

            var response = await client.GetAsync("/api/cliente"); // ajusta si tu ruta es distinta

            // 3) Si token expiró o inválido → volver a login
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Account");
            }

            // 4) Si hubo otro error, mostrar mensaje
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error consultando los clientes: {(int)response.StatusCode}";
                return View(new List<ClienteViewModel>());
            }

            // 5) Deserializar JSON
            var json = await response.Content.ReadAsStringAsync();

            var clientes = JsonSerializer.Deserialize<List<ClienteViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ClienteViewModel>();

            return View(clientes);
        }



        // ✅ POST: /Cliente/Create  (desde el modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel model)
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos. Verifica el formulario.";
                return RedirectToAction("Index");
            }

            var client = _apiClient.CreateClientWithJwt(jwt);

            // ✅ Payload incluyendo IdCategoria
            var payload = JsonSerializer.Serialize(new
            {
                nombre = model.Nombre,
                email = model.Email,
                telefono = model.Telefono
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/cliente", content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo crear datos del cliente. Código: {(int)response.StatusCode}. Detalle: {body}";
                return RedirectToAction("Index");
            }

            TempData["Ok"] = "El cliente a sido creado correctamente.";
            return RedirectToAction("Index");
        }



    }
}
