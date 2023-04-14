using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using SongBookService.API.DTOs;
using SongBookService.API.Extensions.StructuredSongExtensions;
using SongBookService.API.Models.StructuredSong;
using SongBookService.API.Repository.StructuredSong;

namespace SongBookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleSongController : ControllerBase
    {
        private readonly IStructuredSongRepository _repository;

        public SimpleSongController(IStructuredSongRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<SimpleSongController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SimpleSongDTO>>> GetSimpleSongsDTOsAsync()
        {
            var songs = await _repository.GetSongsAsync();
            var songDTOs = songs.Select(x=>x.AsSimpleSongDTO());

            return songDTOs == null ?
               NotFound()
               : Ok(songDTOs);
        }

        // GET api/<SimpleSongController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SimpleSongDTO>> GetSimpleSongDTOAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            return resultSong.AsSimpleSongDTO == null ?
                NotFound()
                : Ok(resultSong);
        }
    }
}
