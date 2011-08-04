using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part03_ClientTests
    {
        private static readonly Uri NorthUri = new Uri("/rooms/10", UriKind.Relative);
        private static readonly Uri BaseUri = new Uri("http://localhost:1234");

        
    }
}