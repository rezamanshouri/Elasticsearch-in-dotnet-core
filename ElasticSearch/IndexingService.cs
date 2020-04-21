namespace Elasticsearch
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Elasticsearch.Index;
    using Elasticsearch.Utilities.AppSettingsModels;
    using Elasticsearch.Utilities.Lock;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    
    using NLog;

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class IndexingService : IHostedService, IDisposable
    {
        private readonly int IndexingFrequencySeconds;

        private readonly IIndexService indexService;

        private readonly ILockWork lockWork;

        private readonly ILogger logger;

        private Timer timer;

        public IndexingService(
            ILogger<IndexingService> logger,
            IIndexService indexService,
            ILockWork lockWork,
            IndexingSettings indexingSettings)
        {
            this.logger = logger;
            this.indexService = indexService;
            this.lockWork = lockWork;

            this.IndexingFrequencySeconds = indexingSettings.IndexingFrequencySeconds;
        }

        public void Dispose()
        {
            this.timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation($"Indexing background task is starting. The frequency is {this.IndexingFrequencySeconds} seconds.");

            this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(this.IndexingFrequencySeconds));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Indexing background task is stopping.");

            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                MappedDiagnosticsLogicalContext.Set("chainId", Guid.NewGuid().ToString());
                if (!this.lockWork.DoWork(() => this.indexService.IndexAll()))
                {
                    this.logger.LogInformation("The indexing background task skipped since the previous task is still running.");
                }
            }
            catch (Exception e)
            {
                // The background task should never throw.
                this.logger.LogError(e, "The indexing background task failed.");
            }
            finally
            {
                MappedDiagnosticsLogicalContext.Clear();
            }
        }
    }
}