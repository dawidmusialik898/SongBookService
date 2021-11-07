using Microsoft.AspNetCore.Http;
using SongBookService.API.DTOs;
using SongBookService.API.Model.Entities;
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
                Number = song.Number?.Number,
                Title = song.Title?.Title,
                Parts = song.DistinctParts.Select(
                    p => new SimplePartWithoutStructureDTO() { Text = p.GetText(), Name = p.Name?.Name }).ToList()
            };
        }
    }
}
