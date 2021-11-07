using Microsoft.AspNetCore.Http;
using SongBookService.API.DTOs;
using SongBookService.API.Model.Entities;
using SongBookService.API.Model.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongBookService.API.Extensions
{
    public static class DTOExtensions
    {
        public static SongItemListDTO AsItemListDTO(this Song song)
        {
            return new SongItemListDTO()
            {
                Id = song.Id,
                Title = song.Title?.Title,
                Number = song.Number?.Number,
            };
        }

        public static SimpleSongWithoutStructureDTO AsSimpleSongWithoutStructureDTO(this Song song)
        {
            return new SimpleSongWithoutStructureDTO()
            {
                Id= song.Id,
                Number = song.Number?.Number,
                Title = song.Title?.Title,
                Parts = song.DistinctParts.Select(
                    p => new SimplePartWithoutStructureDTO() { Text = p.GetText(), Name = p.Name?.Name }).ToList()
            };
        }

        public static Song AsSong(this SimpleSongWithoutStructureDTO simpleSong)
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

        private static List<Part> GetPartsFromSimpleSong(SimpleSongWithoutStructureDTO simpleSong)
        {
            List<Part> outputParts = new();
            foreach (var p in simpleSong.Parts)
            {
                outputParts.Add(p.AsPart());
            }
            return outputParts;
        }

        private static Part AsPart(this SimplePartWithoutStructureDTO p)
        {
            var lyrics = p.Text
                .Split(Constants.NewLineSymbols, StringSplitOptions.None)
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
                        DistinctLines=lines.ToList(),
                        LineOrder= lines.Select(l=>l.Id).ToList()
                    }
                };

            return new Part()
            {
                Id = Guid.NewGuid(),
                Name = string.IsNullOrEmpty(p.Name)? null:new PartName(p.Name),
                DistinctSlides = slides,
                SlideOrder = slides.Select(s => s.Id).ToList()
            };
        }
    }
}
