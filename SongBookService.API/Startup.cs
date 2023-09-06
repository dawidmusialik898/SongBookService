using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using SongBookService.API.Options;
using Microsoft.AspNetCore.Routing;

namespace SongBookService.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }
        private Options.CorsPolicy cors;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddMongoClient(services);
            AddOptions(services);
            RegisterServices(services);

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = true);

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SongBookService.API", Version = "v1" }));

            AddCorsPolicy(services);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => x.TokenValidationParameters = new()
            {

            });

            InitSongDatabase(services);
        }

        private static void InitSongDatabase(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var repository = (ISongRepository)provider.GetService(typeof(ISongRepository));
            repository.Initialize();
        }

        private void AddCorsPolicy(IServiceCollection services)
        {
            cors = Configuration.GetSection("CorsPolicy").Get<Options.CorsPolicy>();
            services.AddCors(options => options.AddPolicy(cors.Name,
                                   policy => policy.WithOrigins(cors.Origins)));
        }

        private void AddOptions(IServiceCollection services)
        {
            services.Configure<SongRepositoryOptions>(Configuration.GetSection(nameof(SongRepositoryOptions)));
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ISongDbInitializer, SneSongsFromXmlInitializer>();
            services.AddSingleton<ISongRepository, MongoSongRepository>();
        }

        private void AddMongoClient(IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(Configuration.GetConnectionString("SongsDb")));
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

            //app.UseAuthentication();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints
                => endpoints.MapControllers().RequireCors(cors.Name));

        }
    }
}
