using System;

namespace SongBookService.API.Models.ValueObjects
{
    /// <summary>
    /// Song title.
    /// </summary>
    public record SongTitle
    {
        /// <summary>
        /// Song title as a string.
        /// </summary>
        public string Title { get; init; }
        public SongTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
            }
            if (title.Length > 50)
            {
                throw new ArgumentException("Given title is too long");
            }

            Title = title;
        }
    }
}