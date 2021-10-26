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
        public Guid _id;

        /// <summary>
        /// Part name.
        /// </summary>
        public PartName _name;

        /// <summary>
        /// List of slide guids in order we want to show them.
        /// </summary>
        public List<Guid> _slideOrder;

        /// <summary>
        /// List of distinct slides.
        /// </summary>
        public List<Slide> _distinctSlides = new();
    }
}
