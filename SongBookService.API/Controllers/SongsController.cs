//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Mvc;

//using SongBookService.API.DTOs;
//using SongBookService.API.Extensions.FullSongExtensions;
//using SongBookService.API.Models.FullSong;
//using SongBookService.API.Repository.FullSong;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace SongBookService.API.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class SongsController : ControllerBase
//    {
//        private readonly ISongRepository _repository;
//        public SongsController(ISongRepository repository)
//            => _repository = repository;

//        /// GET: <SongsController>
//        [HttpGet("SongItemList")]
//        public async Task<ActionResult<IEnumerable<SongItemListDTO>>> GetSongListItemsAsync()
//        {
//            var result = await _repository.GetSongsAsync();
//            return result == null ?
//                NotFound()
//                : Ok(result.Select(song => song.AsItemListDTO()).OrderBy(s => s.Number));
//        }

//        // GET <SongsController>/5
//        [HttpGet("FullSong/{id}")]
//        public async Task<ActionResult<Song>> GetFullSongByIdAsync(Guid id)
//        {
//            var resultSong = await _repository.GetSongAsync(id);
//            return resultSong == null ?
//                NotFound()
//                : Ok(resultSong);
//        }
//        // POST <SongsController>
//        [HttpPost("FullSong")]
//        public async Task<ActionResult> AddFullSongAsync([FromBody] Song song)
//        {
//            var dbsong = await _repository.GetSongAsync(song.Id);
//            if (dbsong != null)
//            {
//                return BadRequest("Song with this id already exists in database.");
//            }
//            //need to add check with number- cannot have two songs with the same number.
//            //or meyby number and id should be generated??

//            await _repository.AddSongAsync(song);
//            return Ok();
//        }

//        [HttpPut("FullSong")]
//        public async Task<ActionResult> ModifyFullSongAsync([FromBody] Song song)
//        {
//            var dbsong = await _repository.GetSongAsync(song.Id);
//            if (dbsong != null)
//            {
//                await _repository.UpdateSongAsync(song);
//            }

//            await _repository.AddSongAsync(song);
//            return Ok();
//        }

//        /// DELETE <SongsController>/5
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> DeleteSongAsync(Guid id)
//        {
//            var result = await _repository.GetSongAsync(id);
//            if (result == null)
//            {
//                return NotFound();
//            }
//            await _repository.DeleteSongAsync(id);
//            return Ok();
//        }
//    }
//}
