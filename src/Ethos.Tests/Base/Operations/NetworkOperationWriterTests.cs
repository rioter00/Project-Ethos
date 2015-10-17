using System.Linq;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System.Mapping;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Tests.Infrastructure.Base;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class NetworkOperationWriterTests
    {
        class TestOperation : IOperation
        {
        }

        class TestOperationWithResponse : IOperation<TestResponse>
        {
        }

        class TestResponse : OperationResponseBase
        {
        }

        private OperationMap _map;
        private InMemoryOperationTransport _transport;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _map = new OperationMap();
            _map.MapOperation(typeof (TestOperation));
            _map.MapOperation(typeof (TestOperationWithResponse));
        }

        [SetUp]
        public void SetupTest()
        {
            _transport = new InMemoryOperationTransport();
        }

        [Test]
        public void ShouldWriteContextInitialization()
        {
            var writer = new NetworkOperationWriter(_map, new SerializationService(new BinarySerializer()), _transport);
            writer.WriteContextInitialization(ContextType.InstanceServer);

            var sentOperation = _transport.SentOperations.SingleOrDefault();
            sentOperation.ShouldNotBeNull();

            sentOperation.Item1.ShouldBe(OperationCode.SetupContext);
            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.ContextType, ContextType.InstanceServer);
        }

        [Test]
        public void ShouldWriteOperation()
        {
            var writer = new NetworkOperationWriter(_map, new SerializationService(new BinarySerializer()), _transport);
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
            var writer = new NetworkOperationWriter(_map, new SerializationService(new BinarySerializer()), _transport);

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
            var writer = new NetworkOperationWriter(_map, new SerializationService(new BinarySerializer()), _transport);
            writer.WriteResponse(new TestOperationWithResponse(), new TestResponse());

            var sentOperation = _transport.SentOperations.SingleOrDefault();
            sentOperation.ShouldNotBeNull();

            sentOperation.Item1.ShouldBe(OperationCode.HandleResponse);

            sentOperation.Item2.ShouldContainKeyAndValue((byte) OperationParameterCode.OperationId, _map.GetMappedOperation(typeof (TestOperationWithResponse)).Id);
            sentOperation.Item2.ShouldContainKey((byte) OperationParameterCode.OperationResponseData);
        }
    }
}