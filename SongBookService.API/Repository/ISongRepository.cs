using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SongBookService.API.Models;

namespace SongBookService.API.Repository
{
    public interface ISongRepository
    {
        public Task Initialize();
        public Task AddSong(Song song);
        public Task UpdateSong(Song modifiedSong);
        public Task DeleteSong(Guid id);
        public Task<Song> GetSong(Guid id);
        public Task<IEnumerable<Song>> GetSongs();
    }
}
