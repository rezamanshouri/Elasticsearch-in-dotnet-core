namespace Elasticsearch.Utilities.AppInfo
{
    using System;

    internal class AppInfoService : IAppInfoService
    {
        public string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
    }
}