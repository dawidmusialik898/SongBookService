using SongBookService.API.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongBookService.API.DTOs
{
    public record SimpleSongWithoutStructureDTO
    {
        public Guid Id { get; init; }
        public int? Number { get; init; }
        public string Title { get; init; }


        public List<SimplePartWithoutStructureDTO> Parts { get; init; }
    }

    public record SimplePartWithoutStructureDTO
    {
        public string Name { get; init; }
        public string Text { get; init; }
    }
}
