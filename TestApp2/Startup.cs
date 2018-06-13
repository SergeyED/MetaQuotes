using MetaQuotes.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MetaQuotes.WebApp
{
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
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddRouting();
            services.AddMvc();

            services.AddTransient<IFileReadService, FileReadService>();
            services.AddTransient<ISearchService, SearchService>();
            services.AddSingleton<IRepository, Repository>();
            services.AddTransient<IConverterService, ConverterService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            //_memoryCache = cache;
            //app.UseResponseCaching();
            
            app.UseMvc();
            app.UseStaticFiles();


            var repository = app.ApplicationServices.GetService<IRepository>();
            repository.Load(@"/Users/cepega/Documents/geobase.dat");
        }
    }
}
