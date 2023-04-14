using System;
using System.Collections.Generic;
using System.Linq;

using SongBookService.API.DTOs;
using SongBookService.API.Models.ValueObjects;
using SongBookService.API.Models.StructuredSong;

namespace SongBookService.API.Extensions.StructuredSongExtensions
{
    public static class SimpleDTOExtensions
    {
        public static SongItemListDTO AsItemListDTO(this StructuredSong song)
        {
            return new SongItemListDTO()
            {
                Id = song.Id,
                Title = song.Title,
                Number = song.Number,
            };
        }

        public static SimpleSongDTO AsSimpleSongDTO(this StructuredSong song)
        {
            return new SimpleSongDTO()
            {
                Id = song.Id,
                Number = song.Number,
                Title = song.Title,
                Parts = song.Parts?.Select(
                    p => new SimplePartDTO() { Text = p.GetText(), Name = p.Name }).ToList()
            };
        }

        public static StructuredSong AsStructuredSong(this SimpleSongDTO simpleSong)
        {
            var parts = GetPartsFromSimpleSong(simpleSong);
            return new StructuredSong()
            {
                Id = simpleSong.Id,
                Author = null,
                Key = Key.Unknown,
                Number = simpleSong.Number,
                Title = simpleSong.Title,
                OriginalTitle = null,
                Parts = parts,
                PartOrder = parts.Select(p => p.Id).ToList()
            };
        }

        private static List<StructuredPart> GetPartsFromSimpleSong(SimpleSongDTO simpleSong)
        {
            List<StructuredPart> outputParts = new();
            foreach (var p in simpleSong.Parts)
            {
                outputParts.Add(p.AsPart());
            }

            return outputParts;
        }

        private static StructuredPart AsPart(this SimplePartDTO p)
        {

            var slides = new List<StructuredSlide>()
            {
                new StructuredSlide()
                {
                    Id=Guid.NewGuid(),
                    Text=p.Text,
                }
            };

            return new StructuredPart()
            {
                Id = Guid.NewGuid(),
                Name = p.Name,
                Slides = slides,
                SlideOrder = slides.Select(s => s.Id).ToList()
            };
        }
    }
}