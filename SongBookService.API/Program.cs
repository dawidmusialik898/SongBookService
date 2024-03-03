using System.Threading.Tasks;

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

SetupServices(builder, config);

var app = builder.Build();

await CreateSuperUser(app, config);
await InitializeSongDatabase(app, config);

SetupMiddleware(app);

app.Run();

static void SetupServices(WebApplicationBuilder builder, IConfiguration config)
{
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    builder.Services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(config.GetConnectionString("SongsDb")));

    builder.Services.Configure<SongRepository>(config.GetSection(nameof(SongRepository)));

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    builder.Services.AddDbContext<UserDbContext>(
        options => options.UseSqlServer(config.GetConnectionString("UsersDb")));

    builder.Services.AddIdentityApiEndpoints<SongServiceUser>()
        .AddRoles<IdentityRole>()
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddUserManager<UserManager<SongServiceUser>>()
        .AddEntityFrameworkStores<UserDbContext>();

    var cors = config.GetSection(nameof(CorsPolicy)).Get<CorsPolicy>();
    builder.Services.AddCors(options => options.AddPolicy(cors.Name,
                           policy => policy.WithOrigins(cors.Origins)));

    builder.Services.AddSingleton<ISongDbInitializer, SneSongsFromXmlInitializer>();
    builder.Services.AddSingleton<ISongRepository, MongoSongRepository>();

    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = true);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void SetupMiddleware(WebApplication app)
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

static async Task InitializeSongDatabase(WebApplication app, ConfigurationManager config)
{
    using var scope = app.Services.CreateScope();

    var repository = scope.ServiceProvider.GetRequiredService<ISongRepository>();

    await repository.Initialize();
}

static async Task CreateSuperUser(WebApplication app, IConfiguration config)
{
    using var scope = app.Services.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SongServiceUser>>();

    var saRole = config["SuperUser:Role"];

    string[] roles = [saRole, "Admin", "Owner", "Contributor", "User"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var roleRole = new IdentityRole(role);
            await roleManager.CreateAsync(roleRole);
        }
    }

    var saEmail = config["SuperUser:Email"];
    var saPassword = config["SuperUser:Password"];
    
    if (await userManager.FindByNameAsync(saEmail) == null)
    {
        var user = new SongServiceUser()
        {
            UserName = "sa",
            Email = saEmail,
            EmailConfirmed = true,
        };
        var result = await userManager.CreateAsync(user, saPassword);
        await userManager.AddToRoleAsync(user, saRole);
    }
}