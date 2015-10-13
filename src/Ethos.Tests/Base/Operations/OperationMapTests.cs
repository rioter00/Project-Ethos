using System;
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
        public void ShouldMapOperations()
        {
            var map = new OperationMap();

            var mappedOperation = map.MapOperation(typeof (TestOperation));
            mappedOperation.OperationType.ShouldBe(typeof (TestOperation));
        }

        [Test]
        public void ShouldRetrieveOperationById()
        {
            var map = new OperationMap();

            var mappedOperation = map.MapOperation(typeof (TestOperation));
            map.GetMappedOperation(mappedOperation.Id).ShouldBe(mappedOperation);
        }

        [Test]
        public void ShouldRetrieveOperationByType()
        {
            var map = new OperationMap();

            var mappedOperation = map.MapOperation(typeof (TestOperation));
            map.GetMappedOperation(mappedOperation.OperationType).ShouldBe(mappedOperation);
        }

        [Test]
        public void ShouldSucceedInTryingToFindOperationByName()
        {
            var map = new OperationMap();
            var mappedOperation = map.MapOperation(typeof (TestOperation));

            Type foundType;
            var success = map.TryGetMappedOperation("TestOperation", out foundType);

            success.ShouldBeTrue();
            foundType.ShouldBe(mappedOperation.OperationType);
        }

        [Test]
        public void ShouldFailInTryingToFindOperationByName()
        {
            var map = new OperationMap();

            Type foundType;
            var success = map.TryGetMappedOperation("InvalidOperationType", out foundType);

            success.ShouldBeFalse();
            foundType.ShouldBeNull();
        }
    }
}