using System.Linq;

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

            CreateMap<StructuredPartDTO, StructuredPart>();
            CreateMap<StructuredPart, StructuredPartDTO>();

            CreateMap<StructuredSlideDTO, StructuredSlide>();
            CreateMap<StructuredSlide, StructuredSlideDTO>();

            CreateMap<StructuredSong, SongItemListDTO>();

            CreateMap<StructuredSong, SimpleSongDTO>()
                .ForMember(dest => dest.Parts, opt => opt.MapFrom(src => src.Parts.Select(
                    x => new SimplePartDTO()
                    {
                        Name = x.Name,
                        Text = x.GetText()
                    }
                )));
        }
    }
}
