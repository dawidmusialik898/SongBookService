using System;
using System.Collections.Generic;
using System.Linq;

namespace SongBookService.API.Model.Entities
{
    public class Slide
    {
        public Guid Id { get; set; }
        public List<Line> Lines { get; set; } = new();
        public string GetText()
        {
            var textLines = Lines.Select(l => l.GetText());
            return string.Join(Environment.NewLine, textLines);
        }
    }
}