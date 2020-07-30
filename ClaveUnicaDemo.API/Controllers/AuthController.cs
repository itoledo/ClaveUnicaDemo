using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ClaveUnicaDemo.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ClaveUnicaDemo.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        const string callbackScheme = "claveunicademo";
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("claveunica")]
        public async Task Get()
        {
            var auth = await Request.HttpContext.AuthenticateAsync("OpenIdConnect");

            if (!auth.Succeeded
                || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
            {
                // Not authenticated, challenge
                await Request.HttpContext.ChallengeAsync("OpenIdConnect");
            }
            else
            {
                //_logger.LogInformation($"principal: {0}", JsonConvert.SerializeObject(auth.Principal));
                foreach (var pr in auth.Principal.Identities)
                {
                    _logger.LogInformation("identity: {0} -> {1} -> {2} -> {3}", pr.Label, pr.NameClaimType, pr.Name, pr.IsAuthenticated);

                    foreach (var cl in pr.Claims)
                    {
                        _logger.LogInformation("claim: {0}-{1}-{2}-{3}", cl.Type, cl.Value, cl.ValueType, cl.Issuer);
                        foreach (var pro in cl.Properties)
                            _logger.LogInformation("prop: {0} -> {1}", pro.Key, pro.Value);
                    }
                }

                foreach (var tok in auth.Properties.GetTokens())
                    _logger.LogInformation($"tok: {tok.Name} -> {tok.Value}");

                string run = string.Empty;
                string name = auth.Principal.FindFirstValue("name");
                string email = auth.Principal.FindFirstValue("email");

                if (!string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
                {
                    try
                    {
                        // obtengamos el userinfo
                        using (var cl = new HttpClient())
                        {
                            cl.DefaultRequestHeaders.Authorization =
                                                new AuthenticationHeaderValue("Bearer", auth.Properties.GetTokenValue("access_token"));
                            var ret = await cl.PostAsync("https://www.claveunica.gob.cl/openid/userinfo/", new StringContent(string.Empty));
                            ret.EnsureSuccessStatusCode();
                            var json = await ret.Content.ReadAsStringAsync();
                            _logger.LogInformation("respuesta userinfo: {0}", json);
                            var ui = JsonConvert.DeserializeObject<UserInfo>(json);
                            if (ui?.RolUnico != null)
                                run = $"{ui.RolUnico.Numero}-{ui.RolUnico.DV}";
                            if (ui?.Name != null)
                                name = string.Join(" ", ui.Name.Nombres) + " " + string.Join(" ", ui.Name.Apellidos);
                            if (!string.IsNullOrEmpty(ui?.Email))
                                email = ui?.Email;
                        }
                    } catch (Exception e)
                    {
                        _logger.LogError(e, "excepción al obtener UserInfo");
                    }
                }

                var qs = new Dictionary<string, string>
                {
                    { "access_token", auth.Properties.GetTokenValue("access_token") },
                    { "refresh_token", auth.Properties.GetTokenValue("refresh_token") ?? string.Empty },
                    { "expires", (auth.Properties.ExpiresUtc?.ToUnixTimeSeconds() ?? -1).ToString() },
                    { "run", run },
                    { "name",  name },
                    { "email", email },
                };

                // Build the result url
                var url = callbackScheme + "://#" + string.Join(
                    "&",
                    qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                // Redirect to final url
                Request.HttpContext.Response.Redirect(url);
            }
        }

        [HttpGet("claveunicalogout")]
        public async Task Logout()
        {
            await Request.HttpContext.SignOutAsync("Cookies");
            // Build the result url
            var url = callbackScheme + "://";
            await Request.HttpContext.SignOutAsync("OpenIdConnect");
            //Redirect to final url
            Request.HttpContext.Response.Redirect(url);
        }
    }
}