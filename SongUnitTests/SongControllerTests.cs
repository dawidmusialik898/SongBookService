using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using SongBookService.API.Controllers;
using SongBookService.API.DTOs;
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
            _songRepository.GetSongs().Returns(Task.FromResult<IEnumerable<Song>>(_songMock.SongGenerator.Generate(10)));

            var actionResult = await _sut.GetSongs();

            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = actionResult.Result as OkObjectResult;

            (result.Value as IEnumerable<SongDTO>).Should().NotBeNullOrEmpty();

            result.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)] //just to not write separate tests for [] and null
        public async Task GetSongs_ShouldReturnStatusCode200AndSongs_WhenThereIsNoSongsInDb(bool isNull)
        {
            var songs = isNull ? null : Array.Empty<Song>();

            _songRepository.GetSongs().Returns(Task.FromResult<IEnumerable<Song>>(songs));

            var actionResult = await _sut.GetSongs();

            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = actionResult.Result as OkObjectResult;

            result.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetSongs_ShouldReturnInternalServerError_WhenSongRepositoryThrowsAnError()
        {
            _songRepository.GetSongs().Throws(new Exception("Any Error"));

            var actionResult = await _sut.GetSongs();

            var result = actionResult.Result as ObjectResult;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetSongById_ShouldReturnStatusCode200AndSong_WhenSongExistsInDb()
        {
            var guid = Guid.NewGuid();

            _songRepository.GetSong(guid).ReturnsForAnyArgs(Task.FromResult(_songMock.SongGenerator.Generate()));

            var actionResult = await _sut.GetSongById(guid);

            var result = actionResult.Result as OkObjectResult;

            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            (result.Value as SongDTO?).Should().NotBeNull();
        }
    }
}