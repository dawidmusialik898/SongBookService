using System;
using System.Collections.Generic;

namespace SongBookService.API.DTOs
{
    public record StructuredSongDTO
    {
        public Guid Id { get; init; }
        public string Number { get; init; }
        public string Title { get; init; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public List<StructuredPartDTO> Parts { get; init; }
        public List<Guid> PartOrder { get; init; }
        public List<StructuredSlideDTO> Slides{ get; init; }
    }

    public record struct StructuredPartDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public List<Guid> SlideOrder { get; init; }
    }

    public record struct StructuredSlideDTO
    {
        public Guid Id { get; init; }
        public string Text { get; init; }
    }
}
