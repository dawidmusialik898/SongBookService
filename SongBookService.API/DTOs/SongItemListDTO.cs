using MongoDB.Driver.Core.Operations;
using SongBookService.API.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongBookService.API.DTOs
{
    public record SongItemListDTO
    {
        public Guid Id { get; init; }
        public int? Number { get; init; }
        public string Title { get; init; }
    }
}
