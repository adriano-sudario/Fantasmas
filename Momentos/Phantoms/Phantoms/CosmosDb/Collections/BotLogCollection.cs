using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Phantoms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phantoms.CosmosDb.Collections
{
    public static class BotLogCollection
    {
        private static readonly string collectionId = "BotLogs";

        public static async Task<IEnumerable<PhantomBotLog>> GetAsync(Expression<Func<PhantomBotLog, bool>> predicate = null)
        {
            IQueryable<PhantomBotLog> query = Database.Client.CreateDocumentQuery<PhantomBotLog>(
                UriFactory.CreateDocumentCollectionUri(Database.Id, collectionId), GetFeedOptions());

            if (predicate != null)
                query = query.Where(predicate);

            IDocumentQuery<PhantomBotLog> documentQuery = query.AsDocumentQuery();

            List<PhantomBotLog> results = new List<PhantomBotLog>();
            while (documentQuery.HasMoreResults)
                results.AddRange(await documentQuery.ExecuteNextAsync<PhantomBotLog>());

            return results;
        }

        public static async Task<Document> CreateAsync(PhantomBotLog phantomBotLog)
        {
            return await Database.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Database.Id, collectionId), phantomBotLog, options: GetRequestOptions());
        }

        public static async Task DeleteAsync(IEnumerable<PhantomBotLog> phantomBotLogs)
        {
            foreach (PhantomBotLog phantomBotLog in phantomBotLogs)
                await DeleteAsync(phantomBotLog.Id);
        }

        public static async Task DeleteAsync(PhantomBotLog phantomBotLog)
        {
            await DeleteAsync(phantomBotLog.Id);
        }

        public static async Task DeleteAsync(string id)
        {
            await Database.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(Database.Id, collectionId, id), options: GetRequestOptions());
        }

        private static FeedOptions GetFeedOptions()
        {
            return new FeedOptions { MaxItemCount = -1, JsonSerializerSettings = GetJsonSerializerSettings() };
        }

        private static RequestOptions GetRequestOptions()
        {
            return new RequestOptions { JsonSerializerSettings = GetJsonSerializerSettings() };
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
        }
    }
}
