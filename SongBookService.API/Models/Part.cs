using System;
using System.Collections.Generic;
using System.Linq;

namespace SongBookService.API.Models
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
        public string Name { get; set; }

        /// <summary>
        /// List of distinct slides.
        /// </summary>
        public List<Slide> Slides { get; set; }
        public List<Guid> SlideOrder { get; set; }

        public string GetText()
        {
            var slides = SlideOrder.Select(x => Slides.Where(y => y.Id == x).First()).Select(z => z.Text);
            return string.Join(Environment.NewLine, slides);
        }
    }
}
