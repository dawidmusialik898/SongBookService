using System;

namespace SongBookService.API.DTOs
{
    public record SongItemListDTO
    {
        public Guid Id { get; init; }
        public string Number { get; init; }
        public string Title { get; init; }
    }
}
