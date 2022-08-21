namespace SongBookService.API.Models.ValueObjects
{
    /// <summary>
    /// Value object.
    /// Represents part name.
    /// </summary>
    public record PartName
    {
        /// <summary>
        /// Part name.
        /// Up to 10 characters.
        /// </summary>
        public string Name { get; init; }
        public PartName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            if (name.Length > 10)
            {
                throw new System.ArgumentException("Name of song part have to be up to 10.");
            }

            Name = name;
        }
    }
}