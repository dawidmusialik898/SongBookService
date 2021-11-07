﻿using Microsoft.AspNetCore.Mvc;
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

        // GET: <SongsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongItemListDTO>>> GetSongsAsync()
        {
            var result = await _repository.GetSongsAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result.Select(song=>song.AsItemListDTO()));
        }

        // GET <SongsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetFullSongByIdAsync(Guid id)
        {
            var result = await _repository.GetSongAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST <SongsController>
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] Song song)
        {
            await _repository.AddSongAsync(song);
            return Ok();
        }

        // PUT <SongsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody] Song song)
        {
            await _repository.UpdateSongAsync(song);
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
