using System;
using System.Collections.Generic;

namespace SongBookService.API.Models
{
    public class SongBook
    {
        public Guid Id { get; set; }
        public IEnumerable<Song> Songs { get; set; }
    }
}
