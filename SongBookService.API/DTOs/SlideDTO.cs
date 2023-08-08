using System;

namespace SongBookService.API.DTOs
{
    public readonly record struct SlideDTO
    {
        public Guid Id { get; init; }
        public string Text { get; init; }
    }
}
