using System;
using System.Collections.Generic;
using System.Linq;

using SongBookService.API.DTOs;
using SongBookService.API.Model.Entities;
using SongBookService.API.Model.ValueObjects;

namespace SongBookService.API.Extensions
{
    public static class StructuredDTOExtensins
    {
        public static StructuredSongDTO AsStructuredSong(this Song song)
        {
            return new StructuredSongDTO()
            {
                Id = song.Id,
                Number = song.Number?.AsString(),
                Title = song.Title?.Title,
                PartOrder = song.PartOrder,
                Parts = song.DistinctParts.Select(p => p.AsStructuredPart()).ToList()
            };

        }
        private static StructuredPartDTO AsStructuredPart(this Part part)
        {

            return new StructuredPartDTO()
            {
                Id = part.Id,
                Name = part.Name?.Name,
                Slides = part.DistinctSlides.Select(s => s.AsStructuredSlide()).ToList(),
                SlideOrder = part.SlideOrder,
            };
        }
        private static StructuredSlideDTO AsStructuredSlide(this Slide slide)
        {
            return new StructuredSlideDTO()
            {
                Id = slide.Id,
                Text = slide.GetText()
            };
        }
        public static Song AsSong(this StructuredSongDTO structuredSong)
        {
            return new Song()
            {
                Id = structuredSong.Id,
                Number = new SongNumber(structuredSong.Number),
                Title = new SongTitle(structuredSong.Title),
                DistinctParts = structuredSong.Parts.Select(p => p.AsPart()).ToList(),
                PartOrder = structuredSong.PartOrder,
                Author = null,
                Key = 0,
                OriginalTitle = null,
                Tempo = null,
            };
        }
        private static Part AsPart(this StructuredPartDTO structuredPart)
        {
            return new Part()
            {
                Id = structuredPart.Id,
                Name = new PartName(string.IsNullOrEmpty(structuredPart.Name) ? null : structuredPart.Name),
                DistinctSlides = structuredPart.Slides.Select(s => s.AsSlide()).ToList(),
                SlideOrder = structuredPart.SlideOrder,
            };
        }
        private static Slide AsSlide(this StructuredSlideDTO structuredSlide)
        {
            var lines = new List<Line>();
            foreach (var line in structuredSlide.Text.Split(Constants._newLineSymbols, StringSplitOptions.None))
            {
                lines.Add(GetLine(line));
            }

            return new Slide()
            {
                Id = structuredSlide.Id,
                Lines = lines
            };
        }
        private static Line GetLine(string line)
        {
            var lyrics = new List<Lyrics>()
            {
                new Lyrics()
                {
                    Id = Guid.NewGuid(),
                    Text = line,
                }
            };

            return new Line()
            {
                Id = Guid.NewGuid(),
                Lyrics = lyrics,
                Order = lyrics.Select(ly => ly.Id).ToList()
            };
        }
    }
}

