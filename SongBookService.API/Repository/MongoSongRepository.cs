﻿using MongoDB.Bson;
using MongoDB.Driver;
using SongBookService.API.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongBookService.API.Repository
{
    public class MongoSongRepository : ISongRepository
    {
        private const string _databaseName = "SongBook";
        private const string _collectionName = "Songs";
        private readonly IMongoCollection<Song> _songCollection;
        private readonly FilterDefinitionBuilder<Song> _filterBuilder = Builders<Song>.Filter;

        public MongoSongRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(_databaseName);
            _songCollection = database.GetCollection<Song>(_collectionName);
        }

        public async Task AddSongAsync(Song song)
        {
            await _songCollection.InsertOneAsync(song);
        }

        public async Task DeleteSongAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(song => song.Id, id);
            await _songCollection.DeleteOneAsync(filter);
        }

        public async Task<Song> GetSongAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(song => song.Id, id);
            return await _songCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Song>> GetSongsAsync()
        {
            return await _songCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateSongAsync(Song modifiedSong)
        {
            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, modifiedSong.Id);
            await _songCollection.ReplaceOneAsync(filter, modifiedSong);
        }
    }
}
