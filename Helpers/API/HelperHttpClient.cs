using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers.API
{
    public class HelperHttpClient
    {
        public static string Token { get; set; }
        //Get from credit safe
        public static int PortfolioId = 1681868;
        public static HttpClient HttpClient { get; set; }
        public static async Task Authenticate(HttpClient httpClient)
        {
            var loginVm = new HelperApiLoginVm();
            using var login = await httpClient.PostAsJsonAsync("authenticate", loginVm);
            var jwtTokenViewModel = await login.Content.ReadFromJsonAsync<HelperJwtTokenVm>();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtTokenViewModel.Token);
            Token = jwtTokenViewModel.Token;
            HttpClient = httpClient;
        }
    }
}
