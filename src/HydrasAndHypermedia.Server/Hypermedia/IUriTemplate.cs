namespace HydrasAndHypermedia.Server.Hypermedia
{
    public interface IUriTemplate
    {
        string RoutePrefix { get; }
        string UriTemplateValue { get; }
    }
}