using System;

namespace SongBookService.API.Model.ValueObjects
{
    /// <summary>
    /// Value object.
    /// Author could be a person or organisation.
    /// For example, Kari Jobe- person, or Hillsong- organisation.
    /// </summary>
    public record Author
    {
        /// <summary>
        /// First name of person, or name of organisation
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Surname of person, useless for organisation.
        /// </summary>
        public string Surname { get; init; }

        public Author(string name, string surname)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            if (name.Length > 30)
            {
                throw new ArgumentException("Given name is too long.");
            }
            if (surname?.Length > 30)
            {
                throw new ArgumentException("Given surname is too long");
            }
            Name = name;
            Surname = surname;
        }
    }
}