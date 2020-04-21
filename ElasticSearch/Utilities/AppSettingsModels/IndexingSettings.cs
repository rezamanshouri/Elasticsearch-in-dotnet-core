namespace Elasticsearch.Utilities.AppSettingsModels
{
    using System.ComponentModel.DataAnnotations;

    public class IndexingSettings
    {
        [Range(0, 172800, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int IndexingFrequencySeconds { get; set; }

        [Range(500, 10000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int MaxDocumentsPerBatch { get; set; }
    }
}