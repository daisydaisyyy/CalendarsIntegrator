using Azure.Identity;
using Microsoft.Graph;


namespace CalendarsIntegrator.Dependencies.Concretes
{
    internal class GraphClient : IGraphClient
    {
      

        public GraphServiceClient Client { get; set; }

        public GraphClient(string tenantId, string clientId, string clientSecret, string[] scopes)
        {



            // The client credentials flow requires that you request the
            // /.default scope, and preconfigure your permissions on the
            // app registration in Azure. An administrator must grant consent
            // to those permissions beforehand.

            //scopes = new[] { "https://graph.microsoft.com/.default" };

            //IDs USED IN THE FIRST VERSION OF THE PROGRAM, PRECAUTION


            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            /* var tenantId = "EXAMPLE_KEY3";

             // Values from app registration
             var clientId = "EXAMPLE_KEY1";
             var clientSecret = EXAMPLE_KEY2


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

    }
}