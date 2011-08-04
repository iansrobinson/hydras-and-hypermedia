using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part02_AtomMediaTypeTests
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
    }
}