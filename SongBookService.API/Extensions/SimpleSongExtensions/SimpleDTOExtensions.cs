using System;
using System.Collections.Generic;
using System.Linq;

using SongBookService.API.DTOs;
using SongBookService.API.Models.ValueObjects;

namespace SongBookService.API.Extensions.SimpleSongExtensions
{
    public static class SimpleDTOExtensions
    {
        public static SongItemListDTO AsItemListDTO(this Models.SimpleSong.Song song)
        {
            return new SongItemListDTO()
            {
                Id = song.Id,
                Title = song.Title?.Title,
                Number = song.Number?.AsString(),
            };
        }

        public static SimpleSongDTO AsSimpleSongDTO(this Models.SimpleSong.Song song)
        {
            return new SimpleSongDTO()
            {
                Id = song.Id,
                Number = song.Number?.Prefix + song.Number?.Number,
                Title = song.Title?.Title,
                Parts = song.DistinctParts?.Select(
                    p => new SimplePartDTO() { Text = p.GetText(), Name = p.Name?.Name }).ToList()
            };
        }

        public static Models.SimpleSong.Song AsSong(this SimpleSongDTO simpleSong)
        {
            var parts = GetPartsFromSimpleSong(simpleSong);
            return new Models.SimpleSong.Song()
            {
                Id = simpleSong.Id,
                Author = null,
                Key = Key.Unknown,
                Number = new SongNumber(simpleSong.Number),
                Title = new SongTitle(simpleSong.Title),
                OriginalTitle = null,
                Tempo = null,
                DistinctParts = parts,
                PartOrder = parts.Select(p => p.Id).ToList()
            };
        }

        private static IEnumerable<Models.SimpleSong.Part> GetPartsFromSimpleSong(SimpleSongDTO simpleSong)
        {

            return simpleSong.Parts.Select(x => {
                var slides = new List<Models.SimpleSong.Slide>()
                {
                    new Models.SimpleSong.Slide()
                    {
                        Id = Guid.NewGuid(),
                        Text = x.Text
                    }
                };

                return new Models.SimpleSong.Part()
                {
                    Id = Guid.NewGuid(),
                    Name = new PartName(x.Name),
                    DistinctSlides = slides,
                    SlideOrder = slides.Select(x => x.Id).ToList(),
                };
            });
        }
    }
}
