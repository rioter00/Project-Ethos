using System;
using System.Collections.Generic;
using System.Linq;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Serialization;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base
{
    [TestFixture]
    public class OperationProcessorTests
    {
        #region Mocking

        class TestTransport : INetworkTransport
        {
            public IList<Tuple<OperationCode, IDictionary<byte, object>>> SentOperations { get; }

            public TestTransport()
            {
                SentOperations = new List<Tuple<OperationCode, IDictionary<byte, object>>>();
            }

            public void SendOperation(OperationCode code, IDictionary<byte, object> parameters)
            {
                SentOperations.Add(new Tuple<OperationCode, IDictionary<byte, object>>(code, parameters));
            }

            public void Disconnect()
            {
            }
        }

        class TestOperation : IOperation
        {
        }

        class TestOperationWithResponse : IOperation<TestResponse>
        {
            public string Data { get; set; }
        }

        class TestResponse : OperationResponse
        {
            public string Data { get; set; }
            public int MethodInvokeCount { get; set; }

            public void Method()
            {
                MethodInvokeCount++;
            }
        }

        class TestOperationHandler : IOperationHandler<TestOperation>
        {
            public int HandleInvokeCount { get; private set; }

            public void Handle(TestOperation operation)
            {
                HandleInvokeCount++;
            }
        }

        class TestOperationHandlerWithResponse : IOperationHandler<TestOperationWithResponse, TestResponse>
        {
            public int HandleInvokeCount { get; private set; }

            public TestResponse Handle(TestOperationWithResponse operation)
            {
                HandleInvokeCount++;

                var response = new TestResponse {Data = operation.Data};
                response.AddModalError("asdf", "asdfasdf");

                return response;
            }
        }

        #endregion

        private ProtobufSerializer _serializer;
        private OperationMap _map;

        private TestTransport _transport;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _serializer = new ProtobufSerializer();
            _serializer.RegisterTypes(new[]
            {
                typeof (TestOperation),
                typeof (TestOperationWithResponse),
                typeof (OperationResponse),
                typeof (TestResponse)
            });

            _map = new OperationMap();
            _map.MapOperationsInAssembly(typeof (TestOperation).Assembly);
        }

        [SetUp]
        public void SetupTest()
        {
            _transport = new TestTransport();
        }

        [Test]
        public void ShouldWriteOperation()
        {
            var processor = new OperationProcessor(_serializer, _map, _transport, null);

            processor.WriteOperation(new TestOperation());

            var sentOperation = _transport.SentOperations.SingleOrDefault(t => t.Item1 == OperationCode.HandleOperation);
            sentOperation.ShouldNotBeNull();

            sentOperation.Item2.Keys.ShouldContain((byte) OperationParameterCode.OperationId);
            sentOperation.Item2.Keys.ShouldContain((byte) OperationParameterCode.OperationData);
        }

        [Test]
        public void ShouldWriteOperationWithResponse()
        {
            var processor = new OperationProcessor(_serializer, _map, _transport, null);

            var operation = new TestOperationWithResponse {Data = "asdf"};
            var promise = processor.WriteOperationWithResponse(operation);

            processor.ActiveOperations.ShouldContain(operation);
            promise.ShouldBeOfType<OperationPromise<TestResponse>>();

            var sentOperation = _transport.SentOperations.SingleOrDefault(t => t.Item1 == OperationCode.HandleOperationWithResponse);
            sentOperation.ShouldNotBeNull();

            sentOperation.Item2.Keys.ShouldContain((byte) OperationParameterCode.OperationId);
            sentOperation.Item2.Keys.ShouldContain((byte) OperationParameterCode.OperationData);
            sentOperation.Item2.Keys.ShouldContain((byte) OperationParameterCode.OperationResponseId);
        }

        [Test]
        public void ShouldWriteOperationResponse()
        {
            var processor = new OperationProcessor(_serializer, _map, _transport, null);

            processor.WriteResponse(0, 0, new TestResponse {Data = "asdf"});

            var sentOperation = _transport.SentOperations.SingleOrDefault(t => t.Item1 == OperationCode.HandleOperationResponse);
            sentOperation.ShouldNotBeNull();

            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationId, (byte) 0);
            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationResponseId, (byte) 0);

            sentOperation.Item2.Keys.ShouldContain((byte) OperationParameterCode.OperationResponseData);
        }

        [Test]
        public void ShouldReadOperation()
        {
            var handler = new TestOperationHandler();
            var processor = new OperationProcessor(_serializer, _map, _transport, t => handler);

            processor.WriteOperation(new TestOperation());
            processor.ReadOperation(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperation).Item2);

            handler.HandleInvokeCount.ShouldBe(1);
        }

        [Test]
        public void ShouldReadOperationWithResponse()
        {
            var handler = new TestOperationHandlerWithResponse();
            var processor = new OperationProcessor(_serializer, _map, _transport, t => handler);

            var operation = new TestOperationWithResponse {Data = "asdf"};
            processor.WriteOperationWithResponse(operation);

            processor.ReadOperationWithResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationWithResponse).Item2);

            _transport.SentOperations.ShouldContain(t => t.Item1 == OperationCode.HandleOperationResponse);
            handler.HandleInvokeCount.ShouldBe(1);
        }

        [Test]
        public void ShouldReadOperationResponse()
        {
            var processor = new OperationProcessor(_serializer, _map, _transport, t => new TestOperationHandlerWithResponse());
            var operation = new TestOperationWithResponse {Data = "asdf"};

            var promise = processor.WriteOperationWithResponse(operation);
            processor.ReadOperationWithResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationWithResponse).Item2);

            processor.ReadResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationResponse).Item2);

            promise.IsCompleted.ShouldBeTrue();
            processor.ActiveOperations.ShouldBeEmpty();
        }

        [Test]
        public void ShouldInvokeCallback()
        {
            var processor = new OperationProcessor(_serializer, _map, _transport, t => new TestOperationHandlerWithResponse());

            TestResponse response = null;
            processor.WriteOperationWithResponse(new TestOperationWithResponse {Data= "asdf"})
                .Then(t =>
                {
                    t.Method();
                    response = t;
                });

            processor.ReadOperationWithResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationWithResponse).Item2);
            processor.ReadResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationResponse).Item2);

            response.MethodInvokeCount.ShouldBe(1);
        }

        [Test]
        public void ShouldMarkResponseAsInvalidWithModalErrors()
        {
            var processor = new OperationProcessor(_serializer, _map, _transport, t => new TestOperationHandlerWithResponse());

            TestResponse response = null;
            processor.WriteOperationWithResponse(new TestOperationWithResponse {Data = "asdf"})
                .Then(t =>
                {
                    t.Method();
                    response = t;
                });

            processor.ReadOperationWithResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationWithResponse).Item2);
            processor.ReadResponse(_transport.SentOperations.Single(t => t.Item1 == OperationCode.HandleOperationResponse).Item2);

            response.ModalErrors.ShouldBe("asdf: asdfasdf");
            response.IsValid.ShouldBeFalse();
        }
    }
}