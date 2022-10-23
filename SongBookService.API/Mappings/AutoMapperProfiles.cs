using AutoMapper;

using SongBookService.API.DTOs;
using SongBookService.API.Models.StructuredSong;

namespace SongBookService.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<StructuredSongDTO, StructuredSong>();
            CreateMap<StructuredSong, StructuredSongDTO>();

            CreateMap<StructuredSong, SongItemListDTO>();
        }
    }
}
