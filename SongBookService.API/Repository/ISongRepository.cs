using SongBookService.API.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongBookService.API.Repository
{
    public interface ISongRepository
    {
        public Task AddSongAsync(Song song);
        public Task UpdateSongAsync(Song modifiedSong);
        public Task DeleteSongAsync(Guid id);
        public Task<Song> GetSongAsync(Guid id);
        public Task<IEnumerable<Song>> GetSongsAsync();
    }
}
