﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using SongBookService.API.DbInitializers.FullSong;
using SongBookService.API.Models.FullSong;
using SongBookService.API.Repository.FullSong;

namespace SongBookService.API.Repository.FullSsong
{
    public class MongoSongRepository : ISongRepository
    {
        private const string _databaseName = "SongBook";
        private const string _collectionName = "Songs";
        private readonly IMongoCollection<Song> _songCollection;
        private readonly FilterDefinitionBuilder<Song> _filterBuilder = Builders<Song>.Filter;

        public MongoSongRepository(IMongoClient mongoClient, IFullSongDbInitializer initializer)
        {
            var database = mongoClient.GetDatabase(_databaseName);

            _songCollection = database.GetCollection<Song>(_collectionName);

            var songs = _songCollection.Find(new BsonDocument()).ToList();
            if (!songs.Any())
            {
                _songCollection.InsertMany(initializer.GetSongs());
            }
        }

        public async Task AddSongAsync(Song song) =>
            await _songCollection.InsertOneAsync(song);

        public async Task DeleteSongAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(song => song.Id, id);
            await _songCollection.DeleteOneAsync(filter);
        }

        public async Task<Song> GetSongAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(song => song.Id, id);
            var song = await _songCollection.FindAsync(filter);
            return await song.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Song>> GetSongsAsync()
        {
            var songs = await _songCollection.FindAsync(new BsonDocument());
            return await songs.ToListAsync();
        }

        public async Task UpdateSongAsync(Song modifiedSong)
        {
            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, modifiedSong.Id);
            await _songCollection.ReplaceOneAsync(filter, modifiedSong);
        }
    }
}