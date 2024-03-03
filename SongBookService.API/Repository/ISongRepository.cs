using System;
using System.Threading.Tasks;

using SongBookService.API.Models;

namespace SongBookService.API.Repository
{
    public interface ISongRepository
    {
        public Task Initialize();
        public Task AddSongToSongBook(Guid songBookId, Song newSong);
        public Task UpdateSongInSongBook(Guid songBookId, Song modifiedSong);
        public Task DeleteSongFromSongBook(Guid songBookId, Guid songId);
        public Task<SongBook> GetSongBook(Guid songBookId);
        public Task<Song> GetSongFromSongBook(Guid songBookId, Guid songId);
    }
}
