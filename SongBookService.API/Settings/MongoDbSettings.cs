namespace SongBookService.API.Settings
{
    internal class MongoDbSettings
    {
        internal string Host { get; set; }
        internal int Port { get; set; }
        internal string User { get; set; }
        internal string Password { get; set; }

        internal string ConnectionString
        {
            get
            {
                return $"mongodb://{User}:{Password}@{Host}:{Port}";
            }
        }
    }
}