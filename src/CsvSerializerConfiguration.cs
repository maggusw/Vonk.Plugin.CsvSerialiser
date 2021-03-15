using Vonk.Core.Context;
using Vonk.Core.Pluggability;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vonk.Plugin.CsvSerializer
{
    [VonkConfiguration(order: 5202)]
    public static class CsvSerializerConfiguration
    {
        public static IServiceCollection AddCsvSerializerService(
            this IServiceCollection services)
        {
            services.TryAddSingleton<CsvSerializerService>();
            return services;
        }

        public static IApplicationBuilder Configure(
            IApplicationBuilder builder)
        {
            builder.OnCustomInteraction(VonkInteraction
                    .instance_custom, "csv")
                .AndMethod("GET")
                .HandleAsyncWith<CsvSerializerService>(
                    (svc, context) => svc.GetSearchResult(context));

            builder.OnCustomInteraction(VonkInteraction
                    .type_custom, "csv")
                .AndMethod("GET")
                .HandleAsyncWith<CsvSerializerService>(
                    (svc, context) => svc.GetSearchResult(context));

            return builder;
        }

    }

// this only needs to trigger iff CsvSerializerService is being called
    // i.e. when $csv is being used as query parameter

    [VonkConfiguration(order: 1119)]

    public class CsvSerializerMiddlewareConfiguration
    {
        public static IServiceCollection ConfigureServices
            (IServiceCollection services)
        {
            services.AddSingleton<CsvSerializerService>();
            return services;
        }
        
        public static IApplicationBuilder Configure
            (IApplicationBuilder builder)
        {
            builder.UseMiddleware<CsvSerializerMiddleware>();
            return builder;
        }
    }
}
