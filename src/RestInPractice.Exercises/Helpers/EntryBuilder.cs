using System;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HydrasAndHypermedia.Server.Xhtml;

namespace HydrasAndHypermedia.Exercises.Helpers
{
    public class EntryBuilder
    {
        private readonly SyndicationItem entry;

        public EntryBuilder()
        {
            entry = new SyndicationItem();
        }

        public EntryBuilder WithBaseUri(Uri baseUri)
        {
            entry.BaseUri = baseUri;
            return this;
        }
        
        public EntryBuilder WithTitle(string title)
        {
            entry.Title = SyndicationContent.CreatePlaintextContent(title);
            return this;
        }

        public EntryBuilder WithForm(FormWriter form)
        {
            entry.Content = SyndicationContent.CreateXhtmlContent(form.ToXhtml());
            return this;
        }

        public EntryBuilder WithCategory(string category)
        {
            entry.Categories.Add(new SyndicationCategory(category));
            return this;
        }

        public EntryBuilder WithContinueLink(Uri uri)
        {
            entry.Links.Add(new SyndicationLink { Uri = uri, RelationshipType = "continue" });
            return this;
        }

        public EntryBuilder WithNorthLink(Uri uri)
        {
            entry.Links.Add(new SyndicationLink {Uri = uri, RelationshipType = "north"});
            return this;
        }

        public EntryBuilder WithSouthLink(Uri uri)
        {
            entry.Links.Add(new SyndicationLink {Uri = uri, RelationshipType = "south"});
            return this;
        }

        public EntryBuilder WithEastLink(Uri uri)
        {
            entry.Links.Add(new SyndicationLink {Uri = uri, RelationshipType = "east"});
            return this;
        }

        public EntryBuilder WithWestLink(Uri uri)
        {
            entry.Links.Add(new SyndicationLink {Uri = uri, RelationshipType = "west"});
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(new Utf8Writer(sb)))
            {
                new Atom10ItemFormatter(entry).WriteTo(writer);
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}