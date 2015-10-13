using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Operations;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class OperationReflectionExtensionsTests
    {
        #region Mocking

        class TestOperation : IOperation<TestResponse>
        {
        }

        class TestResponse : OperationResponse
        {
        }

        class TestHandler : IOperationHandler<TestOperation>
        {
            public void Handle(TestOperation operation)
            {
                throw new System.NotImplementedException();
            }
        }

        class TestHandlerWithResponse : IOperationHandler<TestOperation, TestResponse>
        {
            public TestResponse Handle(TestOperation operation)
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

        [Test]
        public void ShouldGetResponseType()
        {
            typeof (TestOperation).GetResponseType().ShouldBe(typeof (TestResponse));
        }

        [Test]
        public void ShouldDetermineIfTypeIsOperationHandler()
        {
            typeof (TestOperation).IsOperationHandler().ShouldBeFalse();
            typeof (TestHandler).IsOperationHandler().ShouldBeTrue();
            typeof (TestHandlerWithResponse).IsOperationHandler().ShouldBeTrue();
        }

        [Test]
        public void ShouldGetHandlerInterfaceType()
        {
            typeof (TestHandler).GetHandlerInterfaceType().ShouldBe(typeof (IOperationHandler<TestOperation>));
            typeof (TestHandlerWithResponse).GetHandlerInterfaceType().ShouldBe(typeof (IOperationHandler<TestOperation, TestResponse>));
        }
    }
}