using System.Linq;
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
    public class NetworkOperationReaderTests
    {
        class TestOperation : IOperation
        {
            public string Data { get; set; }
        }

        class TestOperationWithResponse : IOperation<TestResponse>
        {
            public string Data { get; set; }
        }

        class TestResponse : OperationResponseBase
        {
            public string Data { get; set; }
        }

        private OperationMap _map;
        private SerializationService _serializer;

        private InMemoryOperationTransport _transport;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _map = new OperationMap();

            _map.MapOperation(typeof (TestOperation));
            _map.MapOperation(typeof (TestOperationWithResponse));

            _serializer = new SerializationService(new BinarySerializer());
        }

        [SetUp]
        public void SetupTest()
        {
            _transport = new InMemoryOperationTransport();
        }

        [Test]
        public void ShouldReadOperation()
        {
            var writer = new NetworkOperationWriter(_map, _serializer, _transport);

            var operation = new TestOperation {Data = "test_data"};
            writer.WriteOperation(operation);

            var reader = new NetworkOperationReader(_map, _serializer);
            var readOperation = (TestOperation) reader.ReadOperation(_transport.SentOperations.Single().Item2);

            readOperation.Data.ShouldBe(operation.Data);
        }

        [Test]
        public void ShouldReadPromiseId()
        {
            var writer = new NetworkOperationWriter(_map, _serializer, _transport);

            var promise = new OperationPromise<TestResponse>(0, new TestOperationWithResponse());
            writer.WriteOperationWithResponse(promise);

            var reader = new NetworkOperationReader(_map, _serializer);
            var promiseId = reader.ReadPromiseId(_transport.SentOperations.Single().Item2);

            promiseId.ShouldBe(promise.Id);
        }

        [Test]
        public void ShouldReadResponse()
        {
            var writer = new NetworkOperationWriter(_map, _serializer, _transport);

            var response = new TestResponse {Data = "test_data"};
            writer.WriteResponse(new TestOperationWithResponse(), response);

            var reader = new NetworkOperationReader(_map, _serializer);
            var readResponse = (TestResponse) reader.ReadResponse(_transport.SentOperations.Single().Item2);

            readResponse.Data.ShouldBe(response.Data);
        }
    }
}