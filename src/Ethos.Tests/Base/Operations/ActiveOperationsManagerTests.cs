using System;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class ActiveOperationsManagerTests
    {
        #region Mocking

        class TestOperation : IOperation<TestResponse>
        {
        }

        class TestResponse : OperationResponseBase
        {
        }

        #endregion

        [Test]
        public void ShouldRegisterOperation()
        {
            var manager = new ActiveOperationsManager();

            var operation = new TestOperation();
            var promise = manager.RegisterOperation(operation);

            manager.ActiveOperations.ShouldContain(operation);
            promise.ShouldBeOfType<OperationPromise<TestResponse>>();
        }

        [Test]
        public void ShouldRetrieveAndRemoveOperation()
        {
            var manager = new ActiveOperationsManager();
            var promise = manager.RegisterOperation(new TestOperation());

            var retrievedPromise = manager.RetrieveAndRemoveOperation(promise.Id);

            manager.ActiveOperations.ShouldBeEmpty();
            retrievedPromise.ShouldBe(promise);
        }

        [Test]
        public void ShouldThrowWhenRetrievingNonexistentOperation()
        {
            var manager = new ActiveOperationsManager();

            Action action = () => manager.RetrieveAndRemoveOperation(2);
            action.ShouldThrow<InvalidOperationException>();
        }
    }
}