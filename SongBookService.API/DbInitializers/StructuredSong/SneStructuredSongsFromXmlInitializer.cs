using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using SongBookService.API.Models.StructuredSong;

namespace SongBookService.API.DbInitializers.StructuredSong
{
    public class SneStructuredSongsFromXmlInitializer : IStructuredSongDbInitializer
    {
        private readonly string _filepath = @"snesongs.xml";
        public IEnumerable<Models.StructuredSong.StructuredSong> GetSongs()
        {
            if (!File.Exists(_filepath))
            {
                throw new InvalidDataException(Path.GetFullPath(_filepath));
            }

            XmlDocument doc = new();
            doc.Load(_filepath);
            var xmlSongs = doc.DocumentElement.SelectNodes(@"//SlideGroup");
            var songs = new Models.StructuredSong.StructuredSong[xmlSongs.Count];
            for (var i = 0; i < xmlSongs.Count; i++)
            {
                songs[i] = GetSong(xmlSongs[i]);
            }

            return songs;
        }
        private static Models.StructuredSong.StructuredSong GetSong(XmlNode xmlSong)
        {
            if (xmlSong is null)
            {
                throw new ArgumentNullException(nameof(xmlSong));
            }

            var songNumberString = xmlSong.SelectSingleNode(@".//Number")?.InnerText;
            var title = xmlSong.SelectSingleNode(@".//Title")?.InnerText;

            Models.StructuredSong.StructuredSong outputSong = new()
            {
                Author = null,
                Id = Guid.NewGuid(),
                Key = 0,
                OriginalTitle = null,
                Number = songNumberString,
                Title = title,
                Parts = GetParts(xmlSong.SelectNodes(@".//Slide"))
            };
            CollapseSlidesIntoParts(outputSong);
            MakePartUnique(outputSong);
            GetSlideOrder(outputSong);

            return outputSong;
        }

        private static void GetSlideOrder(Models.StructuredSong.StructuredSong outputSong)
        {
            IEnumerable<StructuredSlide> slides = new List<StructuredSlide>();
            foreach (var part in outputSong.PartOrder)
            {
                slides = slides.Concat(outputSong.Parts.First(x => x.Id == part).Slides);
            }
            outputSong.SlideOrder = slides.Select(x => x.Id);
        }

        private static void CollapseSlidesIntoParts(Models.StructuredSong.StructuredSong outputSong)
        {
            var collapsedParts = new List<StructuredPart>();
            var partGroupedByName = outputSong.Parts.GroupBy(x => x.Name).ToList();
            foreach (var partGroup in partGroupedByName)
            {
                var ids = partGroup.SelectMany(p => p.Slides.Select(s => s.Id));
                var collapsedPart = new StructuredPart()
                {
                    Name = partGroup.Select(p => p.Name).First(),
                    Id = Guid.NewGuid(),
                    Slides = partGroup.SelectMany(p => p.Slides).ToList(),
                };
                collapsedParts.Add(collapsedPart);
            }

            outputSong.Parts = collapsedParts;
        }

        private static void MakePartUnique(Models.StructuredSong.StructuredSong outputSong)
        {
            var uniqueParts = new List<StructuredPart>();
            var ids = new List<Guid>();
            foreach (var part in outputSong.Parts)
            {
                var duplicates = outputSong.Parts.Where(x => x.GetText().Equals(part.GetText())).ToArray();
                if (!uniqueParts.Any(x => x.Id == duplicates.First().Id))
                {
                    uniqueParts.Add(duplicates.First());
                }

                ids.Add(duplicates.First().Id);
            }

            outputSong.PartOrder = ids;
            outputSong.Parts = uniqueParts;
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
                Slides = new()
            };
            outputPart.Slides.Add(GetSlide(xmlSongPart.SelectSingleNode(@".//Text")?.InnerText));
            MakeSlidesUnique(outputPart);
            return outputPart;
        }

        private static void MakeSlidesUnique(StructuredPart part)
        {
            var ids = new List<Guid>();
            var uniqueSlides = new List<StructuredSlide>();
            foreach (var slide in part.Slides)
            {
                var duplicates = part.Slides.Where(x => x.Text.Equals(part.GetText())).ToArray();
                if (!uniqueSlides.Any(x => x.Id == duplicates.First().Id))
                {
                    uniqueSlides.Add(duplicates.First());
                }

                ids.Add(duplicates.First().Id);
            }
            part.Slides = uniqueSlides;
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
                Text = text
            };

            return outputSlide;
        }
    }
}
