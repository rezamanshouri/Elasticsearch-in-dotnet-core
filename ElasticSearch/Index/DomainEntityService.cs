namespace Elasticsearch.Index
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Elasticsearch.Index.Entities;
    using Elasticsearch.Utilities.AppSettingsModels;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public class DomainEntityService : IDomainEntityService
    {
        private readonly HttpClient httpClient;

        private readonly ILogger logger;

        private readonly string entitiesBaseUrl;

        /*
         * This class is responsible to retrieve the list of entities either from your database or via an API call to be indexed.
         */
        public DomainEntityService(ILogger<IDomainEntityService> logger, HttpClient httpClient, EntitiesApi entitiesApi)
        {
            this.logger = logger;
            this.httpClient = httpClient;

            this.entitiesBaseUrl = entitiesApi.Url;
        }

        public List<Person> GetPeople()
        {
            return this.GetEntities<Person>("people-relative-path");
        }

        private List<T> GetEntities<T>(string entityPath)
        {
            this.logger.LogInformation($"Fetching '{typeof(T)}' from Api");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, this.GetFullUrl(entityPath));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = this.httpClient.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<List<T>>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                // Failing to get one entity should not halt indexing other types.
                this.logger.LogError(e, $"Fetching '{typeof(T)}' from Api failed");
                throw;
            }
        }

        private string GetFullUrl(string entityPath)
        {
            return $"{this.entitiesBaseUrl}/{entityPath}";
        }
    }
}