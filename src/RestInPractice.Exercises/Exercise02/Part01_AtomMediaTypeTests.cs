using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using NUnit.Framework;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise02
{
    [TestFixture]
    public class Part01_AtomMediaTypeTests
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
    }
}