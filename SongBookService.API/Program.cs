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
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SongBookService.API.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authentication.BearerToken;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

AddMongoClient(builder, config);
AddOptions(builder, config);
AddAuthentication(builder);
AddCorsPolicy(builder, config);

RegisterServices(builder);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = true);

builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SongBookService.API", Version = "v1" }));

InitSongDatabase(builder);

var app = builder.Build();

Configure(app);

app.Run();

static void InitSongDatabase(WebApplicationBuilder builder)
{
    var provider = builder.Services.BuildServiceProvider();
    var repository = (ISongRepository)provider.GetService(typeof(ISongRepository));
    repository.Initialize();
}

void AddCorsPolicy(WebApplicationBuilder builder, IConfiguration configuration)
{
    var cors = configuration.GetSection(nameof(CorsPolicy)).Get<CorsPolicy>();
    builder.Services.AddCors(options => options.AddPolicy(cors.Name,
                           policy => policy.WithOrigins(cors.Origins)));
}

void AddOptions(WebApplicationBuilder builder, IConfiguration configuration) =>
    builder.Services.Configure<SongRepositoryOptions>(configuration.GetSection(nameof(SongRepositoryOptions)));

static void RegisterServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<ISongDbInitializer, SneSongsFromXmlInitializer>();
    builder.Services.AddSingleton<ISongRepository, MongoSongRepository>();
}

void AddMongoClient(WebApplicationBuilder builder, IConfiguration configuration)
{
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    builder.Services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(configuration.GetConnectionString("SongsDb")));
}

static void AddAuthentication(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie();

    builder.Services.AddAuthorizationBuilder();

    builder.Services.AddDbContext<UserDbContext>(
        options => options.UseInMemoryDatabase("UserDb"));

    builder.Services.AddIdentityCore<User>()
        .AddEntityFrameworkStores<UserDbContext>()
        .AddApiEndpoints();
}

static void Configure(IApplicationBuilder app)
{
    app.UseHttpsRedirection();

    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SongBookService.API v1"));

    app.UseRouting();

    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseEndpoints(x =>
    {
        x.MapIdentityApi<User>();
        x.MapControllers();
    });
}