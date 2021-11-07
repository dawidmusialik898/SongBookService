using System;
using System.Collections.Generic;

namespace SongBookService.API.Model.Entities
{
    public class Line
    {
        public Guid Id { get; set; }
        public List<Lyrics> Lyrics { get; set; } = new();
        public List<Chords> Chords { get; set; } = new();
        public List<Guid> Order { get; set; } = new();
        public string GetText()
        {
            return string.Join("",Lyrics);
        }
    }
}