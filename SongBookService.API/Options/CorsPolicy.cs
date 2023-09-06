using System;

namespace SongBookService.API.Options
{
    public class CorsPolicy
    {
        public string Name { get; set; } = string.Empty;
        public string[] Origins { get; set; } = Array.Empty<string>();
    }
}
