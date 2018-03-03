using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore_keyvault_sample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Security.Cryptography.X509Certificates;

namespace aspnetcore_keyvault_sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Vault()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var cert = await keyVaultClient.GetCertificateAsync(configuration["AppSettings:CertId"]);
            var secret = await keyVaultClient.GetSecretAsync(cert.SecretIdentifier.Identifier);
            var pfxb = Convert.FromBase64String(secret.Value);

            var x509cert = new X509Certificate2(rawData: pfxb,
                password: "",
                keyStorageFlags: X509KeyStorageFlags.MachineKeySet);

            //ViewBag.Thumbprint = x509cert.Thumbprint;

            return Content(x509cert.Thumbprint);
        }
    }
}
