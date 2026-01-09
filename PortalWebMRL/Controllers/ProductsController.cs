using Microsoft.AspNetCore.Mvc;
using PortalWebMRL.Models;
using PortalWebMRL.Services;
using System.Text;
using System.Text.Json;

namespace PortalWebMRL.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApiClient _apiClient;

        public ProductsController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // ✅ Categorías (hardcode por ahora)
        private static List<CategoriaOption> GetCategorias()
        {
            return new List<CategoriaOption>
            {
                new CategoriaOption { IdCategoria = 1, Nombre = "Computadoras y Laptops" },
                new CategoriaOption { IdCategoria = 2, Nombre = "Periféricos" },
                new CategoriaOption { IdCategoria = 3, Nombre = "Almacenamiento y Memoria" },
                new CategoriaOption { IdCategoria = 4, Nombre = "Audio y Video" },
                new CategoriaOption { IdCategoria = 5, Nombre = "Electrodomésticos" },
                new CategoriaOption { IdCategoria = 6, Nombre = "Energía y Respaldo" }
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1) JWT
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            // 2) Cargar categorías para el modal
            ViewBag.Categorias = GetCategorias();

            // 3) Llamar API
            var client = _apiClient.CreateClientWithJwt(jwt);
            var response = await client.GetAsync("/api/products");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error consultando productos: {(int)response.StatusCode}";
                return View(new List<ProductoViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ProductoViewModel>();

            return View(productos);
        }

        // ✅ POST: /Products/Create  (desde el modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoCreateViewModel model)
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
                descripcion = model.Descripcion,
                precio = model.Precio,
                stock = model.Stock,
                idCategoria = model.IdCategoria
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/products", content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo crear el producto. Código: {(int)response.StatusCode}. Detalle: {body}";
                return RedirectToAction("Index");
            }

            TempData["Ok"] = "Producto creado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
