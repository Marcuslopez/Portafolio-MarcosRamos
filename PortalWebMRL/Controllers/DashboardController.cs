using Microsoft.AspNetCore.Mvc;
using PortalWebMRL.Services;
using System.Text.Json;

namespace PortalWebMRL.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiClient _apiClient;

        public DashboardController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            var http = _apiClient.CreateClientWithJwt(jwt);

            var resp = await http.GetAsync("/api/orders");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando órdenes: {(int)resp.StatusCode}";
                return View();
            }

            var json = await resp.Content.ReadAsStringAsync();
            var orders = JsonDocument.Parse(json).RootElement;

            int creadas = 0, pagadas = 0, anuladas = 0;
            decimal totalVendido = 0;

            foreach (var o in orders.EnumerateArray())
            {
                var estado = o.GetProperty("estado").GetString() ?? "";
                var total = o.TryGetProperty("total", out var t) ? t.GetDecimal() : 0;

                if (estado == "CREADA") creadas++;
                else if (estado == "PAGADA") { pagadas++; totalVendido += total; }
                else if (estado == "ANULADA") anuladas++;
            }

            ViewBag.Creadas = creadas;
            ViewBag.Pagadas = pagadas;
            ViewBag.Anuladas = anuladas;
            ViewBag.TotalVendido = totalVendido;

            return View();
        }
    }
}
