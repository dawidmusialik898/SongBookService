using System;
using System.Collections.Generic;
using System.Linq;
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
    public class StructuredSongController : ControllerBase
    {
        private readonly IStructuredSongRepository _repository;
        private readonly IMapper _mapper;
        public StructuredSongController(IStructuredSongRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// GET: <SimpleSongsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongItemListDTO>>> GetSongListItemsAsync()
        {
            var result = await _repository.GetSongsAsync();
            return result == null ?
                NotFound()
                : Ok(result.Select(song => _mapper.Map<SongItemListDTO>(song)).OrderBy(s => s.Number));
        }

        // GET <SimpleSongsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StructuredSongDTO>> GetSimpleSongByIdAsync(Guid id)
        {
            var resultSong = await _repository.GetSongAsync(id);
            return _mapper.Map<StructuredSongDTO>(resultSong) == null ?
                NotFound()
                : Ok(resultSong);
        }

        // POST <SimpleSongsController>
        [HttpPost]
        public async Task<ActionResult> AddFullSongAsync([FromBody] StructuredSongDTO songDTO)
        {
            var dbsong = await _repository.GetSongAsync(songDTO.Id);
            if (dbsong != null)
            {
                return BadRequest("Song with this id already exists in database.");
            }

            await _repository.AddSongAsync(_mapper.Map<StructuredSong>(songDTO));
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> ModifyFullSongAsync([FromBody] StructuredSongDTO songDTO)
        {
            var dbsong = await _repository.GetSongAsync(songDTO.Id);
            var song = _mapper.Map<StructuredSong>(songDTO);
            if (dbsong != null)
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
            if (result == null)
            {
                return NotFound();
            }
            await _repository.DeleteSongAsync(id);
            return Ok();
        }
    }
}
