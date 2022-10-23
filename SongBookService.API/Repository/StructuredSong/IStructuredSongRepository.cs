using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongBookService.API.Repository.StructuredSong
{
    public interface IStructuredSongRepository
    {
        public Task AddSongAsync(Models.StructuredSong.StructuredSong song);
        public Task UpdateSongAsync(Models.StructuredSong.StructuredSong modifiedSong);
        public Task DeleteSongAsync(Guid id);
        public Task<Models.StructuredSong.StructuredSong> GetSongAsync(Guid id);
        public Task<IEnumerable<Models.StructuredSong.StructuredSong>> GetSongsAsync();
    }
}
