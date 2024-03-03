using SongBookService.API.Models;

namespace SongBookService.API.DbInitializers
{
    public interface ISongDbInitializer
    {
        SongBook InitializeSneSongBook();
    }
}
