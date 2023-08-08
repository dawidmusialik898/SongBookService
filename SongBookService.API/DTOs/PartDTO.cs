using System;
using System.Collections.Generic;

namespace SongBookService.API.DTOs
{
    public readonly record struct PartDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public List<Guid> SlideOrder { get; init; }
    }
}
