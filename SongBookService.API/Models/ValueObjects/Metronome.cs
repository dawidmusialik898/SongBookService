using System;

namespace SongBookService.API.Models.ValueObjects
{
    /// <summary>
    /// Value object.
    /// Contains metronome cadence.
    /// </summary>
    public record Metronome
    {
        /// <summary>
        /// Song cadence.
        /// Have to be grater than 0.
        /// </summary>
        public int Cadence { get; init; }
        public Metronome(int cadence)
        {
            if (cadence < 1)
            {
                throw new ArgumentException("Cadence cannot not be below 1");
            }
            Cadence = cadence;
        }
    }
}