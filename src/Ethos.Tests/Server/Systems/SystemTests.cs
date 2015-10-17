using Ethos.Base.Infrastructure.Components;
using Ethos.Server.Infrastructure.Systems;
using NUnit.Framework;

namespace Ethos.Tests.Server.Systems
{
    [TestFixture]
    public class SystemTests
    {
        interface ITestSystemServer : IComponent
        {
            void Method();
        }

        interface ITestSystemClient : IComponent
        {
            void Method();
        }

        class TestSystemServer : ServerSystemBase<ITestSystemServer, ITestSystemClient>, ITestSystemServer
        {
            public void Method()
            {
            }
        }
    }
}