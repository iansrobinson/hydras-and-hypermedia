using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace HydrasAndHypermedia.Server.Hypermedia
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UriTemplateAttribute : Attribute, IServiceBehavior
    {
        private readonly UriFactoryWorker uriFactoryWorker;

        public UriTemplateAttribute(string routePrefix) : this(routePrefix, string.Empty)
        {
        }

        public UriTemplateAttribute(string routePrefix, string uriTemplate)
        {
            uriFactoryWorker = new UriFactoryWorker(routePrefix, uriTemplate);
        }

        public UriFactoryWorker UriFactoryWorker
        {
            get { return uriFactoryWorker; }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //Do nothing
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            //Do nothing
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var serviceType = serviceDescription.ServiceType;

            foreach (var endpoint in serviceDescription.Endpoints)
            {
                var contract = endpoint.Contract;

                foreach (var method in serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var behavior = GetWebAttribute(contract.Operations.Find(method.Name));
                    if (behavior != null)
                    {
                        behavior.UriTemplate = uriFactoryWorker.UriTemplateValue;
                    }
                }
            }
        }

        private static dynamic GetWebAttribute(OperationDescription operation)
        {
            return (from b in operation.Behaviors
                    where b.GetType().Equals(typeof (WebGetAttribute)) || b.GetType().Equals(typeof (WebInvokeAttribute))
                    select b).FirstOrDefault();
        }
    }
}