namespace Elasticsearch.Utilities.AppSettingsModels
{
    using System.ComponentModel.DataAnnotations;

    public class EntitiesApi
    {
        [Required]
        public string Url { get; set; }
    }
}