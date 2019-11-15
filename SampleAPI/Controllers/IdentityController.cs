using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SampleAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public IdentityController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var client = _clientFactory.CreateClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:44303/");
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var payload = new
            {
                token = accessToken
            };


            var tokenResponse = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "delegation",

                ClientId = "api1.client",
                ClientSecret = "secret",

                Parameters =
                {
                    { "scope", "api2" },
                    { "token", accessToken}
                },
                
            });

            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://localhost:44341/identity");

            return new ContentResult() { Content = await response.Content.ReadAsStringAsync() };
        }
    }
}
