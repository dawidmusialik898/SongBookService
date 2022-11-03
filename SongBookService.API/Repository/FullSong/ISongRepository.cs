using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongBookService.API.Repository.FullSong
{
    public interface ISongRepository
    {
        public Task AddSongAsync(Models.FullSong.Song song);
        public Task UpdateSongAsync(Models.FullSong.Song modifiedSong);
        public Task DeleteSongAsync(Guid id);
        public Task<Models.FullSong.Song> GetSongAsync(Guid id);
        public Task<IEnumerable<Models.FullSong.Song>> GetSongsAsync();
    }
}
