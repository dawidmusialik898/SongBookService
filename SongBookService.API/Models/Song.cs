using System;
using System.Collections.Generic;

namespace SongBookService.API.Models.Song
{
    public class Song
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Author { get; set; }
        public string Number { get; set; }
        public string Key { get; set; }
        public List<Part> Parts { get; set; }
        public List<Guid> PartOrder { get; set; }
    }
}
