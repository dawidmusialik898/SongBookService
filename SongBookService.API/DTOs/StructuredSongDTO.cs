using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongBookService.API.DTOs
{
    public record StructuredSongDTO
    {
        public Guid Id { get; init; }
        public string Number { get; init; }
        public string Title { get; init; }
        public List<StructuredPartDTO> Parts { get; init; }
        public List<Guid> PartOrder { get; init; }
    }

    public record StructuredPartDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public List<StructuredSlideDTO> Slides { get; init; }
        public List<Guid> SlideOrder { get; init; }
    }

    public record StructuredSlideDTO
    {
        public Guid Id { get; init; }
        public string Text { get; init; }
    }
}
