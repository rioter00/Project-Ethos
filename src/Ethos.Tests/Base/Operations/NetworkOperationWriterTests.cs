using System.Linq;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Operations.Networking;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Tests.Infrastructure.Base;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class NetworkOperationWriterTests
    {
        #region Mocking

        class TestOperation : IOperation
        {
        }

        class TestOperationWithResponse : IOperation<TestResponse>
        {
        }

        class TestResponse : OperationResponse
        {
        }

        #endregion

        private OperationMap _map;
        private ProtobufSerializer _serializer;

        private InMemoryOperationTransport _transport;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _map = new OperationMap();
            _map.MapOperation(typeof (TestOperation));
            _map.MapOperation(typeof (TestOperationWithResponse));

            _serializer = new ProtobufSerializer();
            _serializer.RegisterTypes(new[]
            {
                typeof (TestOperation),
                typeof (TestOperationWithResponse),
                typeof (OperationResponse),
                typeof (TestResponse)
            });
        }

        [SetUp]
        public void SetupTest()
        {
            _transport = new InMemoryOperationTransport();
        }

        [Test]
        public void ShouldWriteOperation()
        {
            var writer = new NetworkOperationWriter(_map, _serializer, _transport);
            writer.WriteOperation(new TestOperation());

            var sentOperation = _transport.SentOperations.SingleOrDefault();
            sentOperation.ShouldNotBeNull();

            sentOperation.Item1.ShouldBe(OperationCode.HandleOperation);

            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationId, _map.GetMappedOperation(typeof (TestOperation)).Id);
            sentOperation.Item2.ShouldContainKey((byte) OperationParameterCode.OperationData);
        }

        [Test]
        public void ShouldWriteOperationWithResponse()
        {
            var writer = new NetworkOperationWriter(_map, _serializer, _transport);

            var promise = new OperationPromise<TestResponse>(0, new TestOperationWithResponse());
            writer.WriteOperationWithResponse(promise);

            var sentOperation = _transport.SentOperations.SingleOrDefault();
            sentOperation.ShouldNotBeNull();

            sentOperation.Item1.ShouldBe(OperationCode.HandleOperationWithResponse);

            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationId, _map.GetMappedOperation(typeof (TestOperationWithResponse)).Id);
            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationPromiseId, promise.Id);

            sentOperation.Item2.ShouldContainKey((byte) OperationParameterCode.OperationData);
        }

        [Test]
        public void ShouldWriteResponse()
        {
            var writer = new NetworkOperationWriter(_map, _serializer, _transport);
            writer.WriteResponse(new TestOperationWithResponse(), new TestResponse());

            var sentOperation = _transport.SentOperations.SingleOrDefault();
            sentOperation.ShouldNotBeNull();

            sentOperation.Item1.ShouldBe(OperationCode.HandleResponse);

            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationId, _map.GetMappedOperation(typeof (TestOperationWithResponse)).Id);
            sentOperation.Item2.ShouldContainKey((byte) OperationParameterCode.OperationResponseData);
        }
    }
}