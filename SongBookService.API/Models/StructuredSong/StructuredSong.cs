using System;
using System.Collections.Generic;

using SongBookService.API.Models.ValueObjects;

namespace SongBookService.API.Models.StructuredSong
{
    public class StructuredSong
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public string Number { get; set; }
        public Key Key { get; set; }
        public IEnumerable<StructuredPart> Parts { get; set; }
        public IEnumerable<Guid> PartOrder { get; set; }
    }
}
