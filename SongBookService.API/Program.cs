using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;

using SongBookService.API.DbInitializers;

using SongBookService.API.Options;

using SongBookService.API.Repository;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

AddMongoClient(builder.Services, builder.Configuration);
AddOptions(builder.Services, builder.Configuration);
RegisterServices(builder.Services);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = true);

builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SongBookService.API", Version = "v1" }));

var corsPolicy = AddCorsPolicy(builder.Services, builder.Configuration);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => x.TokenValidationParameters = new()
{

});

InitSongDatabase(builder.Services);

var app = builder.Build();

app.Run();

Configure(app, corsPolicy);

static void InitSongDatabase(IServiceCollection services)
{
    var provider = services.BuildServiceProvider();
    var repository = (ISongRepository)provider.GetService(typeof(ISongRepository));
    repository.Initialize();
}

string AddCorsPolicy(IServiceCollection services, IConfiguration configuration)
{
    var cors = configuration.GetSection("CorsPolicy").Get<CorsPolicy>();
    services.AddCors(options => options.AddPolicy(cors.Name,
                           policy => policy.WithOrigins(cors.Origins)));

    return cors.Name;
}

void AddOptions(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<SongRepositoryOptions>(configuration.GetSection(nameof(SongRepositoryOptions)));
}

static void RegisterServices(IServiceCollection services)
{
    services.AddSingleton<ISongDbInitializer, SneSongsFromXmlInitializer>();
    services.AddSingleton<ISongRepository, MongoSongRepository>();
}

void AddMongoClient(IServiceCollection services, IConfiguration configuration)
{
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(configuration.GetConnectionString("SongsDb")));
}

void Configure(IApplicationBuilder app, string corsName)
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
        => endpoints.MapControllers().RequireCors(corsName));

}
