using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using RestInPractice.Server.Xhtml;

namespace RestInPractice.Exercises.Helpers
{
    public class FeedBuilder
    {
        private readonly SyndicationFeed feed;

        public FeedBuilder()
        {
            feed = new SyndicationFeed();
        }

        public FeedBuilder WithForm(FormWriter form)
        {
            feed.ElementExtensions.Add(XmlReader.Create(new StringReader(form.ToXhtml())));
            return this;
        }

        public FeedBuilder WithCategory(string category)
        {
            feed.Categories.Add(new SyndicationCategory(category));
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(new Utf8Writer(sb)))
            {
                new Atom10FeedFormatter(feed).WriteTo(writer);
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}