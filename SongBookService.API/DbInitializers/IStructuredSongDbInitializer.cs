using SongBookService.API.Models.StructuredSong;

using System.Collections.Generic;

namespace SongBookService.API.DbInitializers
{
    public interface IStructuredSongDbInitializer
    {
        IEnumerable<StructuredSong> GetSongs();
    }
}
