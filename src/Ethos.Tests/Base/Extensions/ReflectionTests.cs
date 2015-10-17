using System.IO;
using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Serialization;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Extensions
{
    [TestFixture]
    public class ReflectionTests
    {
        private class TestObject : ICustomSerializer
        {
            public string PublicStringProperty { get; set; }
            private string PrivateStringProperty { get; set; }

            public int PublicReadonlyInt { get; }
            public int PublicPrivateSetInt { get; private set; }

            public void Save(BinaryWriter writer, ISerializer serializer)
            {
                throw new System.NotImplementedException();
            }

            public void Load(BinaryReader reader, ISerializer serializer)
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestOperation : IOperation<TestResponse>
        {
        }

        private class TestResponse : OperationResponseBase
        {
        }

        private class TestHandler : IOperationHandler<TestOperation>
        {
            public void Handle(TestOperation operation)
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestHandlerWithResponse : IOperationHandler<TestOperation, TestResponse>
        {
            public TestResponse Handle(TestOperation operation)
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void ShouldGetPublicProperties()
        {
            var properties = typeof (TestObject).GetPublicProperties();

            properties.ShouldContain(typeof (TestObject).GetProperty("PublicStringProperty"));

            properties.ShouldNotContain(typeof (TestObject).GetProperty("PrivateStringProperty"));
            properties.ShouldNotContain(typeof(TestObject).GetProperty("PublicReadonlyInt"));
            properties.ShouldNotContain(typeof(TestObject).GetProperty("PublicPrivateSetInt"));
        }

        [Test]
        public void ShouldGetResponseType()
        {
            typeof (TestOperation).GetResponseType().ShouldBe(typeof (TestResponse));
        }

        [Test]
        public void ShouldGetHandlerInterfaceType()
        {
            typeof (TestHandler).GetHandlerInterfaceType().ShouldBe(typeof (IOperationHandler<TestOperation>));
            typeof (TestHandlerWithResponse).GetHandlerInterfaceType().ShouldBe(typeof (IOperationHandler<TestOperation, TestResponse>));
        }

        [Test]
        public void ShouldDetermineIfTypeIsOperation()
        {
            typeof (TestOperation).IsOperation().ShouldBeTrue();
            typeof (TestObject).IsOperation().ShouldBeFalse();
        }

        [Test]
        public void ShouldDetermineIfTypeIsOperationResponse()
        {
            typeof (TestResponse).IsOperationResponse().ShouldBeTrue();
            typeof (TestOperation).IsOperationResponse().ShouldBeFalse();
        }

        [Test]
        public void ShouldDetermineIfTypeIsOperationHandler()
        {
            typeof (TestOperation).IsOperationHandler().ShouldBeFalse();

            typeof (TestHandler).IsOperationHandler().ShouldBeTrue();
            typeof (TestHandlerWithResponse).IsOperationHandler().ShouldBeTrue();
        }

        [Test]
        public void ShouldDetermineIfTypeIsCustomSerializer()
        {
            typeof (TestObject).IsCustomSerializer().ShouldBeTrue();
            typeof (TestResponse).IsCustomSerializer().ShouldBeFalse();
        }
    }
}