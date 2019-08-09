using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Controller
{
    [Route("/es")]
    public class ElasticSearchController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("index", Name = "Index")]
        [HttpGet]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult Index()
        {
            return Ok(true);
        }

        [Route("search/{query}", Name = "Search")]
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Search(string query)
        {
            // TODO: this should be a POST
            return Ok(query);
        }
    }
}