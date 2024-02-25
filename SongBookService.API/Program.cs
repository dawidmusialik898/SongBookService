using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using SongBookService.API.DbInitializers;
using SongBookService.API.Identity;
using SongBookService.API.Options;
using SongBookService.API.Repository;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

AddMongoClient(builder, config);
AddOptions(builder, config);
AddAuthentication(builder,config);
AddCorsPolicy(builder, config);
RegisterServices(builder);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

Configure(app);

app.Run();

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
    var mongoConnectionString = configuration.GetConnectionString("SongsDb");
    builder.Services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(configuration.GetConnectionString("SongsDb")));
}

static void AddAuthentication(WebApplicationBuilder builder, IConfiguration configuration)
{
    var sqlConnectionString = configuration.GetConnectionString("UsersDb");
    builder.Services.AddDbContext<UserDbContext>(
        options => options.UseSqlServer(configuration.GetConnectionString("UsersDb")));

    builder.Services.AddIdentityCore<SongServiceUser>()
        .AddEntityFrameworkStores<UserDbContext>()
        .AddApiEndpoints();

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();
}

static void Configure(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();

        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapIdentityApi<SongServiceUser>();

    app.MapControllers();
}