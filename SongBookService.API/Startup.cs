using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using SongBookService.API.DbInitializers;
using SongBookService.API.Repository;

namespace SongBookService.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            var connectionString = Configuration.GetConnectionString("Default");

            services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(connectionString));
            services.AddSingleton<IDbInitializer, SneSongsFromXmlInitializer>();
            services.AddSingleton<ISongRepository, MongoSongRepository>();

            services.AddControllers(options =>
                options.SuppressAsyncSuffixInActionNames = false);

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SongBookService.API", Version = "v1" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SongBookService.API v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints
                => endpoints.MapControllers());
        }
    }
}
