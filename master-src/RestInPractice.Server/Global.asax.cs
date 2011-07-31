using System;
using System.Web;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Formatters;
using RestInPractice.Server.Resources;

namespace RestInPractice.Server
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(Rooms.Instance), (instanceContext, obj) => { })
                .AddFormatters(AtomMediaTypeFormatter.Instance);
            RouteTable.Routes.MapServiceRoute<RoomResource>("rooms", configuration);
        }
    }
}