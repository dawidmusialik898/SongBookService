using SongBookService.API.Model.Entities;
using System.Collections.Generic;

namespace SongBookService.API.DbInitializers
{
    public interface IDbInitializer
    {
        IEnumerable<Song> GetSongs();
    }
}
