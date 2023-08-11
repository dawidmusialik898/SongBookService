using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SongBookService.API.DTOs;
using SongBookService.API.Extensions;
using SongBookService.API.Repository;

namespace SongBookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongRepository _repository;
        public SongController(ISongRepository repository)
            => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDTO>>> GetSongs()
        {
            try
            {
                var songs = await _repository.GetSongs();
                return Ok(songs?.Select(song => song.AsStructuredSongDTO()).OrderBy(s => s.Number));
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET <SimpleSongsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SongDTO>> GetSongById(Guid id)
        {
            var resultSong = await _repository.GetSong(id);
            return resultSong is null ?
                NotFound()
                : Ok(resultSong.AsStructuredSongDTO());
        }

        [HttpPost]
        public async Task<ActionResult> AddSong([FromBody] SongDTO songDTO)
        {
            var dbsong = await _repository.GetSong(songDTO.Id);
            if (dbsong is not null)
            {
                return BadRequest("Song with this id already exists in database.");
            }

            await _repository.AddSong(songDTO.AsStructuredSong());
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> ModifySong([FromBody] SongDTO songDTO)
        {
            var dbsong = await _repository.GetSong(songDTO.Id);
            var song = songDTO.AsStructuredSong();
            if (dbsong is not null)
            {
                await _repository.UpdateSong(song);
            }
            else
            {
                await _repository.AddSong(song);
            }
            return Ok();
        }

        /// DELETE <SimpleSongsController>/5
        [HttpDelete]
        public async Task<ActionResult> DeleteSong(Guid id)
        {
            var result = await _repository.GetSong(id);
            if (result is null)
            {
                return NotFound();
            }
            await _repository.DeleteSong(id);
            return Ok();
        }
    }
}
