﻿using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using NUnit.Framework;
using RestInPractice.Server.Formatters;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part02_AtomMediaTypeFormatterTests
    {
        private const string FeedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">feed-title</title>
  <subtitle type=""text"">feed-description</subtitle>
  <id>feed-id</id>
  <updated>2011-09-05T00:00:00+01:00</updated>
  <link rel=""alternate"" href=""http://localhost/feed/alternate"" />
</feed>";

        private const string EntryXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<entry xmlns=""http://www.w3.org/2005/Atom"">
  <id>entry-id</id>
  <title type=""text"">entry-title</title>
  <updated>2011-09-05T00:00:00+01:00</updated>
  <link rel=""alternate"" href=""http://localhost/entry/alternate"" />
  <content type=""text"">entry-content</content>
</entry>";
            
        [Test]
        public void ShouldSupportAtomMediaType()
        {
            var formatter = AtomMediaType.Instance;
            Assert.IsNotNull(formatter.SupportedMediaTypes.FirstOrDefault(m => m.MediaType.Equals(AtomMediaType.Value)));
        }

        [Test]
        public void ShouldWriteFeedToStream()
        {
            var feed = new SyndicationFeed("feed-title", "feed-description", new Uri("http://localhost/feed/alternate"), "feed-id", new DateTimeOffset(new DateTime(2011, 9, 5)));
            var output = new MemoryStream();

            var formatter = AtomMediaType.Instance;
            formatter.WriteToStream(typeof(SyndicationFeedFormatter), new Atom10FeedFormatter(feed), output, null, null);

            output.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(output))
            {
                Assert.AreEqual(FeedXml, reader.ReadToEnd());
            }
        }

        [Test]
        public void ShouldReadFeedFromStream()
        {
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(FeedXml)))
            {
                var formatter = AtomMediaType.Instance;
                var feed = ((SyndicationFeedFormatter)formatter.ReadFromStream(typeof(SyndicationFeedFormatter), input, null)).Feed;

                Assert.AreEqual("feed-id", feed.Id);
                Assert.AreEqual("feed-title", feed.Title.Text);
                Assert.AreEqual("feed-description", feed.Description.Text);
                Assert.AreEqual(new Uri("http://localhost/feed/alternate"), feed.Links.First(l => l.RelationshipType.Equals("alternate")).Uri);
            }
        }

        [Test]
        public void ShouldWriteEntryToStream()
        {
            var entry = new SyndicationItem("entry-title", SyndicationContent.CreatePlaintextContent("entry-content"), new Uri("http://localhost/entry/alternate"), "entry-id", new DateTimeOffset(new DateTime(2011, 9, 5)));
            var output = new MemoryStream();

            var formatter = AtomMediaType.Instance;
            formatter.WriteToStream(typeof(SyndicationItemFormatter), new Atom10ItemFormatter(entry), output, null, null);

            output.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(output))
            {
                Assert.AreEqual(EntryXml, reader.ReadToEnd());
            }
        }

        [Test]
        public void ShouldReadEntryFromStream()
        {
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(EntryXml)))
            {
                var formatter = AtomMediaType.Instance;
                var entry = ((SyndicationItemFormatter)formatter.ReadFromStream(typeof(SyndicationItemFormatter), input, null)).Item;

                Assert.AreEqual("entry-id", entry.Id);
                Assert.AreEqual("entry-title", entry.Title.Text);
                
                Assert.AreEqual("entry-content", ((TextSyndicationContent)entry.Content).Text);
                Assert.AreEqual(new Uri("http://localhost/entry/alternate"), entry.Links.First(l => l.RelationshipType.Equals("alternate")).Uri);
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Expected to be called with type SyndicationItemFormatter or SyndicationFeedFormatter.")]
        public void ShouldThrowExceptionWhenAttemptingToWriteTypesOtherThanSyndicationItemAndSyndicationFeed()
        {
            var formatter = AtomMediaType.Instance;
            formatter.WriteToStream(typeof (String), new object(), new MemoryStream(), null, null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Expected to be called with type SyndicationItemFormatter or SyndicationFeedFormatter.")]
        public void ShouldThrowExceptionWhenAttemptingToReadTypesOtherThanSyndicationItemAndSyndicationFeed()
        {
            var formatter = AtomMediaType.Instance;
            formatter.ReadFromStream(typeof(String), new MemoryStream(), null);
        }
    }
}