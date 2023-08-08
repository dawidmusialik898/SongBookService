using System;
using System.Collections.Generic;

namespace SongBookService.API.DTOs
{
    public  readonly record struct SongDTO
    {
        public Guid Id { get; init; }
        public string Number { get; init; }
        public string Title { get; init; }
        public string OriginalTitle { get; init; }
        public string Author { get; init; }
        public List<PartDTO> Parts { get; init; }
        public List<Guid> PartOrder { get; init; }
        public List<SlideDTO> Slides{ get; init; }
    }
}
