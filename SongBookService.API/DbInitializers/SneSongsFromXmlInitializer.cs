using SongBookService.API.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SongBookService.API.Model.ValueObjects;
using System.Linq;

namespace SongBookService.API.DbInitializers
{
    public class SneSongsFromXmlInitializer : IDbInitializer
    {
        private readonly string _filepath = @"snesongs.xml";
        public IEnumerable<Song> GetSongs()
        {
            if (!File.Exists(_filepath))
                throw new InvalidDataException(Path.GetFullPath(_filepath));

            XmlDocument doc = new();
            doc.Load(_filepath);
            XmlNodeList xmlSongs = doc.DocumentElement.SelectNodes(@"//SlideGroup");
            Song[] songs = new Song[xmlSongs.Count];
            for (int i = 0; i < xmlSongs.Count; i++)
                songs[i] = GetSong(xmlSongs[i]);

            return songs;
        }
        private static Song GetSong(XmlNode xmlSong)
        {
            if (xmlSong is null)
                throw new ArgumentNullException(nameof(xmlSong));

            SongNumber songNumber = new(xmlSong.SelectSingleNode(@".//Number")?.InnerText);
            var title = xmlSong.SelectSingleNode(@".//Title")?.InnerText;

            Song outputSong = new()
            {
                Author = null,
                Id = Guid.NewGuid(),
                Key = 0,
                OriginalTitle = null,
                Tempo = null,
                Number = songNumber,
                Title = new(string.IsNullOrEmpty(title) ? string.Empty : title),
                DistinctParts = GetParts(xmlSong.SelectNodes(@".//Slide"))
            };


            //MakeSlidesUnique(outputSong.GetSlideOccurences());
            //MakePartUnique(outputSong.PartOccurences);
            return outputSong;
        }

        private static List<Part> GetParts(XmlNodeList xmlSongParts)
        {
            if (xmlSongParts is null)
                throw new ArgumentNullException(nameof(xmlSongParts));

            List<Part> songParts = new();

            for (int i = 0; i < xmlSongParts.Count; i++)
                songParts.Add(GetPart(xmlSongParts[i]));

            return songParts;
        }

        private static Part GetPart(XmlNode xmlSongPart)
        {
            if (xmlSongPart is null)
            {
                throw new ArgumentNullException(nameof(xmlSongPart));
            }

            var partname = xmlSongPart.SelectSingleNode(@".//Part")?.InnerText;
            var outputPart = new Part()
            {
                Name = string.IsNullOrEmpty(partname) ? null : new(partname),
                Id = Guid.NewGuid(),
                DistinctSlides = new()
            };
            outputPart.DistinctSlides.Add(GetSlide(xmlSongPart.SelectSingleNode(@".//Text")?.InnerText));
            return outputPart;
        }

        private static Slide GetSlide(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or whitespace.", nameof(text));
            }

            var outputSlide = new Slide()
            {
                Id = Guid.NewGuid(),
                DistinctLines = GetLines(text)
            };

            return outputSlide;
        }

        private static List<Line> GetLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or whitespace.", nameof(text));
            }
            var outputLines = new List<Line>();
            var lines = text.Split(Constants.NewLineSymbols, StringSplitOptions.None);
            foreach (var line in lines)
            {
                outputLines.Add(GetLine(line));
            }
            return outputLines;
        }

        private static Line GetLine(string line)
        {
            return new Line()
            {
                Id = Guid.NewGuid(),
                Chords = null,
                Lyrics = new()
                {
                    new Lyrics()
                    {
                        Id = Guid.NewGuid(),
                        Text = line
                    }
                }
            };
        }
    }
}
