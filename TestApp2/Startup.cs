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
            services.AddScoped<IBinaryLoader, BinaryLoader>();
            services.AddScoped<IExperimentalBinaryLoader, ExperimentalBinaryLoader>();
            services.AddScoped<IConverterService, ConverterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache cache, IBinaryLoader binaryLoader, IExperimentalBinaryLoader experimentalBinaryLoader)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            _memoryCache = cache;
            app.UseResponseCaching();
            
            app.UseMvc();
            app.UseStaticFiles();

            LoadGeoBase(binaryLoader);
            LoadBinaryGeoBase(experimentalBinaryLoader);

        }

        public void LoadBinaryGeoBase(IExperimentalBinaryLoader experimentalBinaryLoader){

            var db = new BinaryGeoBase();

            if (!_memoryCache.TryGetValue(CacheConstants.BinaryGeoBaseKey, out db))
            {
                //var loader = new BinaryLoader();
                db = experimentalBinaryLoader.ReadBinaryFileToByteArray(@"D:\Work\geobase.dat");
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                _memoryCache.Set(CacheConstants.BinaryGeoBaseKey, db, cacheEntryOptions);
            }

        }

        public void LoadGeoBase(IBinaryLoader binaryLoader){
            var db = new GeoBase();

            if (!_memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out db))
            {
                //var loader = new BinaryLoader();
                db = binaryLoader.LoadDb(@"D:\Work\geobase.dat");
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
                _memoryCache.Set(CacheConstants.GeoBaseKey, db, cacheEntryOptions);
            }
        }
    }
}
