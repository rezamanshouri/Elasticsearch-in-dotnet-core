namespace Elasticsearch.Monitoring
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    using Elasticsearch.Monitoring.Data;
    using Elasticsearch.Utilities.AppSettingsModels;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class MetricsApiAgent : IMetricsApiAgent
    {
        private readonly string metricsApiUrl;

        private readonly IDataPointFactory dataPointFactory;

        private readonly IDataPointTranslator dataPointTranslator;

        private readonly HttpClient httpClient;

        private readonly ILogger logger;

        public MetricsApiAgent(
            ILogger<IMetricsApiAgent> logger,
            IConfiguration config,
            IDataPointFactory dataPointFactory,
            IDataPointTranslator dataPointTranslator,
            HttpClient httpClient,
            MetricsApi metricsApi)
        {
            this.logger = logger;
            this.dataPointTranslator = dataPointTranslator;
            this.dataPointFactory = dataPointFactory;
            this.httpClient = httpClient;
            this.metricsApiUrl = metricsApi.Url;

            // Add API key, etc here read from IConfiguration as an environment variable.
            // this.httpClient.DefaultRequestHeaders.Add("Authorization", $"apikey=\"{config.GetValue<string>("ES_METRICS_API_KEY")}\"");
        }

        public void UploadData<T>(IList<T> metricReports)
            where T : MetricReport
        {
            this.logger.LogInformation($"Starting to upload {metricReports.Count} data points");

            var dataPoints = metricReports.Select(x => this.dataPointFactory.CreateDataPoint(x));
            var content = new StringContent(this.dataPointTranslator.TranslateToLineProtocol(dataPoints), Encoding.UTF8, "text/plain");
            var request = new HttpRequestMessage(HttpMethod.Post, this.metricsApiUrl) { Content = content };
            var response = this.httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            this.logger.LogInformation($"Uploaded {metricReports.Count} data points");
        }
    }
}