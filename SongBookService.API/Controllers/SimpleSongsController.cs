using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SongBookService.API.DTOs;
using SongBookService.API.Extensions;
using SongBookService.API.Model.Entities;
using SongBookService.API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SongBookService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SimpleSongsController : ControllerBase
    {
        private readonly ISongRepository _repository;
        public SimpleSongsController(ISongRepository repository)
        {
            _repository = repository;
        }

        // GET: <SongsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongItemListDTO>>> GetSongListItemsAsync()
        {
            var result = await _repository.GetSongsAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result.Select(song=>song.AsItemListDTO()).OrderBy(s=>s.Number));
        }

        // GET <SongsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SimpleSongWithoutStructureDTO>> GetSimpleSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            var resultDTO = resultSong?.AsSimpleSongWithoutStructureDTO();
            if (resultDTO == null)
            {
                return NotFound();
            }
            return Ok(resultDTO);
        }

        // POST <SongsController>
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] SimpleSongWithoutStructureDTO simpleSong)
        {
            var song = await _repository.GetSongAsync(simpleSong.Id);
            if (song != null)
                return BadRequest("Song with this id already exists in database.");

            await _repository.AddSongAsync(simpleSong.AsSong());
            return Ok();
        }

        // DELETE <SongsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var result = await _repository.GetSongAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            await _repository.DeleteSongAsync(id);
            return Ok();
        }
    }
}
