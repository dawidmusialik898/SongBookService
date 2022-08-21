using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using SongBookService.API.Models.FullSong;

namespace SongBookService.API.DbInitializers
{
    public class SneSongsFromXmlInitializer : IDbInitializer
    {
        private readonly string _filepath = @"snesongs.xml";
        public IEnumerable<Song> GetSongs()
        {
            if (!File.Exists(_filepath))
            {
                throw new InvalidDataException(Path.GetFullPath(_filepath));
            }

            XmlDocument doc = new();
            doc.Load(_filepath);
            var xmlSongs = doc.DocumentElement.SelectNodes(@"//SlideGroup");
            var songs = new Song[xmlSongs.Count];
            for (var i = 0; i < xmlSongs.Count; i++)
            {
                songs[i] = GetSong(xmlSongs[i]);
            }

            return songs;
        }
        private static Song GetSong(XmlNode xmlSong)
        {
            if (xmlSong is null)
            {
                throw new ArgumentNullException(nameof(xmlSong));
            }

            var songNumberString = xmlSong.SelectSingleNode(@".//Number")?.InnerText;
            var title = xmlSong.SelectSingleNode(@".//Title")?.InnerText;

            Song outputSong = new()
            {
                Author = null,
                Id = Guid.NewGuid(),
                Key = 0,
                OriginalTitle = null,
                Tempo = null,
                Number = string.IsNullOrEmpty(songNumberString) ? null : new(songNumberString),
                Title = string.IsNullOrEmpty(title) ? null : new(title),
                DistinctParts = GetParts(xmlSong.SelectNodes(@".//Slide"))
            };
            CollapseSlidesIntoParts(outputSong);
            MakePartUnique(outputSong);
            return outputSong;
        }

        private static void CollapseSlidesIntoParts(Song outputSong)
        {
            var collapsedParts = new List<Part>();
            var partGroupedByName = outputSong.DistinctParts.GroupBy(x => x.Name).ToList();
            foreach (var partGroup in partGroupedByName)
            {
                var ids = partGroup.SelectMany(p => p.DistinctSlides.Select(s => s.Id));
                var collapsedPart = new Part()
                {
                    Name = partGroup.Select(p => p.Name).First(),
                    Id = Guid.NewGuid(),
                    DistinctSlides = partGroup.SelectMany(p => p.DistinctSlides).ToList(),
                    SlideOrder = ids.ToList()
                };
                collapsedParts.Add(collapsedPart);
            }

            outputSong.DistinctParts = collapsedParts;
        }

        private static void MakePartUnique(Song outputSong)
        {
            var uniqueParts = new List<Part>();
            var ids = new List<Guid>();
            foreach (var part in outputSong.DistinctParts)
            {
                var duplicates = outputSong.DistinctParts.Where(x => x.GetText().Equals(part.GetText())).ToArray();
                if (!uniqueParts.Any(x => x.Id == duplicates.First().Id))
                {
                    uniqueParts.Add(duplicates.First());
                }

                ids.Add(duplicates.First().Id);
            }

            outputSong.PartOrder = ids;
            outputSong.DistinctParts = uniqueParts;
        }

        private static List<Part> GetParts(XmlNodeList xmlSongParts)
        {
            if (xmlSongParts is null)
            {
                throw new ArgumentNullException(nameof(xmlSongParts));
            }

            List<Part> songParts = new();

            for (var i = 0; i < xmlSongParts.Count; i++)
            {
                songParts.Add(GetPart(xmlSongParts[i]));
            }

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
            MakeSlidesUnique(outputPart);
            return outputPart;
        }

        private static void MakeSlidesUnique(Part part)
        {
            var ids = new List<Guid>();
            var uniqueSlides = new List<Slide>();
            foreach (var slide in part.DistinctSlides)
            {
                var duplicates = part.DistinctSlides.Where(x => x.GetText().Equals(part.GetText())).ToArray();
                if (!uniqueSlides.Any(x => x.Id == duplicates.First().Id))
                {
                    uniqueSlides.Add(duplicates.First());
                }

                ids.Add(duplicates.First().Id);
            }

            part.SlideOrder = ids;
            part.DistinctSlides = uniqueSlides;
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
                Lines = GetLines(text)
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
            var lines = text.Split(Constants._newLineSymbols, StringSplitOptions.None);
            foreach (var line in lines)
            {
                outputLines.Add(GetLine(line));
            }

            return outputLines;
        }

        private static Line GetLine(string line)
        {
            var lyrics = new List<Lyrics>()
            {
                GetLyrics(line),
            };
            return new Line()
            {
                Id = Guid.NewGuid(),
                Chords = null,
                Lyrics = lyrics,
                Order = lyrics.Select(ly => ly.Id).ToList()
            };
        }

        private static Lyrics GetLyrics(string line)
        {
            return new Lyrics()
            {
                Id = Guid.NewGuid(),
                Text = line
            };
        }
    }
}
