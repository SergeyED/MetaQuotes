using MetaQuotes.Models;
using MetaQuotes.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MetaQuotes.Models.Version2;

namespace MetaQuotes.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IMemoryCache _memoryCache;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddRouting();
            services.AddMvc();

            services.AddScoped<ISearchService, SearchService>();
            services.AddSingleton<IRepository, Repository>();
            services.AddScoped<IConverterService, ConverterService>();

            //services.AddSingleton<IBinaryGeoBase, BinaryGeoBase>(service => {
            //    var db = service.GetService<IExperimentalBinaryLoader>().ReadBinaryFileToByteArray(@"/Users/cepega/Documents/geobase.dat");
            //    return db;

            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache cache, IRepository experimentalBinaryLoader)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            _memoryCache = cache;
            app.UseResponseCaching();
            
            app.UseMvc();
            app.UseStaticFiles();

            experimentalBinaryLoader.Load(@"/Users/cepega/Documents/geobase.dat");
        }
    }
}
