using System;
using System.Collections.Generic;

using SongBookService.API.Model.ValueObjects;

namespace SongBookService.API.Model.Entities
{
    /// <summary>
    /// Entity class representing a song.
    /// </summary>
    public class Song
    {
        public Guid Id { get; set; }
        public SongTitle Title { get; set; }
        public SongTitle OriginalTitle { get; set; }
        public Author Author { get; set; }
        public SongNumber Number { get; set; }
        public Metronome Tempo { get; set; }
        public Key Key { get; set; }
        public List<Guid> PartOrder { get; set; } = new();
        public List<Part> DistinctParts { get; set; } = new();
    }
}
