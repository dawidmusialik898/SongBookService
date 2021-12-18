using System;
using System.Collections.Generic;

namespace SongBookService.API.DTOs
{
    public record SimpleSongDTO
    {
        public Guid Id { get; init; }
        public string Number { get; init; }
        public string Title { get; init; }
        public List<SimplePartDTO> Parts { get; init; }
    }

    public record SimplePartDTO
    {
        public string Name { get; init; }
        public string Text { get; init; }
    }
}
