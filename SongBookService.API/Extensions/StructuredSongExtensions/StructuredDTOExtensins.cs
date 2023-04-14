using System.Linq;

using SongBookService.API.DTOs;
using SongBookService.API.Models.StructuredSong;

namespace SongBookService.API.Extensions.StructuredSongExtensions
{
    public static class StructuredDTOExtensins
    {
        public static StructuredSongDTO AsStructuredSongDTO(this StructuredSong song)
        {
            return new StructuredSongDTO()
            {
                Id = song.Id,
                Number = song.Number,
                Title = song.Title,
                OriginalTitle= song.OriginalTitle,
                Author= song.Author,
                PartOrder = song.PartOrder.ToList(),
                Parts = song.Parts.Select(p => p.AsStructuredPartDTO()).ToList(),
                Slides = song.Parts.SelectMany(x => x.Slides.Select(slide => slide.AsStructuredSlideDTO())).ToList(),
            };
        }
        private static StructuredPartDTO AsStructuredPartDTO(this StructuredPart part)
        {

            return new StructuredPartDTO()
            {
                Id = part.Id,
                Name = part.Name,
                SlideOrder = part.SlideOrder,
            };
        }
        private static StructuredSlideDTO AsStructuredSlideDTO(this StructuredSlide slide)
        {
            return new StructuredSlideDTO()
            {
                Id = slide.Id,
                Text = slide.Text,
            };
        }
        public static StructuredSong AsStructuredSong(this StructuredSongDTO structuredSong)
        {
            return new StructuredSong()
            {
                Id = structuredSong.Id,
                Number = structuredSong.Number,
                Title = structuredSong.Title,
                Parts = structuredSong.Parts.Select(p => p.AsStructuredPart(structuredSong)).ToList(),
                PartOrder = structuredSong.PartOrder,
                Author = null,
                Key = 0,
                OriginalTitle = null,
            };
        }
        private static StructuredPart AsStructuredPart(this StructuredPartDTO structuredPart, StructuredSongDTO structuredSong)
        {
            return new StructuredPart()
            {
                Id = structuredPart.Id,
                Name = string.IsNullOrEmpty(structuredPart.Name) ? null : structuredPart.Name,
                Slides = structuredPart.SlideOrder.Distinct().SelectMany(id => structuredSong.Slides.Where(slide => slide.Id == id)).Select(slide => slide.AsSlide()).ToList(),
                SlideOrder = structuredPart.SlideOrder,
            };
        }
        private static StructuredSlide AsSlide(this StructuredSlideDTO structuredSlide)
        {
            return new StructuredSlide()
            {
                Id = structuredSlide.Id,
                Text = structuredSlide.Text,
            };
        }
    }
}

