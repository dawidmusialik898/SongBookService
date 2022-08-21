using System;
using System.Collections.Generic;
using System.Linq;

using SongBookService.API.Models.ValueObjects;

namespace SongBookService.API.Models.SimpleSong
{
    public class Part
    {
        /// <summary>
        /// Part id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Part name.
        /// </summary>
        public PartName Name { get; set; }

        /// <summary>
        /// List of slide guids in order we want to show them.
        /// </summary>
        public List<Guid> SlideOrder { get; set; } = new();

        /// <summary>
        /// List of distinct slides.
        /// </summary>
        public List<Slide> DistinctSlides { get; set; } = new();

        public string GetText()
        {
            var slides = DistinctSlides.Select(s => s.Text);
            return string.Join(Environment.NewLine, slides);
        }
    }
}
