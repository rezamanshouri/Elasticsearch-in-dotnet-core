namespace Elasticsearch.Controller
{
    using System;

    using Elasticsearch.Search;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/")]
    [ApiController]
    public class ElasticsearchController : ControllerBase
    {
        private readonly ILogger logger;

        private readonly ISearchService searchService;

        public ElasticsearchController(ILogger<ElasticsearchController> logger, ISearchService searchService)
        {
            this.logger = logger;
            this.searchService = searchService;
        }

        /// <summary>
        ///     Returns entities matching a full-text search for the given search term
        /// </summary>
        /// <remarks>
        ///     This endpoint will return the 100 highest-scoring results of any type.
        /// </remarks>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("search/{query}", Name = "Search")]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult Search(string query)
        {
            try
            {
                return string.IsNullOrWhiteSpace(query)
                           ? this.StatusCode(StatusCodes.Status400BadRequest, "The search query must contain some characters.")
                           : this.Ok(this.searchService.Search(query));
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "The search failed because of an internal error.");
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}