using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using MongoDB.Driver;

namespace DocNuget {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var connection = config.Get("DOCNUGET_DATABASE");
            if (connection != null) {
                services.AddInstance(new MongoClient(connection).GetDatabase("DocNuget"));
            }

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(LogLevel.Debug);

            app
                .UseErrorPage()
                .UseDefaultFiles()
                .UseStaticFiles(new StaticFileOptions {
                    ServeUnknownFileTypes = true,
                })
                .UseMvc()
                .Use((context, next) => {
                    context.Request.Path = new PathString("/index.html");
                    return next();
                })
                .UseStaticFiles();
        }
    }
}
