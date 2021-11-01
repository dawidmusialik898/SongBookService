using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace SongBookService.API.Model.Entities
{
    public class Line
    {
        public Guid Id { get; set; }
        public List<Lyrics> Lyrics { get; set; } = new();
        public List<Chords> Chords { get; set; } = new();
        public List<Guid> Order { get; set; } = new();
    }
}