using SongBookService.API.Model.ValueObjects;
using System;
using System.Collections.Generic;

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

        public void AddPart(string s)
        {
            Lyrics l = new();
            l.Text = s;

            Line line = new();
            line.Lyrics.Add(l);

            Slide slide = new();
            slide.DistinctLines.Add(line);

            Part part = new();
            part._distinctSlides.Add(slide);

            DistinctParts.Add(part);

        }
    }
}
