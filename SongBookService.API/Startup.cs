using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using SongBookService.API.DbInitializers.FullSong;
using SongBookService.API.DbInitializers.StructuredSong;
using SongBookService.API.Repository.FullSong;
using SongBookService.API.Repository.FullSsong;
using SongBookService.API.Repository.StructuredSong;

namespace SongBookService.API
{
    public class Startup
    {
        private const string corsPolicy = "CorsPolicy";
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
            services.AddSingleton<IFullSongDbInitializer, SneFullSongsFromXmlInitializer>();
            services.AddSingleton<ISongRepository, MongoSongRepository>();
            services.AddSingleton<IStructuredSongDbInitializer, SneStructuredSongsFromXmlInitializer>();
            services.AddSingleton<IStructuredSongRepository, MongoStructuredSongRepository>();

            services.AddControllers(options =>
                options.SuppressAsyncSuffixInActionNames = false);

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SongBookService.API", Version = "v1" }));

            services.AddCors(options => options.AddPolicy(corsPolicy,
                                  policy => policy.WithOrigins(/*"http://192.168.176.1:4200",*/ "http://localhost:4200")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SongBookService.API v1"));

            app.UseRouting();
            
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints
                => endpoints.MapControllers().RequireCors(corsPolicy));

        }
    }
}
