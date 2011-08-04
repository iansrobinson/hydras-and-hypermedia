using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace RestInPractice.Exercises.Helpers
{
    public class FeedBuilder
    {
        private readonly SyndicationFeed feed;

        public FeedBuilder()
        {
            feed = new SyndicationFeed();
        }

        public FeedBuilder WithCategory(string category)
        {
            feed.Categories.Add(new SyndicationCategory(category));
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                new Atom10FeedFormatter(feed).WriteTo(writer);
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}