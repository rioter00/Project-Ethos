using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.Mapping;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class OperationMapTests
    {
        class TestOperation : IOperation
        {
        }

        [Test]
        public void ShouldMapEvents()
        {
            var sut = new OperationMap();

            var mappedOperation = sut.MapOperation(typeof (TestOperation));

            mappedOperation.OperationType.ShouldBe(typeof (TestOperation));
        }

        [Test]
        public void ShouldRetrieveEventById()
        {
            var sut = new OperationMap();

            var mappedOperation = sut.MapOperation(typeof (TestOperation));

            sut.GetMappedOperation(mappedOperation.Id).ShouldBe(mappedOperation);
        }

        [Test]
        public void ShouldRetrieveEventByType()
        {
            var sut = new OperationMap();

            var mappedOperation = sut.MapOperation(typeof (TestOperation));

            sut.GetMappedOperation(mappedOperation.OperationType).ShouldBe(mappedOperation);
        }
    }
}