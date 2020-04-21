namespace ElasticSearch
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json.Serialization;
    using Microsoft.Extensions.Hosting;

    using Elasticsearch.Index;
    using Elasticsearch.Monitoring;
    using Elasticsearch.Monitoring.Data;
    using Elasticsearch.Search;
    using Elasticsearch.Utilities.AppInfo;
    using Elasticsearch.Utilities.AppSettingsModels;
    using Elasticsearch.Utilities.AppSettingsModels.Validation;
    using Elasticsearch.Utilities.ElasticClientProvider;
    using Elasticsearch.Utilities.Lock;
    using Elasticsearch.Utilities.Time;
    using Elasticsearch;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddLogging();

            // register typed clients.
            // see https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<IDomainEntityService, DomainEntityService>();
            services.AddHttpClient<IMetricsApiAgent, MetricsApiAgent>();

            // Register API related interfaces.
            services.AddScoped<ISearchService, SearchService>();

            // Register background task services which is transient under the hood.
            // https://github.com/aspnet/Hosting/blob/master/src/Microsoft.Extensions.Hosting.Abstractions/ServiceCollectionHostedServiceExtensions.cs#L18
            services.AddHostedService<IndexingService>();

            // Register background task related interfaces as transient because In .NET Core 2.1 the AddHostedService is transient under the hood:
            services.AddTransient<IIndexService, IndexService>();
            services.AddTransient<IElasticClientService, ElasticClientService>();
            services.AddTransient<IElasticClientProvider, ElasticClientProvider>();
            services.AddTransient<IElasticClientFactory, ElasticClientFactory>();
            services.AddTransient<IDataPointTranslator, DataPointTranslator>();
            services.AddTransient<IDataPointFactory, DataPointFactory>();
            services.AddTransient<IClock, Clock>();
            services.AddTransient<IAppInfoService, AppInfoService>();

            // The Lock class has to be singleton.
            services.AddSingleton<ILockWork, LockWork>();

            this.AddConfigurationModels(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Matches request to an endpoint.
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddConfigurationModels(IServiceCollection services)
        {
            // We avoid using IOptions here so we are able to validate configuration models using annotations.
            services.AddSingleton(this.Configuration.GetSection("ElasticConnection").GetValid<ElasticConnection>());
            services.AddSingleton(this.Configuration.GetSection("EntitiesApi").GetValid<EntitiesApi>());
            services.AddSingleton(this.Configuration.GetSection("IndexingSettings").GetValid<IndexingSettings>());
            services.AddSingleton(this.Configuration.GetSection("MetricsApi").GetValid<MetricsApi>());
        }
    }
}