using Microsoft.Azure.Documents.Client;
using System;

namespace Phantoms.CosmosDb
{
    public static class Database
    {
        public static readonly string Id = "Phantoms";
        public static DocumentClient Client { get; private set; }

        static Database()
        {
            string endPoint = "https://localhost:8081/";
            string authKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            Client = new DocumentClient(new Uri(endPoint), authKey, new ConnectionPolicy { EnableEndpointDiscovery = false });
        }
    }
}
