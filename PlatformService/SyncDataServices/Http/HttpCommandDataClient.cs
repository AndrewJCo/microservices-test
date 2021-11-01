using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using Microsoft.Extensions.Configuration;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync($"{configuration["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("=== Sync post to commandservice was ok");
            else
                Console.WriteLine("=== Sync post to commandservice not ok");
        }
    }
}