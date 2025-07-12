using Microsoft.Extensions.Configuration;

namespace DocIntelAnalyzer.Configuration
{
    public class AppConfiguration
    {
        public DocumentIntelligenceConfig DocumentIntelligence { get; set; } = new DocumentIntelligenceConfig();
    }

    public class DocumentIntelligenceConfig
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public static class ConfigurationHelper
    {
        public static AppConfiguration LoadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var config = new AppConfiguration();
            
            // Manual binding instead of using Bind extension
            var diSection = configuration.GetSection("DocumentIntelligence");
            config.DocumentIntelligence.Endpoint = diSection["Endpoint"] ?? string.Empty;
            config.DocumentIntelligence.ApiKey = diSection["ApiKey"] ?? string.Empty;
            
            return config;
        }
    }
}
