using System;
using System.Collections.Generic;
using System.Linq;

namespace HydrasAndHypermedia.Server.Hypermedia
{
    public class UriFactory
    {
        private readonly IDictionary<Type, UriFactoryWorker> workers;

        public UriFactory()
        {
            workers = new Dictionary<Type, UriFactoryWorker>();
        }

        public void Register<T>() where T : class
        {
            Register(typeof(T));
        }

        public void Register(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(UriTemplateAttribute), false);
            if (attributes.Length == 0)
            {
                throw new UriTemplateMissingException();
            }
            var worker = ((UriTemplateAttribute)attributes[0]).UriFactoryWorker;

            workers.Add(type, worker);
        }

        public string GetRoutePrefix<T>() where T :class
        {
            return For<T>().RoutePrefix;
        }

        public string GetUriTemplateValue<T>() where T : class
        {
            return For<T>().UriTemplateValue;
        }

        public string GetUriTemplateValueFor(Type resourceType)
        {
            return workers[resourceType].UriTemplateValue;
        }

        public Uri CreateBaseUri<T>(Uri uri) where T : class
        {
            return For<T>().CreateBaseUri(uri);
        }

        public Uri CreateAbsoluteUri<T>(Uri baseUri, params object[] values) where T : class
        {
            return For<T>().CreateAbsoluteUri(baseUri, values.Select(ConvertToString).ToArray());
        }

        public Uri CreateRelativeUri<T>(params object[] values) where T : class
        {
            return For<T>().CreateRelativeUri(values.Select(ConvertToString).ToArray());
        }

        private UriFactoryWorker For<T>() where T : class
        {
            return workers[typeof(T)];
        }

        private static string ConvertToString(object o)
        {
            if (o is Guid)
            {
                return ((Guid)o).ToString("N");
            }

            return o.ToString();
        }
    }
}