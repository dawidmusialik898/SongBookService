using System;
using System.Collections.Generic;

using SongBookService.API.Models.ValueObjects;

namespace SongBookService.API.Models.SimpleSong
{
    public class Song
    {
        public Guid Id { get; set; }
        public SongTitle Title { get; set; }
        public SongTitle OriginalTitle { get; set; }
        public Author Author { get; set; }
        public SongNumber Number { get; set; }
        public Metronome Tempo { get; set; }
        public Key Key { get; set; }
        public IEnumerable<Part> DistinctParts { get; set; }
        public IEnumerable<Guid> PartOrder { get; set; }
    }
}
