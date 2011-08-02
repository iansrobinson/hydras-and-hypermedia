﻿using System;
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
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(new Rooms()), (instanceContext, obj) => { })
                .AddFormatters(AtomMediaType.Formatter);
            RouteTable.Routes.MapServiceRoute<RoomResource>("rooms", configuration);
        }
    }
}