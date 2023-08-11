using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public SongControllerTests()
        {
            _songRepository = Substitute.For<ISongRepository>();
            _sut = new SongController(_songRepository);
            _songMock = new SongMock();
        }

        [Fact]
        public async Task GetSongs_ShouldReturnStatusCode200AndSongs_WhenSongsAreInDatabase()
        {
            _songRepository.GetSongs().Returns(Task.FromResult<IEnumerable<Song>>(_songMock.SongGenerator.Generate(10)));

            _sut.ControllerContext = Substitute.For<ControllerContext>();

            var actionResult = await _sut.GetSongs();

            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = actionResult.Result as OkObjectResult;

            (result.Value as IEnumerable<SongDTO>).Should().NotBeNullOrEmpty();
            
            result.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetSongs_ShouldReturnStatusCode200AndSongs_WhenThereIsNoSongsInDataBase(bool isNull)
        {
            var songs = isNull ? null : Array.Empty<Song>();
            _songRepository.GetSongs().Returns(Task.FromResult<IEnumerable<Song>>(songs));

            _sut.ControllerContext = Substitute.For<ControllerContext>();

            var actionResult = await _sut.GetSongs();

            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = actionResult.Result as OkObjectResult;

            result.StatusCode.Value.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetSongs_ShouldReturnInternalServerError_WhenSongRepositoryThrowsAnError()
        {
            _songRepository.GetSongs().Throws(new Exception("Any Error"));

            _sut.ControllerContext = Substitute.For<ControllerContext>();

            var actionResult = await _sut.GetSongs();

            var result = actionResult.Result as ObjectResult;

            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}