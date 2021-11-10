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
    public class SongsController : ControllerBase
    {
        private readonly ISongRepository _repository;
        public SongsController(ISongRepository repository)
        {
            _repository = repository;
        }

        #region Common
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
        #endregion common

        #region SimpleSong

        // GET <SongsController>/5
        [HttpGet("SimpleSong/{id}")]
        public async Task<ActionResult<SimpleSongDTO>> GetSimpleSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            var resultDTO = resultSong?.AsSimpleSongDTO();
            if (resultDTO == null)
            {
                return NotFound();
            }
            return Ok(resultDTO);
        }

        // POST <SongsController>
        [HttpPost("SimpleSong")]
        public async Task<ActionResult> PostAsync([FromBody] SimpleSongDTO simpleSong)
        {
            var song = await _repository.GetSongAsync(simpleSong.Id);
            if (song != null)
                return BadRequest("Song with this id already exists in database.");

            await _repository.AddSongAsync(simpleSong.AsSong());
            return Ok();
        }
        #endregion SimpleSong

        #region FullSong
        // GET <SongsController>/5
        [HttpGet("FullSong/{id}")]
        public async Task<ActionResult<Song>> GetFullSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            if (resultSong == null)
            {
                return NotFound();
            }
            return Ok(resultSong);
        }
        // POST <SongsController>
        [HttpPost("FullSong")]
        public async Task<ActionResult> AddFullSongAsync([FromBody] Song song)
        {
            var dbsong = await _repository.GetSongAsync(song.Id);
            if (dbsong != null)
                return BadRequest("Song with this id already exists in database.");

            await _repository.AddSongAsync(song);
            return Ok();
        }
        #endregion FullSong

        #region StructuredSong
        [HttpGet("StrucutredSong/{id}")]
        public async Task<ActionResult<StructuredSongDTO>> GetStructuredSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            if (resultSong == null)
            {
                return NotFound();
            }
            return Ok(resultSong.AsStructuredSong());
        }

        // POST <SongsController>
        [HttpPost("StructuredSong")]
        public async Task<ActionResult> AddStructuredSongAsync([FromBody] StructuredSongDTO structuredSong)
        {
            var dbsong = await _repository.GetSongAsync(structuredSong.Id);
            if (dbsong != null)
                return BadRequest("Song with this id already exists in database.");

            await _repository.AddSongAsync(structuredSong.AsSong());
            return Ok();
        }
        #endregion StructuredSong
    }
}
