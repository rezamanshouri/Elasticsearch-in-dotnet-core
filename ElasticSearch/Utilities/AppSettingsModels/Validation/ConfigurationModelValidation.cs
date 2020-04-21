namespace Elasticsearch.Utilities.AppSettingsModels.Validation
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.Extensions.Configuration;

    public static class ConfigurationModelValidation
    {
        public static T GetValid<T>(this IConfiguration configuration)
        {
            var obj = configuration.Get<T>();
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
            return obj;
        }
    }
}