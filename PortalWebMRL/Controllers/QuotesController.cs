using Microsoft.AspNetCore.Mvc;
using PortalWebMRL.Models;
using PortalWebMRL.Services;
using System.Text.Json;
using System.Text;

namespace PortalWebMRL.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApiClient _apiClient;
        public QuotesController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            var client = _apiClient.CreateClientWithJwt(jwt);
            // Traer productos
            var productosResp = await client.GetAsync("/api/products");
            if (!productosResp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando productos: {(int)productosResp.StatusCode}";                
                return View();
            }

            var productosJson = await productosResp.Content.ReadAsStringAsync();
            var productos = JsonSerializer.Deserialize<List<ProductoOptionViewModel>>(
                productosJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ProductoOptionViewModel>();
            
            ViewBag.Productos = productos;
           

            return View();
        }
    }
}
