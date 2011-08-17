using System;
using System.Linq;
using HydrasAndHypermedia.Client;
using NUnit.Framework;

namespace Tests.HydrasAndHypermedia.Client
{
    [TestFixture]
    public class ApplicationStateInfoTests
    {
        [Test]
        public void FactoryMethodShouldCreateInstanceWithSuppliedEnduranceValue()
        {
            var info = ApplicationStateInfo.WithEndurance(5);
            Assert.AreEqual(5, info.Endurance);
        }

        [Test]
        public void HistoryShouldBeEmptyWhenNewlyCreated()
        {
            var info = ApplicationStateInfo.WithEndurance(5);
            Assert.AreEqual(0, info.History.Count());
        }

        [Test]
        public void BuilderReturnsNewApplicationStateInfoInstane()
        {
            var info = ApplicationStateInfo.WithEndurance(5);
            var newInfo = info.GetBuilder().Build();

            Assert.AreNotEqual(newInfo, info);
        }

        [Test]
        public void ShouldBeAbleToModifyEnduranceUsingBuilder()
        {
            var info = ApplicationStateInfo.WithEndurance(5);
            var newInfo = info.GetBuilder().UpdateEndurance(4).Build();

            Assert.AreEqual(4, newInfo.Endurance);
        }

        [Test]
        public void ShouldBeAbleToModifyHistoryUsingBuilder()
        {
            var uri1 = new Uri("http://localhost/rooms/1");
            var uri2 = new Uri("http://localhost/rooms/2");

            var info = ApplicationStateInfo.WithEndurance(5);
            var newInfo = info.GetBuilder().AddToHistory(uri1, uri2).Build();

            Assert.IsTrue(new[] {uri1, uri2}.SequenceEqual(newInfo.History));
        }
    }
}