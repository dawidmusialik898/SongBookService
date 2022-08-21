using System;
using System.Linq;

namespace SongBookService.API.Models.ValueObjects
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

        public SongNumber(string songNumber)
        {
            //number is empty
            if (string.IsNullOrEmpty(songNumber))
            {
                throw new ArgumentNullException(nameof(songNumber));
            }

            var firstDigit = songNumber.First(x => char.IsDigit(x));
            //without prefix
            if (songNumber.IndexOf(firstDigit) == 0)
            {
                Number = int.Parse(songNumber);
                Prefix = null;
            }
            else //with prefix
            {
                Number = int.Parse(songNumber[songNumber.IndexOf(firstDigit)..]);
                Prefix = songNumber[..songNumber.IndexOf(firstDigit)];
            }

            ValidateSongNumber();
        }

        public SongNumber(int? songNumber)
        {
            Prefix = null;
            Number = songNumber ?? throw new ArgumentNullException(nameof(songNumber));
            ValidateSongNumber();
        }

        public string AsString() =>
            Prefix + Number;
        private void ValidateSongNumber()
        {
            if (Number < 1)
            {
                throw new ArgumentException("Song number have to be above 0");
            }
            else if (Prefix?.Length > 5)
            {
                throw new ArgumentException("Song number prefix can be up to 5 characters");
            }
        }
    }
}