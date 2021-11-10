using Microsoft.AspNetCore.SignalR;
using SongBookService.API.Model.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SongBookService.API.Model.Entities
{
    /// <summary>
    /// Entity class representing song part.
    /// </summary>
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
            var slides = DistinctSlides.Select(s => s.GetText());
            return string.Join(Environment.NewLine,slides);
        }
    }
}
