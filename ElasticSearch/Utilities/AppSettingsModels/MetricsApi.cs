namespace Elasticsearch.Utilities.AppSettingsModels
{
    using System.ComponentModel.DataAnnotations;

    public class MetricsApi
    {
        [Required]
        public string Url { get; set; }
    }
}