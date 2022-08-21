using System;
using System.Collections.Generic;
using System.Linq;

namespace SongBookService.API.Models.FullSong
{
    public class Line
    {
        public Guid Id { get; set; }
        public List<Lyrics> Lyrics { get; set; } = new();
        public List<Chords> Chords { get; set; } = new();
        public List<Guid> Order { get; set; } = new();
        public string GetText() =>
            string.Join("", Lyrics.Select(l => l.Text));
    }
}