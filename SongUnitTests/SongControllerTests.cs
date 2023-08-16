using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using NSubstitute.ReturnsExtensions;

using SongBookService.API.Controllers;
using SongBookService.API.DTOs;
using SongBookService.API.Extensions;
using SongBookService.API.Models;
using SongBookService.API.Repository;

using Xunit;

namespace SongUnitTests
{
    public class SongControllerTests
    {
        private readonly SongController _sut;
        private readonly ISongRepository _songRepository;
        private readonly SongMock _songMock;
        private readonly ILogger<SongController> _logger;

        public SongControllerTests()
        {
            _logger = Substitute.For<ILogger<SongController>>();
            _songRepository = Substitute.For<ISongRepository>();
            _sut = new SongController(_songRepository, _logger);
            _songMock = new SongMock();
        }

        [Fact]
        public async Task GetSongs_ShouldReturnStatusCode200AndSongs_WhenSongsAreInDatabase()
        {
            //arrange
            _songRepository.GetSongs().Returns(Task.FromResult<IEnumerable<Song>>(_songMock.SongGenerator.Generate(10)));

            //act
            var actionResult = await _sut.GetSongs();
            var result = actionResult.Result as OkObjectResult;

            //assert
            result.Should().NotBeNull();
            (result?.Value as IEnumerable<SongDTO>).Should().NotBeNullOrEmpty();
            result?.StatusCode?.Should().Be(StatusCodes.Status200OK);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)] //just to not write separate tests for [] and null
        public async Task GetSongs_ShouldReturnStatusCode200AndSongs_WhenThereIsNoSongsInDb(bool isNull)
        {
            //arrange
            var songs = isNull ? null : Array.Empty<Song>();
            _songRepository.GetSongs().Returns(Task.FromResult<IEnumerable<Song>>(songs));

            //act
            var actionResult = await _sut.GetSongs();
            var result = actionResult.Result as OkObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetSongs_ShouldReturnInternalServerError_WhenSongRepositoryThrowsAnError()
        {
            //arrange
            _songRepository.GetSongs().Throws(new Exception("Any Error"));

            //act
            var actionResult = await _sut.GetSongs();
            var result = actionResult.Result as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetSongById_ShouldReturnStatusCode200AndSong_WhenSongExistsInDb()
        {
            //arrange
            var guid = Guid.NewGuid();
            _songRepository.GetSong(guid).ReturnsForAnyArgs(Task.FromResult(_songMock.SongGenerator.Generate()));

            //act
            var actionResult = await _sut.GetSongById(guid);
            var result = actionResult.Result as OkObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
            (result?.Value as SongDTO?).Should().NotBeNull();
        }

        [Fact]
        public async Task GetSongById_ShouldReturnStatusCode404A_WhenSongDoesNotExistInDb()
        {
            //arrange
            var guid = Guid.NewGuid();
            _songRepository.GetSong(guid).ReturnsNullForAnyArgs();

            //act
            var actionResult = await _sut.GetSongById(guid);
            var result = actionResult.Result as NotFoundObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetSongById_ShouldReturnStatusCode500_WhenUnexpectedExceptionIsThrown()
        {
            //arrange
            var guid = Guid.NewGuid();
            _songRepository.GetSong(guid).ThrowsForAnyArgs(new Exception("Any Error"));

            //act
            var actionResult = await _sut.GetSongById(guid);
            var result = actionResult.Result as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task AddSong_ShouldReturnStatusCode400_WhenSongWithTheSameIdAlreadyExists()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsForAnyArgs(Task.FromResult(song));

            //act
            var actionResult = await _sut.AddSong(song.AsStructuredSongDTO());
            var badRequest = actionResult as BadRequestObjectResult;

            //assert
            badRequest.Should().NotBeNull();
            badRequest?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task AddSong_ShouldReturnStatusCode200_WhenWithTheSameIdDoesNotExistsAndNewSongIsAddedSuccessfully()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsNullForAnyArgs();

            //act
            var actionResult = await _sut.AddSong(song.AsStructuredSongDTO());
            var result = actionResult as OkResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task AddSong_ShouldReturnStatusCode500_WhenUnexpectedErrorHappensWhileTryingToGetSong()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).Throws(new Exception("Any Error"));

            //act
            var actionResult = await _sut.AddSong(song.AsStructuredSongDTO());
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task AddSong_ShouldReturnStatusCode500_WhenUnexpectedErrorHappensWhileTryingToAddSong()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsNullForAnyArgs();
            _songRepository.AddSong(song).ThrowsAsyncForAnyArgs(new Exception("Any Error"));

            //act
            var actionResult = await _sut.AddSong(song.AsStructuredSongDTO());
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task ModifySong_ShouldReturnStatusCode200_WhenSongDoesNotExistAndIsSuccesfullyAdded()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsForAnyArgs(Task.FromResult(song));

            //act
            var actionResult = await _sut.ModifySong(song.AsStructuredSongDTO());
            var result = actionResult as OkResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task ModifySong_ShouldReturnStatusCode200_WhenSongExistsAndIsSuccesfullyModified()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsNullForAnyArgs();

            //act
            var actionResult = await _sut.ModifySong(song.AsStructuredSongDTO());
            var result = actionResult as OkResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task ModifySong_ShouldReturnStatusCode500_WhenGetSongThrowsAnError()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ThrowsAsyncForAnyArgs(new Exception("Any Exception"));

            //act
            var actionResult = await _sut.ModifySong(song.AsStructuredSongDTO());
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task ModifySong_ShouldReturnStatusCode500_WhenSongDoesNotExistInDbButAddSongThrowsAnError()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsNullForAnyArgs();
            _songRepository.AddSong(song).ThrowsForAnyArgs(new Exception("Any Exception"));

            //act
            var actionResult = await _sut.ModifySong(song.AsStructuredSongDTO());
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task ModifySong_ShouldReturnStatusCode500_WhenSongExistsInDbButModifySongThrowsAnError()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsForAnyArgs(Task.FromResult(song));
            _songRepository.UpdateSong(song).ThrowsForAnyArgs(new Exception("Any Exception"));

            //act
            var actionResult = await _sut.ModifySong(song.AsStructuredSongDTO());
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task DeleteSong_ShouldReturnStatusCode200_WhenSongIsDeletedWithSuccess()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsForAnyArgs(Task.FromResult(song));

            //act
            var actionResult = await _sut.DeleteSong(song.Id);
            var result = actionResult as OkResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task DeleteSong_ShouldReturnStatusCode404_WhenSongWithGivenIdDoesNotExist()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsNullForAnyArgs();

            //act
            var actionResult = await _sut.DeleteSong(song.Id);
            var result = actionResult as NotFoundResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task DeleteSong_ShouldReturnStatusCode500_WhenGetSongThrowsAnError()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ThrowsForAnyArgs(new Exception("Any Exception"));

            //act
            var actionResult = await _sut.DeleteSong(song.Id);
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task DeleteSong_ShouldReturnStatusCode500_WhenDeleteSongThrowsAnError()
        {
            //arrange
            var song = _songMock.SongGenerator.Generate();
            _songRepository.GetSong(song.Id).ReturnsForAnyArgs(Task.FromResult(song));
            _songRepository.DeleteSong(song.Id).ThrowsForAnyArgs(new Exception("Any Exception"));
            //act
            var actionResult = await _sut.DeleteSong(song.Id);
            var result = actionResult as ObjectResult;

            //assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}