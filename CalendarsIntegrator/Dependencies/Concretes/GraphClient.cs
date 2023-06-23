using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CalendarsIntegrator.Dependencies.Concretes
{
    internal class GraphClient : IGraphClient
    {
        private string tenantId;
        private string clientId;
        private string clientSecret;
        private string[] scopes;

        public GraphServiceClient Client { get; set; }

        public GraphClient()
        {



            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.

            //tenantId = configuration["TenantId"];
            //clientId = configuration["ClientId"];
            //clientSecret = configuration["ClientSecret"];
            //scopes = configuration.GetSection("Scopes").Get<string[]>();

            scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "EXAMPLE_KEY3";

            // Values from app registration
            var clientId = "EXAMPLE_KEY1";
            var clientSecret = "EXAMPLE_KEY2";

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            Client = new GraphServiceClient(clientSecretCredential, scopes, "https://graph.microsoft.com/beta");

        }

        public GraphClient(string tenantId, string clientId, string clientSecret)
        {
            this.tenantId = tenantId;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }
    }
}
