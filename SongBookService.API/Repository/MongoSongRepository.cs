using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using SongBookService.API.DbInitializers;
using SongBookService.API.Models;
using SongBookService.API.Options;

namespace SongBookService.API.Repository
{
    public class MongoSongRepository : ISongRepository
    {
        private readonly IMongoCollection<SongBook> _songBookCollection;
        private readonly ISongDbInitializer _initializer;
        private readonly IOptions<SongRepository> _songRepositoryOptions;

        public MongoSongRepository(
            IMongoClient mongoClient,
            ISongDbInitializer initializer,
            IOptions<SongRepository> songRepositoryOptions)
        {
            _initializer = initializer;
            _songRepositoryOptions = songRepositoryOptions;
            var database = mongoClient.GetDatabase(_songRepositoryOptions.Value.DatabaseName);
            _songBookCollection = database.GetCollection<SongBook>(_songRepositoryOptions.Value.CollectionName);
        }

        public async Task Initialize()
        {
            var songs = await _songBookCollection.FindAsync(new BsonDocument());
            if (!songs.Any())
            {
                await _songBookCollection.InsertOneAsync(_initializer.InitializeSneSongBook());
            }
        }

        public async Task AddSongToSongBook(Guid songBookId, Song newSong)
        {
            ArgumentNullException.ThrowIfNull(newSong);
            var songBook = await GetSongBook(songBookId);

            var songWithTheSameIdExists = songBook.Songs.Any(x => x.Id == newSong.Id);
            if (songWithTheSameIdExists)
            {
                throw new ArgumentException(
                    $"Song with this Id: {newSong.Id}, already exists in database", nameof(newSong));
            }

            var songWithTheSameNumberExists = songBook.Songs.Any(x => x.Number == newSong.Number);
            if (songWithTheSameNumberExists)
            {
                throw new ArgumentException(
                    $"Song with this Number: {newSong.Number} already exists in database", nameof(newSong));
            }

            songBook.Songs.ToList().Add(newSong);

            await _songBookCollection.ReplaceOneAsync(x => x.Id == songBookId, songBook);
        }

        public async Task DeleteSongFromSongBook(Guid songBookId, Guid songId)
        {
            var songBook = await GetSongBook(songBookId);

            var songWithTheSameIdExists = songBook.Songs.Any(x => x.Id == songId);
            if (songWithTheSameIdExists)
            {
                throw new ArgumentException(
                    $"Song with this Id: {songId}, does not exist in database", nameof(songId));
            }

            var result = await _songBookCollection.DeleteOneAsync(x => x.Id == songId);
            if (result.DeletedCount == 0)
            {
                throw new Exception(
                    $"Could not delete song with this Id: {songId}");
            }
        }
        
        public async Task<SongBook> GetSongBook(Guid songBookId)
        {
            return (await _songBookCollection.FindAsync(x => x.Id == songBookId)).ToList().FirstOrDefault()
                            ?? throw new ArgumentException($"SongBook with given id: {songBookId} was not found.", nameof(songBookId));
        }

        public async Task<Song> GetSongFromSongBook(Guid songBookId, Guid songId)
        {
            var songBook = await GetSongBook(songBookId);

            return songBook.Songs.FirstOrDefault(x => x.Id == songId)
                ?? throw new ArgumentException($"Song with this Id: {songId}, does not exist in database", nameof(songId));
        }

        public async Task UpdateSongInSongBook(Guid songBookId, Song modifiedSong)
        {
            var songBook = await GetSongBook(songBookId);
            var songs = songBook.Songs.ToList();
            var existingSong = songs.FirstOrDefault(x => x.Id == modifiedSong.Id);
            if (existingSong is null)
            {
                songs.Add(modifiedSong);
            }
            else
            {
                var index = songs.IndexOf(existingSong);
                songs[index] = modifiedSong;
            }

            songBook.Songs = songs;
            var result = await _songBookCollection.ReplaceOneAsync(x => x.Id == songBookId, songBook);
        }
    }
}
