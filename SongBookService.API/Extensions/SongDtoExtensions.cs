using System.Linq;

using SongBookService.API.DTOs;
using SongBookService.API.Models;

namespace SongBookService.API.Extensions
{
    public static class SongDtoExtensions
    {
        public static SongDTO AsStructuredSongDTO(this Song song)
        {
            return new SongDTO()
            {
                Id = song.Id,
                Number = song.Number,
                Title = song.Title,
                OriginalTitle = song.OriginalTitle,
                Author = song.Author,
                PartOrder = song.PartOrder.ToList(),
                Parts = song.Parts.Select(p => p.AsStructuredPartDTO()).ToList(),
                Slides = song.Parts.SelectMany(x => x.Slides.Select(slide => slide.AsStructuredSlideDTO())).ToList(),
            };
        }
        private static PartDTO AsStructuredPartDTO(this Part part)
        {

            return new PartDTO()
            {
                Id = part.Id,
                Name = part.Name,
                SlideOrder = part.SlideOrder,
            };
        }
        private static SlideDTO AsStructuredSlideDTO(this Slide slide)
        {
            return new SlideDTO()
            {
                Id = slide.Id,
                Text = slide.Text,
            };
        }
        public static Song AsStructuredSong(this SongDTO structuredSong)
        {
            return new Song()
            {
                Id = structuredSong.Id,
                Number = structuredSong.Number,
                Title = structuredSong.Title,
                Parts = structuredSong.Parts.Select(p => p.AsStructuredPart(structuredSong)).ToList(),
                PartOrder = structuredSong.PartOrder,
                Author = null,
                Key = "",
                OriginalTitle = null,
            };
        }
        private static Part AsStructuredPart(this PartDTO structuredPart, SongDTO structuredSong)
        {
            return new Part()
            {
                Id = structuredPart.Id,
                Name = string.IsNullOrEmpty(structuredPart.Name) ? null : structuredPart.Name,
                Slides = structuredPart.SlideOrder.Distinct().SelectMany(id => structuredSong.Slides.Where(slide => slide.Id == id)).Select(slide => slide.AsSlide()).ToList(),
                SlideOrder = structuredPart.SlideOrder,
            };
        }
        private static Slide AsSlide(this SlideDTO structuredSlide)
        {
            return new Slide()
            {
                Id = structuredSlide.Id,
                Text = structuredSlide.Text,
            };
        }
    }
}

