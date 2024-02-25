using System;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SongBookService.API.Identity
{
    public class UserDbContext : IdentityDbContext<SongServiceUser>
    {
        //private readonly DbContextOptions<UserDbContext> _options;
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
    }
}
