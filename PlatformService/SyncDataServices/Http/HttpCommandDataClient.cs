using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config["CommandServicebaseUrl"]);
        }

        public async Task SendPlatformToCommand(PlatformReadDto dto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, mediaType: "application/json");
            var response = await _httpClient.PostAsync("/api/c/platforms", content);
            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> sync POST to command service was ok");
            }
            else{
                Console.WriteLine("--> Sync post to Command service was not ok");
            }
        }
    }
}