﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<SongDTO>>> GetSongsDTOsAsync()
        {
            var result = await _repository.GetSongsAsync();
            return result is null ?
                NotFound()
                : Ok(result.Select(song => song.AsStructuredSongDTO()).OrderBy(s => s.Number));
        }

        // GET <SimpleSongsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SongDTO>> GetSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            return resultSong is null ?
                NotFound()
                : Ok(resultSong.AsStructuredSongDTO());
        }

        [HttpPost]
        public async Task<ActionResult> AddSongAsync([FromBody] SongDTO songDTO)
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
        public async Task<ActionResult> ModifySongAsync([FromBody] SongDTO songDTO)
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