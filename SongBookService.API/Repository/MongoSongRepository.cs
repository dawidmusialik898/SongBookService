using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using SongBookService.API.DbInitializers;
using SongBookService.API.Options;

namespace SongBookService.API.Repository
{
    public class MongoSongRepository : ISongRepository
    {
        private readonly IMongoCollection<Models.Song> _songCollection;
        private readonly FilterDefinitionBuilder<Models.Song> _filterBuilder = Builders<Models.Song>.Filter;
        private readonly ISongDbInitializer _initializer;
        private readonly IOptions<SongRepositoryOptions> _songRepositoryOptions;

        public MongoSongRepository(
            IMongoClient mongoClient,
            ISongDbInitializer initializer,
            IOptions<SongRepositoryOptions> songRepositoryOptions)
        {
            _initializer = initializer;
            _songRepositoryOptions = songRepositoryOptions;

            var database = mongoClient.GetDatabase(_songRepositoryOptions.Value.DatabaseName);
            _songCollection = database.GetCollection<Models.Song>(_songRepositoryOptions.Value.CollectionName);
        }

        public async Task Initialize()
        {
            var songs = await _songCollection.FindAsync(new BsonDocument());
            if (!songs.Any())
            {
                await _songCollection.InsertManyAsync(_initializer.GetSongs());
            }
        }

        public async Task AddSong(Models.Song song)
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

        public async Task DeleteSong(Guid id)
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

        public async Task<Models.Song> GetSong(Guid id)
        {

            var song = await _songCollection.FindAsync(x => x.Id == id);
            var songList = song.ToList();
            return songList.Count == 0 ?
                null :
                songList.First();
        }

        public async Task<IEnumerable<Models.Song>> GetSongs()
        {
            var songs = await _songCollection.FindAsync(new BsonDocument());
            return await songs.ToListAsync();
        }


        public async Task UpdateSong(Models.Song modifiedSong)
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
