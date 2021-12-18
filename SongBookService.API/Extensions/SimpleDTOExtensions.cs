using System;
using System.Collections.Generic;
using System.Linq;

using SongBookService.API.DTOs;
using SongBookService.API.Model.Entities;
using SongBookService.API.Model.ValueObjects;

namespace SongBookService.API.Extensions
{
    public static class SimpleDTOExtensions
    {
        public static SongItemListDTO AsItemListDTO(this Song song)
        {
            return new SongItemListDTO()
            {
                Id = song.Id,
                Title = song.Title?.Title,
                Number = song.Number?.AsString(),
            };
        }

        public static SimpleSongDTO AsSimpleSongDTO(this Song song)
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

        public static Song AsSong(this SimpleSongDTO simpleSong)
        {
            var parts = GetPartsFromSimpleSong(simpleSong);
            return new Song()
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

        private static List<Part> GetPartsFromSimpleSong(SimpleSongDTO simpleSong)
        {
            List<Part> outputParts = new();
            foreach (var p in simpleSong.Parts)
            {
                outputParts.Add(p.AsPart());
            }

            return outputParts;
        }

        private static Part AsPart(this SimplePartDTO p)
        {
            var lyrics = p.Text
                .Split(Constants._newLineSymbols, StringSplitOptions.None)
                .ToList()
                .Select(s => new Lyrics()
                {
                    Id = Guid.NewGuid(),
                    Text = s
                });

            var lines = lyrics
                .Select(ly => new Line()
                {
                    Id = Guid.NewGuid(),
                    Lyrics = new List<Lyrics>() { ly },
                    Order = new List<Guid>() { ly.Id }
                });

            var slides = new List<Slide>()
                {
                    new Slide()
                    {
                        Id=Guid.NewGuid(),
                        Lines=lines.ToList(),
                    }
                };

            return new Part()
            {
                Id = Guid.NewGuid(),
                Name = string.IsNullOrEmpty(p.Name) ? null : new PartName(p.Name),
                DistinctSlides = slides,
                SlideOrder = slides.Select(s => s.Id).ToList()
            };
        }
    }
}
