using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System;
using Ethos.Tests.Infrastructure.Base;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class OperationServiceTests
    {
        #region Mocking

        class TestOperation : IOperation
        {
        }

        class TestOperationWithResponse : IOperation<TestResponse>
        {
        }

        class TestResponse : OperationResponseBase
        {
        }

        class TestOperationHandler : IOperationHandler<TestOperation>
        {
            public TestOperation LastHandledOperation { get; private set; }
            public int HandleInvokeCount { get; private set; }

            public void Handle(TestOperation operation)
            {
                LastHandledOperation = operation;
                HandleInvokeCount++;
            }
        }

        class TestOperationWithResponseHandler : IOperationHandler<TestOperationWithResponse, TestResponse>
        {
            public TestOperationWithResponse LastHandledOperation { get; private set; }
            public TestResponse LastReturnedResponse { get; private set; }

            public int HandleInvokeCount { get; private set; }

            public TestResponse Handle(TestOperationWithResponse operation)
            {
                LastHandledOperation = operation;
                LastReturnedResponse = new TestResponse();

                HandleInvokeCount++;
                return LastReturnedResponse;
            }
        }
        
        #endregion

        private ActiveOperationsManager _activeOperations;
        private InMemoryOperationWriter _writer;

        [SetUp]
        public void SetupTest()
        {
            _activeOperations = new ActiveOperationsManager();
            _writer = new InMemoryOperationWriter();
        }

        [Test]
        public void ShouldHandleOperation()
        {
            var handler = new TestOperationHandler();
            var service = new OperationService(_activeOperations, _writer, type => handler);

            var operation = new TestOperation();
            service.HandleOperation(operation);

            handler.LastHandledOperation.ShouldBe(operation);
            handler.HandleInvokeCount.ShouldBe(1);
        }

        [Test]
        public void ShouldHandleOperationWithResponse()
        {
            var handler = new TestOperationWithResponseHandler();
            var service = new OperationService(_activeOperations, _writer, type => handler);

            var operation = new TestOperationWithResponse();
            service.HandleOperationWithResponse(operation, 1);

            handler.LastHandledOperation.ShouldBe(operation);
            handler.LastReturnedResponse.PromiseId.ShouldBe((byte) 1);

            handler.HandleInvokeCount.ShouldBe(1);

            _writer.WrittenResponses.ShouldContain(handler.LastReturnedResponse);
        }

        [Test]
        public void ShouldHandleResponse()
        {
            var service = new OperationService(_activeOperations, _writer, null);
            var promise = _activeOperations.RegisterOperation(new TestOperationWithResponse());

            var response = new TestResponse {PromiseId = promise.Id};
            service.HandleResponse(response);

            promise.IsCompleted.ShouldBeTrue();
            promise.Response.ShouldBe(response);
        }
    }
}