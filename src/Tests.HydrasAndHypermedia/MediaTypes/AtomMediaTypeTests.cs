using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using HydrasAndHypermedia.MediaTypes;
using NUnit.Framework;

namespace Tests.HydrasAndHypermedia.MediaTypes
{
    [TestFixture]
    public class AtomMediaTypeTests
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
            var formatter = AtomMediaType.Formatter;
            Assert.IsNotNull(formatter.SupportedMediaTypes.FirstOrDefault(m => m.Equals(AtomMediaType.Value)));
        }

        [Test]
        public void ShouldWriteFeedToStream()
        {
            var feed = new SyndicationFeed("feed-title", "feed-description", new Uri("http://localhost/feed/alternate"), "feed-id", new DateTimeOffset(new DateTime(2011, 9, 5)));
            var output = new MemoryStream();

            var formatter = AtomMediaType.Formatter;
            formatter.WriteToStream(typeof (SyndicationFeed), feed, output, null, null);

            output.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(output))
            {
                Assert.AreEqual(FeedXml, reader.ReadToEnd());
            }
        }

        [Test]
        public void ShouldWriteEntryToStream()
        {
            var entry = new SyndicationItem("entry-title", SyndicationContent.CreatePlaintextContent("entry-content"), new Uri("http://localhost/entry/alternate"), "entry-id", new DateTimeOffset(new DateTime(2011, 9, 5)));
            var output = new MemoryStream();

            var formatter = AtomMediaType.Formatter;
            formatter.WriteToStream(typeof (SyndicationItem), entry, output, null, null);

            output.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(output))
            {
                Assert.AreEqual(EntryXml, reader.ReadToEnd());
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidOperationException), ExpectedMessage = "Expected to be called with type SyndicationItem or SyndicationFeed.")]
        public void ShouldThrowExceptionWhenAttemptingToWriteTypesOtherThanSyndicationItemAndSyndicationFeed()
        {
            var formatter = AtomMediaType.Formatter;
            formatter.WriteToStream(typeof (String), new object(), new MemoryStream(), null, null);
        }

        [Test]
        public void ShouldReadFeedFromStream()
        {
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(FeedXml)))
            {
                var formatter = AtomMediaType.Formatter;
                var feed = (SyndicationFeed) formatter.ReadFromStream(typeof (SyndicationFeed), input, null);

                Assert.AreEqual("feed-id", feed.Id);
                Assert.AreEqual("feed-title", feed.Title.Text);
                Assert.AreEqual("feed-description", feed.Description.Text);
                Assert.AreEqual(new Uri("http://localhost/feed/alternate"), feed.Links.First(l => l.RelationshipType.Equals("alternate")).Uri);
            }
        }

        [Test]
        public void ShouldReadEntryFromStream()
        {
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(EntryXml)))
            {
                var formatter = AtomMediaType.Formatter;
                var entry = (SyndicationItem) formatter.ReadFromStream(typeof (SyndicationItem), input, null);

                Assert.AreEqual("entry-id", entry.Id);
                Assert.AreEqual("entry-title", entry.Title.Text);

                Assert.AreEqual("entry-content", ((TextSyndicationContent) entry.Content).Text);
                Assert.AreEqual(new Uri("http://localhost/entry/alternate"), entry.Links.First(l => l.RelationshipType.Equals("alternate")).Uri);
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidOperationException), ExpectedMessage = "Expected to be called with type SyndicationItem or SyndicationFeed.")]
        public void ShouldThrowExceptionWhenAttemptingToReadTypesOtherThanSyndicationItemAndSyndicationFeed()
        {
            var formatter = AtomMediaType.Formatter;
            formatter.ReadFromStream(typeof (String), new MemoryStream(), null);
        }

        [Test]
        public void ShouldBeAbleToReadFromAStreamThatHasAlreadyBeenRead()
        {
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(EntryXml)))
            {
                var buffer = new Byte[input.Length];
                input.Read(buffer, 0, buffer.Length);

                var formatter = AtomMediaType.Formatter;
                var entry = formatter.ReadFromStream(typeof (SyndicationItem), input, null);

                Assert.IsInstanceOf(typeof (SyndicationItem), entry);
            }
        }
    }
}