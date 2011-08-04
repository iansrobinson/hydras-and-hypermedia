using System;
using System.Web;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Server
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(new Rooms()), (instanceContext, obj) => { });
            
            // Workaround for serialization issue in Preview 4. 
            // Must clear default XML formatter from Formatters before adding Atom formatter.
            var hostConfiguration = (HttpHostConfiguration)configuration;
            hostConfiguration.OperationHandlerFactory.Formatters.Clear();
            hostConfiguration.OperationHandlerFactory.Formatters.Insert(0, AtomMediaType.Formatter);

            RouteTable.Routes.MapServiceRoute<RoomResource>("rooms", configuration);
        }
    }
}