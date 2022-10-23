using SongBookService.API.Models.StructuredSong;

using System.Collections.Generic;

namespace SongBookService.API.DbInitializers.StructuredSong
{
    public interface IStructuredSongDbInitializer
    {
        IEnumerable<Models.StructuredSong.StructuredSong> GetSongs();
    }
}
