using System;
using System.Collections.Generic;
using System.Linq;

namespace SongBookService.API.Model.Entities
{
    public class Slide
    {
        public Guid Id { get; set; }
        public List<Line> DistinctLines { get; set; } = new();
        public List<Guid> LineOrder { get; set; } = new();
        public string GetText()
        {
            var textLines = DistinctLines.Select(l => l.GetText());
            return string.Join(Environment.NewLine, textLines);
        }
    }
}