using Azure.Core;
using ClassDataMRL.Interfaces;
using ClassDataMRL.Repositories;
using ClassDomainMRL.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;


namespace WebAPIMRL.Services
{
    public class RealtimeNotifier
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public RealtimeNotifier(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task NotifyAsync(string type, string message, object? data = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();                              

                var payload = new
                {
                    type,
                    message,
                    data
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.PostAsync("http://localhost:4000/events", content);
            }
            catch
            {
                // ✅ No rompemos la API si Node no está levantado
                // (opcional: loggear aquí)
            }
        }
    }
}
