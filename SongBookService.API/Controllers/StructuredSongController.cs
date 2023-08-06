using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using SongBookService.API.DTOs;
using SongBookService.API.Extensions.StructuredSongExtensions;
using SongBookService.API.Repository.StructuredSong;

namespace SongBookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StructuredSongController : ControllerBase
    {
        private readonly IStructuredSongRepository _repository;
        public StructuredSongController(IStructuredSongRepository repository) 
            => _repository = repository;

        /// GET: <SimpleSongsController>
        [HttpGet("SongItemList")]
        public async Task<ActionResult<IEnumerable<SongItemListDTO>>> GetSongListItemsAsync()
        {
            var result = await _repository.GetSongsAsync();
            return result is null ?
                NotFound()
                : Ok(result.Select(song => song.AsItemListDTO()).OrderBy(s => s.Number));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StructuredSongDTO>>> GetStructuredSongsDTOsAsync()
        {
            var result = await _repository.GetSongsAsync();
            return result is null ?
                NotFound()
                : Ok(result.Select(song => song.AsStructuredSongDTO()).OrderBy(s => s.Number));
        }

        // GET <SimpleSongsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StructuredSongDTO>> GetStructuredSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            var resultSongDTO = resultSong.AsStructuredSongDTO();
            return resultSongDTO is null ?
                NotFound()
                : Ok(resultSongDTO);
        }

        [HttpPost]
        public async Task<ActionResult> AddStructuredSongAsync([FromBody] StructuredSongDTO songDTO)
        {
            var dbsong = await _repository.GetSongAsync(songDTO.Id);
            if (dbsong is not null)
            {
                return BadRequest("Song with this id already exists in database.");
            }

            await _repository.AddSongAsync(songDTO.AsStructuredSong());
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> ModifyStructuredSongAsync([FromBody] StructuredSongDTO songDTO)
        {
            var dbsong = await _repository.GetSongAsync(songDTO.Id);
            var song = songDTO.AsStructuredSong();
            if (dbsong is not null)
            {
                await _repository.UpdateSongAsync(song);
            }
            else
            {
                await _repository.AddSongAsync(song);
            }
            return Ok();
        }

        /// DELETE <SimpleSongsController>/5
        [HttpDelete]
        public async Task<ActionResult> DeleteSongAsync(Guid id)
        {
            var result = await _repository.GetSongAsync(id);
            if (result is null)
            {
                return NotFound();
            }
            await _repository.DeleteSongAsync(id);
            return Ok();
        }
    }
}
