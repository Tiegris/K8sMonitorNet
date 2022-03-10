using Pinger;
using NUnit.Framework;

namespace EndpointPingerTests
{
    public class EndpointTests_1
    {
        [SetUp]
        public void Setup() {

        }

        [Test]
        public void Test1() {
            Endpoint ep = new Endpoint {
                FailureThreshold = 0,
            };
            ep.Fail();
            Assert.AreEqual(StatusType.Dead, ep.Status);
            ep.Fail();
            Assert.AreEqual(StatusType.Dead, ep.Status);
            ep.Success();
            Assert.AreEqual(StatusType.Healthy, ep.Status);
        }

        [Test]
        public void Test2() {
            Endpoint ep = new Endpoint {
                FailureThreshold = 1,
            };
            ep.Fail();
            Assert.AreEqual(StatusType.Dying, ep.Status);
            ep.Fail();
            Assert.AreEqual(StatusType.Dead, ep.Status);
            ep.Success();
            Assert.AreEqual(StatusType.Recovering, ep.Status);
            ep.Success();
            Assert.AreEqual(StatusType.Healthy, ep.Status);
        }


        [Test]
        public void Test3() {
            Endpoint ep = new Endpoint {
                FailureThreshold = 1,
            };
            ep.Success();
            Assert.AreEqual(StatusType.Healthy, ep.Status);
            ep.Success();
            Assert.AreEqual(StatusType.Healthy, ep.Status);
            ep.Fail();
            Assert.AreEqual(StatusType.Dying, ep.Status);
            ep.Success();
            Assert.AreEqual(StatusType.Healthy, ep.Status);
        }

    }
}