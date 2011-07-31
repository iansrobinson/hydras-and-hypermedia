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
            var rooms = new Rooms(
                new Room(1, "Entrance", "You descend a rope into a rubble-strewn hall. The air is cold and dank.", Exit.North(2), Exit.East(3), Exit.West(4)));
            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(rooms), (instanceContext, obj) => { })
                .AddFormatters(AtomFormatter.Instance);
            RouteTable.Routes.MapServiceRoute<RoomResource>("rooms", configuration);
        }
    }
}