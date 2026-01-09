using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalWebMRL.Models;
using System.Text;
using System.Text.Json;

namespace PortalWebMRL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/auth/login";

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                 ViewBag.Error = "Credenciales incorrectas";
                 
                //var error = await response.Content.ReadAsStringAsync();
                //ViewBag.Error = $"Error API: {response.StatusCode}";
                return View(model);

            }

            var result = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(result);
            var token = doc.RootElement.GetProperty("token").GetString();

            // 🔐 Guardar JWT en Cookie segura
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}
