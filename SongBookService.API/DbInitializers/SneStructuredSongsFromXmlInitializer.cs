using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using SongBookService.API.Models.StructuredSong;

namespace SongBookService.API.DbInitializers
{
    public class SneStructuredSongsFromXmlInitializer : IStructuredSongDbInitializer
    {
        private readonly string _filepath = @"snesongs.xml";
        public IEnumerable<StructuredSong> GetSongs()
        {
            if (!File.Exists(_filepath))
            {
                throw new InvalidDataException(Path.GetFullPath(_filepath));
            }

            XmlDocument doc = new();
            doc.Load(_filepath);
            var xmlSongs = doc.DocumentElement.SelectNodes(@"//SlideGroup");
            var songs = new StructuredSong[xmlSongs.Count];
            for (var i = 0; i < xmlSongs.Count; i++)
            {
                songs[i] = GetSong(xmlSongs[i]);
            }

            return songs;
        }
        private static StructuredSong GetSong(XmlNode xmlSong)
        {
            if (xmlSong is null)
            {
                throw new ArgumentNullException(nameof(xmlSong));
            }

            var songNumberString = xmlSong.SelectSingleNode(@".//Number")?.InnerText;
            var title = xmlSong.SelectSingleNode(@".//Title")?.InnerText;

            StructuredSong outputSong = new()
            {
                Author = null,
                Id = Guid.NewGuid(),
                Key = 0,
                OriginalTitle = null,
                Number = songNumberString,
                Title = title,
                DistinctParts = GetParts(xmlSong.SelectNodes(@".//Slide"))
            };
            CollapseSlidesIntoParts(outputSong);
            MakePartUnique(outputSong);
            GetSlideOrder(outputSong);

            return outputSong;
        }

        private static void GetSlideOrder(StructuredSong outputSong)
        {
            IEnumerable<StructuredSlide> slides = new List<StructuredSlide>();
            foreach (var part in outputSong.PartOrder)
            {
                slides =slides.Concat(outputSong.DistinctParts.First(x => x.Id == part).DistinctSlides);
            }
            outputSong.SlideOrder = slides.Select(x => x.Id);
        }

        private static void CollapseSlidesIntoParts(StructuredSong outputSong)
        {
            var collapsedParts = new List<StructuredPart>();
            var partGroupedByName = outputSong.DistinctParts.GroupBy(x => x.Name).ToList();
            foreach (var partGroup in partGroupedByName)
            {
                var ids = partGroup.SelectMany(p => p.DistinctSlides.Select(s => s.Id));
                var collapsedPart = new StructuredPart()
                {
                    Name = partGroup.Select(p => p.Name).First(),
                    Id = Guid.NewGuid(),
                    DistinctSlides = partGroup.SelectMany(p => p.DistinctSlides).ToList(),
                };
                collapsedParts.Add(collapsedPart);
            }

            outputSong.DistinctParts = collapsedParts;
        }

        private static void MakePartUnique(StructuredSong outputSong)
        {
            var uniqueParts = new List<StructuredPart>();
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

        private static List<StructuredPart> GetParts(XmlNodeList xmlSongParts)
        {
            if (xmlSongParts is null)
            {
                throw new ArgumentNullException(nameof(xmlSongParts));
            }

            List<StructuredPart> songParts = new();

            for (var i = 0; i < xmlSongParts.Count; i++)
            {
                songParts.Add(GetPart(xmlSongParts[i]));
            }

            return songParts;
        }

        private static StructuredPart GetPart(XmlNode xmlSongPart)
        {
            if (xmlSongPart is null)
            {
                throw new ArgumentNullException(nameof(xmlSongPart));
            }

            var partname = xmlSongPart.SelectSingleNode(@".//Part")?.InnerText;
            var outputPart = new StructuredPart()
            {
                Name = partname,
                Id = Guid.NewGuid(),
                DistinctSlides = new()
            };
            outputPart.DistinctSlides.Add(GetSlide(xmlSongPart.SelectSingleNode(@".//Text")?.InnerText));
            MakeSlidesUnique(outputPart);
            return outputPart;
        }

        private static void MakeSlidesUnique(StructuredPart part)
        {
            var ids = new List<Guid>();
            var uniqueSlides = new List<StructuredSlide>();
            foreach (var slide in part.DistinctSlides)
            {
                var duplicates = part.DistinctSlides.Where(x => x.Text.Equals(part.GetText())).ToArray();
                if (!uniqueSlides.Any(x => x.Id == duplicates.First().Id))
                {
                    uniqueSlides.Add(duplicates.First());
                }

                ids.Add(duplicates.First().Id);
            }
            part.DistinctSlides = uniqueSlides;
        }

        private static StructuredSlide GetSlide(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or whitespace.", nameof(text));
            }

            var outputSlide = new StructuredSlide()
            {
                Id = Guid.NewGuid(),
                Text= text
            };

            return outputSlide;
        }
    }
}
