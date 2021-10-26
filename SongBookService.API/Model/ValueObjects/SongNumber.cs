using System;

namespace SongBookService.API.Model.ValueObjects
{
    /// <summary>
    /// Value object.
    /// Contains song number and possible prefix.
    /// </summary>
    public record SongNumber
    {
        /// <summary>
        /// Song number.
        /// Have to be grater than 0.
        /// </summary>
        public int Number { get; init; }
        /// <summary>
        /// Song number prefix.
        /// Up to 5 characters.
        /// </summary>
        public string Prefix { get; init; }

        public SongNumber(int number, string prefix)
        {
            if (number < 1)
            {
                throw new ArgumentException("Song number have to be above 0");
            }
            Number = number;
            if (prefix == null)
            {
                Prefix = prefix;
            }
            if (prefix.Length > 5)
            {
                throw new ArgumentException("Song number prefix can be up to 5 characters");
            }
        }
    }
}