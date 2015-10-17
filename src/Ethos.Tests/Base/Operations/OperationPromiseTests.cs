using System;
using Ethos.Base.Infrastructure.Operations;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Operations
{
    [TestFixture]
    public class OperationPromiseTests
    {
        #region Mocking

        class TestOperation : IOperation<TestResponse>
        {
        }

        class TestResponse : OperationResponseBase
        {
            public int MethodInvokeCount { get; private set; }

            public void Method()
            {
                MethodInvokeCount++;
            }
        }

        #endregion

        [Test]
        public void ShouldRegisterCallback()
        {
            var promise = new OperationPromise<TestResponse>(0, new TestOperation());

            Action<TestResponse> callback = response => response.Method();
            promise.Then(callback);

            promise.Callbacks.ShouldContain(callback);
            promise.IsCompleted.ShouldBeFalse();
        }

        [Test]
        public void ShouldInvokeCallbacksOnComplete()
        {
            var promise = new OperationPromise<TestResponse>(0, new TestOperation());
            promise.Then(t => t.Method());

            var response = new TestResponse();
            promise.Complete(response);

            promise.Response.ShouldBe(response);
            promise.IsCompleted.ShouldBeTrue();

            response.MethodInvokeCount.ShouldBe(1);
        }

        [Test]
        public void ShouldThrowWhenRegisteringCallbackAfterCompletion()
        {
            var promise = new OperationPromise<TestResponse>(0, new TestOperation());
            promise.Complete(new TestResponse());

            Action action = () => promise.Then(t => t.Method());
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void ShouldThrowWhenCompletingPromiseAfterCompletion()
        {
            var promise = new OperationPromise<TestResponse>(0, new TestOperation());
            promise.Complete(new TestResponse());

            Action action = () => promise.Complete(new TestResponse());
            action.ShouldThrow<InvalidOperationException>();
        }
    }
}