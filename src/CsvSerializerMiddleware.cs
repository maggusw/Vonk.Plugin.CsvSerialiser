using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Serilog;
using static Vonk.Plugin.CsvSerializer.utils.Utils;
using Vonk.Core.Context.Http;
using Task = System.Threading.Tasks.Task;

namespace Vonk.Plugin.CsvSerializer
{
    internal class CsvSerializerMiddleware
    {
        private readonly RequestDelegate _next;

        public CsvSerializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task SetError(HttpContext httpContext,
            Exception exception)
        {
            _ = await Task.FromResult(true);
            
            var contentLength = Encoding.UTF8.GetByteCount
                (exception.Message).ToString();

            httpContext.Response.Headers.Add("Content-Type",
                "text/csv");
            httpContext.Response.Headers.Add
                ("Content-Length", contentLength);

            httpContext.Response.StatusCode = 404;
            
            await httpContext.Response.WriteAsync(exception
                .Message);

        }

        public async Task Invoke(HttpContext httpContext,
            CsvSerializerService service)
        {
            var path = httpContext.Request.Path.Value;
            var method = httpContext.Request.Method;
            var queryString =
                httpContext.Request.QueryString.ToString();
            if (path.StartsWith("/$csv"))
            {
                await SetError(httpContext, new 
                    ArgumentException("Operation too costly. " +
                                      "Please specify a less broad query."));
            } else if ((path.EndsWith("csv") || queryString.Contains
              ("csv") || queryString.Contains("%24csv"))
               && method ==
               "GET")
            {
                Log.Information("Started Invocation");

                try
                {
                    var searchResult = await service.GetSearchResult(
                        httpContext
                            .Vonk());

                    if (path.Contains("?meta"))
                    {
                        // ToDo: handle bundle differently
                    }

                    var bundle = await CreateBundleFromSearchResult
                        (searchResult);
                    var dictList =
                        await CreateDictListFromBundle(bundle);
    
                    var keyList = await CreateDistinctKeyListFromDicts
                        (dictList);
                    
                    var csv =
                        await CreateCsvFromLists(dictList, keyList);
                    
                    var contentLength =
                        Encoding.UTF8.GetByteCount(csv)
                            .ToString();

                    httpContext.Response.Headers.Add("Content-Type",
                        "text/csv");
                    httpContext.Response.Headers.Add("Content-Length",
                        contentLength);

                    httpContext.Response.StatusCode = 200;

                    await httpContext.Response.WriteAsync(csv);

                    Log.Information("Invocation handled");

                }
                catch (NullReferenceException exception)
                {
                    await SetError(httpContext, exception);
                }
                catch (ArgumentException exception)
                {
                    await SetError(httpContext, exception);
                }
                
            }
            else
            {
                await _next(httpContext);
            }

        }
    }
}