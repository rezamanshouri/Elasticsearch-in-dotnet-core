namespace Elasticsearch.Utilities.AppSettingsModels
{
    using System.ComponentModel.DataAnnotations;

    public class ElasticConnection
    {
        [Required]
        public string Url { get; set; }
    }
}