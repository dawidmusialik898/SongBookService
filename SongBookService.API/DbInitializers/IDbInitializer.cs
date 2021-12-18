using System.Collections.Generic;

using SongBookService.API.Model.Entities;

namespace SongBookService.API.DbInitializers
{
    public interface IDbInitializer
    {
        IEnumerable<Song> GetSongs();
    }
}
