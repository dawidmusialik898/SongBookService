using System;
using System.Collections.Generic;
using System.Linq;

using SongBookService.API.Models.ValueObjects;

namespace SongBookService.API.Models.StructuredSong
{
    public class StructuredPart
    {
        /// <summary>
        /// Part id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Part name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of distinct slides.
        /// </summary>
        public List<StructuredSlide> Slides { get; set; } = new();
        public List<Guid> SlideOrder { get; set; } = new();

        public string GetText()
        {
            var slides = SlideOrder.Select(x => Slides.Where(y => y.Id == x).First()).Select(z=>z.Text);
            return string.Join(Environment.NewLine, slides);
        }
    }
}
