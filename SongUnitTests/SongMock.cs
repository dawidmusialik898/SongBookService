using Bogus;

using SongBookService.API.Models;

namespace SongUnitTests
{
    internal class SongMock
    {
        private readonly Faker<Slide> _slideFaker;
        private readonly Faker<Part> _partFaker;
        public Faker<Song> SongGenerator { get; set; }

        public SongMock()
        {
            _slideFaker = new Faker<Slide>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.Text, f => f.Lorem.Lines(4));

            _partFaker = new Faker<Part>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Random.String(3))
                .RuleFor(p => p.Slides, f => _slideFaker.Generate(f.Random.Int(4, 10)))
                .RuleFor(p => p.SlideOrder, (f, p) => p.Slides.Select(s => s.Id).ToList());

            SongGenerator = new Faker<Song>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.Title, f => f.Person.FirstName)
                .RuleFor(s => s.Author, f => f.Person.FullName)
                .RuleFor(s => s.Key, f => f.Person.Gender.ToString())
                .RuleFor(s => s.Number, f => Random.Shared.Next().ToString())
                .RuleFor(s => s.OriginalTitle, f => f.Person.Company.Name)
                .RuleFor(s => s.Parts, f => _partFaker.Generate(f.Random.Int(4,10)))
                .RuleFor(s => s.PartOrder, (f,s) => s.Parts.Select(p =>p.Id).ToList());
        }
    }
}
