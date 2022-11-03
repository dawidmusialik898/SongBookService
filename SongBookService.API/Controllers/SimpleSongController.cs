using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using SongBookService.API.DTOs;
using SongBookService.API.Models.StructuredSong;
using SongBookService.API.Repository.StructuredSong;

namespace SongBookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleSongController : ControllerBase
    {
        private readonly IStructuredSongRepository _repository;
        private readonly IMapper _mapper;

        public SimpleSongController(IMapper mapper, IStructuredSongRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // GET: api/<SimpleSongController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SimpleSongDTO>>> GetSongsAsync()
        {
            var songs = await _repository.GetSongsAsync();
            var songDTOs = _mapper.Map<IEnumerable<StructuredSong>, IEnumerable<SimpleSongDTO>>(songs);

            return songDTOs == null ?
               NotFound()
               : Ok(songDTOs);
        }

        // GET api/<SimpleSongController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SimpleSongDTO>> GetSongAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            return _mapper.Map<SimpleSongDTO>(resultSong) == null ?
                NotFound()
                : Ok(resultSong);
        }
    }
}
