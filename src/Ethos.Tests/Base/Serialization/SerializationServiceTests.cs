using Ethos.Base.Infrastructure.Serialization;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Serialization
{
    [TestFixture]
    public class SerializationServiceTests
    {
        [Test]
        public void ShouldReadAndWriteObject()
        {
            var service = new SerializationService(new BinarySerializer());

            const string value = "asdf";
            var data = service.WriteObject(typeof (string), value);

            var readValue = service.ReadObject(typeof (string), data);
            readValue.ShouldBe(value);
        }

        [Test]
        public void ShouldReadAndWriteArguments()
        {
            var serializer = new SerializationService(new BinarySerializer());

            var types = new[] {typeof (string), typeof (int), typeof (double)};
            var values = new object[] {"asdf", 43, 32.12};

            var data = serializer.WriteArguments(types, values);
            var readValues = serializer.ReadArguments(types, data);

            readValues.ShouldBe(values);
        }
    }
}