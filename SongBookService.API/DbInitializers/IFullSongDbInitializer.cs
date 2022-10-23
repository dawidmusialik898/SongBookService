using System.Collections.Generic;

using SongBookService.API.Models.FullSong;

namespace SongBookService.API.DbInitializers
{
    public interface IFullSongDbInitializer
    {
        IEnumerable<Song> GetSongs();
    }
}
