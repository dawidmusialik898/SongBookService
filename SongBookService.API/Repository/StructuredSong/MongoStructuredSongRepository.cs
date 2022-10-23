using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using SongBookService.API.DbInitializers.StructuredSong;

namespace SongBookService.API.Repository.StructuredSong
{
    public class MongoStructuredSongRepository : IStructuredSongRepository
    {
        private const string _databaseName = "SongBook";
        private const string _collectionName = "SimpleSongs";
        private readonly IMongoCollection<Models.StructuredSong.StructuredSong> _songCollection;
        private readonly FilterDefinitionBuilder<Models.StructuredSong.StructuredSong> _filterBuilder = Builders<Models.StructuredSong.StructuredSong>.Filter;

        public MongoStructuredSongRepository(IMongoClient mongoClient, IStructuredSongDbInitializer initializer)
        {
            var database = mongoClient.GetDatabase(_databaseName);

            _songCollection = database.GetCollection<Models.StructuredSong.StructuredSong>(_collectionName);

            var songs = _songCollection.Find(new BsonDocument()).ToList();
            if (!songs.Any())
            {
                _songCollection.InsertMany(initializer.GetSongs());
            }
        }
        public async Task AddSongAsync(Models.StructuredSong.StructuredSong song)
        {
            if (song is null)
            {
                throw new ArgumentNullException(nameof(song));
            }

            var songWithTheSameIdAsyncCursor = await _songCollection.FindAsync(x => x.Id == song.Id);
            var songWithTheSameId = songWithTheSameIdAsyncCursor.ToList();
            if (songWithTheSameId.Any())
            {
                throw new Exception($"Song with this Id: {songWithTheSameId.First().Id}, already exists in database");
            }

            var songWithTheSameTitleAsyncCursor = await _songCollection.FindAsync(x => x.Number == song.Number);
            var songWithTheSameTitle = songWithTheSameTitleAsyncCursor.ToList();
            if (songWithTheSameTitle.Any())
            {
                throw new Exception($"Song with this Number: {songWithTheSameTitle.First().Number}" +
                    " already exists in database");
            }

            await _songCollection.InsertOneAsync(song);
        }
        public async Task DeleteSongAsync(Guid id)
        {
            var songWithTheSameId = await _songCollection.FindAsync(x => x.Id == id);
            if (!songWithTheSameId.Any())
            {
                throw new Exception($"Song with this Id: {id}, does not exist in database");
            }
            var result = await _songCollection.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new Exception($"Could not delete song with this Id: {id}");
            }
        }

        public async Task<Models.StructuredSong.StructuredSong> GetSongAsync(Guid id)
        {

            var song = await _songCollection.FindAsync(x => x.Id == id);
            var songList = song.ToList();
            if (!songList.Any())
            {
                throw new Exception($"Song with this Id: {id}, does not exist in database");
            }
            return songList.First();
        }
        public async Task<IEnumerable<Models.StructuredSong.StructuredSong>> GetSongsAsync()
        {
            var songs = await _songCollection.FindAsync(new BsonDocument());
            return await songs.ToListAsync();
        }
        public async Task UpdateSongAsync(Models.StructuredSong.StructuredSong modifiedSong)
        {
            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, modifiedSong.Id);
            var songToBeReplacedAsyncCursor = await _songCollection.FindAsync(filter);
            var songToBeReplaced = songToBeReplacedAsyncCursor.ToList();
            if (!songToBeReplaced.Any())
            {
                await _songCollection.InsertOneAsync(modifiedSong);
            }
            var result = await _songCollection.ReplaceOneAsync(filter, modifiedSong);
        }
    }
}
