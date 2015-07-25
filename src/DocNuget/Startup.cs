using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;

namespace DocNuget {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            services.ConfigureMvcJson(options => {
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(LogLevel.Debug);
            app.UseErrorPage();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions {
                ServeUnknownFileTypes = true,
            });
            app.UseMvc();
        }
    }
}
