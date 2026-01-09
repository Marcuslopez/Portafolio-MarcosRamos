using Microsoft.AspNetCore.Mvc;
using PortalWebMRL.Services;
using PortalWebMRL.Models;
using System.Text.Json;
using System.Text;

namespace PortalWebMRL.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApiClient _apiClient;

        public OrdersController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            var client = _apiClient.CreateClientWithJwt(jwt);
                             
            
            var ordenesResp = await client.GetAsync("/api/orders");   // ej: "/api/orders" si así está
            if (!ordenesResp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando órdenes: {(int)ordenesResp.StatusCode}";
                return View(new List<OrdenListViewModel>());
            }

            if (ordenesResp.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                ordenesResp.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Account");
            }



            var json = await ordenesResp.Content.ReadAsStringAsync();



            // llamado de los endpoints api/cliente:
            var clientesResp = await client.GetAsync("/api/cliente");
            if (!clientesResp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando clientes: {(int)clientesResp.StatusCode}";
                return View(new List<OrdenListViewModel>());
            }

            var ordenesJson = await ordenesResp.Content.ReadAsStringAsync();
            var clientesJson = await clientesResp.Content.ReadAsStringAsync();


            // Si tu API devuelve OrdenResponseDto con Detalles, igual funciona: ignoramos Detalles aquí.
            var ordenes = JsonSerializer.Deserialize<List<OrdenListViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<OrdenListViewModel>();

            //Para traer el cliente adjunto a las ordenes
            var clientes = JsonSerializer.Deserialize<List<ClienteOptionViewModel>>(clientesJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ClienteOptionViewModel>();

            // ✅ Diccionario para buscar nombre rápido
            var clientesDict = clientes.ToDictionary(c => c.IdCliente, c => c.Nombre);

            // ✅ Completar ClienteNombre
            foreach (var o in ordenes)
            {
                if (clientesDict.TryGetValue(o.IdCliente, out var nombre))
                    o.ClienteNombre = nombre;
                else
                    o.ClienteNombre = "(No encontrado)";
            }


            return View(ordenes);
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            var client = _apiClient.CreateClientWithJwt(jwt);

            // Traer clientes
            var clientesResp = await client.GetAsync("/api/cliente");
            if (!clientesResp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando clientes: {(int)clientesResp.StatusCode}";
                return View();
            }

            var clientesJson = await clientesResp.Content.ReadAsStringAsync();
            var clientes = JsonSerializer.Deserialize<List<ClienteOptionViewModel>>(
                clientesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ClienteOptionViewModel>();

            // Traer productos
            var productosResp = await client.GetAsync("/api/products");
            if (!productosResp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando productos: {(int)productosResp.StatusCode}";
                ViewBag.Clientes = clientes;
                return View();
            }

            var productosJson = await productosResp.Content.ReadAsStringAsync();
            var productos = JsonSerializer.Deserialize<List<ProductoOptionViewModel>>(
                productosJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ProductoOptionViewModel>();

            ViewBag.Clientes = clientes;
            ViewBag.Productos = productos;

            return View();
        }

        // POST: crea orden en la API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder()
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            // El JS enviará "orderJson" (string) en un input hidden
            var orderJson = Request.Form["orderJson"].ToString();
            if (string.IsNullOrWhiteSpace(orderJson))
            {
                TempData["Error"] = "No se recibió la orden. Intenta nuevamente.";
                return RedirectToAction("Create");
            }

            var client = _apiClient.CreateClientWithJwt(jwt);

            var content = new StringContent(orderJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/orders", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo crear la orden. Código: {(int)response.StatusCode}. Detalle: {body}";
                return RedirectToAction("Create");
            }

            var result = await response.Content.ReadAsStringAsync();
            TempData["Ok"] = $"Orden creada correctamente. Respuesta: {result}";
            return RedirectToAction("Create");
        }


        //Ordenes por ID
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            var http = _apiClient.CreateClientWithJwt(jwt);

            // 1) Traer orden con detalle
            var ordenResp = await http.GetAsync($"/api/orders/{id}");
            if (!ordenResp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error cargando orden: {(int)ordenResp.StatusCode}";
                return View(null);
            }

            var ordenJson = await ordenResp.Content.ReadAsStringAsync();

            // La API te devuelve OrdenResponseDto (IdOrden, IdCliente, FechaOrden, Total, Estado, Detalles)
            var orden = JsonSerializer.Deserialize<OrdenDetailsViewModel>(ordenJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (orden == null)
            {
                ViewBag.Error = "No se pudo leer la orden.";
                return View(null);
            }

            // 2) Traer clientes para resolver el nombre
            var clientesResp = await http.GetAsync("/api/cliente");
            if (clientesResp.IsSuccessStatusCode)
            {
                var clientesJson = await clientesResp.Content.ReadAsStringAsync();
                var clientes = JsonSerializer.Deserialize<List<ClienteOptionViewModel>>(clientesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ClienteOptionViewModel>();

                var cli = clientes.FirstOrDefault(c => c.IdCliente == orden.IdCliente);
                orden.ClienteNombre = cli != null ? cli.Nombre : "(No encontrado)";
            }
            else
            {
                orden.ClienteNombre = "(No disponible)";
            }

            // 3) Traer productos para resolver nombre
            var productosResp = await http.GetAsync("/api/products");
            if (productosResp.IsSuccessStatusCode)
            {
                var productosJson = await productosResp.Content.ReadAsStringAsync();
                var productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(productosJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<ProductoViewModel>();

                var prodDict = productos.ToDictionary(p => p.IdProducto, p => p.Nombre);

                // Completar ProductoNombre en cada detalle
                foreach (var d in orden.Detalles)
                {
                    if (prodDict.TryGetValue(d.IdProducto, out var nombreProd))
                        d.ProductoNombre = nombreProd;
                    else
                        d.ProductoNombre = "(No encontrado)";
                }
            }
            else
            {
                // Si falla productos, al menos no rompemos la vista
                foreach (var d in orden.Detalles)
                    d.ProductoNombre = "(No disponible)";
            }

            return View(orden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            if (!Request.Cookies.TryGetValue("jwt", out var jwt) || string.IsNullOrWhiteSpace(jwt))
                return RedirectToAction("Login", "Account");

            var http = _apiClient.CreateClientWithJwt(jwt);

            var payload = JsonSerializer.Serialize(new { estado = estado });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var resp = await http.PutAsync($"/api/orders/{id}/status", content);

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                resp.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Account");
            }

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo cambiar el estado. Código: {(int)resp.StatusCode}. Detalle: {body}";
                return RedirectToAction("Index");
            }

            TempData["Ok"] = $"Orden #{id} actualizada a estado: {estado}";
            return RedirectToAction("Index");
        }
    }
}
