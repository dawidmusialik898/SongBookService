using System;
using System.Collections.Generic;

namespace SongBookService.API.Model.Entities
{
    public class Slide
    {
        public Guid Id { get; set; }
        public List<Line> DistinctLines { get; set; } = new();
        public List<Guid> LineOrder { get; set; } = new();
    }
}