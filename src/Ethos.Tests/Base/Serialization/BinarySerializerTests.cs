using System;
using System.IO;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Serialization;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Serialization
{
    [TestFixture]
    public class BinarySerializerTests
    {
        class TestObject1 : ICustomSerializer
        {
            public string Data { get; set; }

            public void Save(BinaryWriter writer, ISerializer serializer)
            {
                serializer.WriteObject(writer, typeof (string), Data);
            }

            public void Load(BinaryReader reader, ISerializer serializer)
            {
                Data = (string) serializer.ReadObject(reader, typeof (string));
            }
        }

        class TestObject2
        {
            public string Data { get; set; }
        }

        enum TestEnum
        {
            One,
            Two,
            Three,
            Four
        }

        class TestOperation : IOperation<TestResponse>
        {
            public string Data { get; set; }
        }

        class TestResponse : OperationResponseBase
        {
            public string Data { get; set; }
        }

        [TestCase(typeof (byte), (byte) 1)]
        [TestCase(typeof (int), 34)]
        [TestCase(typeof (float), 0.032f)]
        [TestCase(typeof (double), 0.124)]
        [TestCase(typeof (bool), true)]
        [TestCase(typeof (string), "asdf")]
        public void ShouldWriteObjectToMemoryStream(Type type, object value)
        {
            var serializer = new BinarySerializer();

            byte[] data;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                serializer.WriteObject(bw, type, value);
                data = ms.ToArray();
            }

            data.ShouldNotBeEmpty();
        }

        [TestCase(typeof (byte), (byte) 1)]
        [TestCase(typeof (int), 34)]
        [TestCase(typeof (float), 0.032f)]
        [TestCase(typeof (double), 0.124)]
        [TestCase(typeof (bool), true)]
        [TestCase(typeof (string), "asdf")]
        public void ShouldReadObjectFromMemoryStream(Type type, object value)
        {
            var serializer = new BinarySerializer();

            byte[] data;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                serializer.WriteObject(bw, type, value);
                data = ms.ToArray();
            }

            object readValue;
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
                readValue = serializer.ReadObject(br, type);

            readValue.ShouldBe(value);
        }

        [Test]
        public void ShouldReadAndWriteGuid()
        {
            var serializer = new BinarySerializer();
            var value = Guid.NewGuid();

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (Guid), value),
                br => serializer.ReadObject(br, typeof (Guid)).ShouldBe(value));
        }

        [Test]
        public void ShouldReadAndWriteDateTime()
        {
            var serializer = new BinarySerializer();
            var value = DateTime.Now;

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (DateTime), value),
                br => serializer.ReadObject(br, typeof (DateTime)).ShouldBe(value));
        }

        [Test]
        public void ShouldThrowWhenUnrecognizedType()
        {
            var serializer = new BinarySerializer();

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                Action writeAction = () => serializer.WriteObject(bw, typeof (TestObject2), new TestObject2 {Data = "asdf"});
                writeAction.ShouldThrow<ArgumentException>();
            }

            using (var ms = new MemoryStream(new byte[] {1, 1, 2}))
            using (var br = new BinaryReader(ms))
            {
                Action readAction = () => serializer.ReadObject(br, typeof (TestObject2));
                readAction.ShouldThrow<ArgumentException>();
            }
        }

        [Test]
        public void ShouldReadAndWriteNullValue()
        {
            var serializer = new BinarySerializer();

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (string), null),
                br => serializer.ReadObject(br, typeof (string)).ShouldBeNull());
        }

        [Test]
        public void ShouldReadAndWriteArray()
        {
            var serializer = new BinarySerializer();
            var value = new[] {"Stuff", "And", "Things"};

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (string[]), value),
                br => serializer.ReadObject(br, typeof (string[])).ShouldBe(value));
        }

        [TestCase(TestEnum.One)]
        [TestCase(TestEnum.Two)]
        [TestCase(TestEnum.Three)]
        [TestCase(TestEnum.Four)]
        public void ShouldReadAndWriteEnum(object value)
        {
            var serializer = new BinarySerializer();

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (TestEnum), value),
                br => serializer.ReadObject(br, typeof (TestEnum)).ShouldBe(value));
        }

        [Test]
        public void ShouldReadAndWriteCustomSerializer()
        {
            var serializer = new BinarySerializer();
            var value = new TestObject1 {Data = "asdf"};

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (TestObject1), value),
                br => ((TestObject1) serializer.ReadObject(br, typeof (TestObject1))).Data.ShouldBe(value.Data));
        }

        [Test]
        public void ShouldReadAndWriteOperation()
        {
            var serializer = new BinarySerializer();
            var value = new TestOperation {Data = "asdf"};

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (TestOperation), value),
                br => ((TestOperation) serializer.ReadObject(br, typeof (TestOperation))).Data.ShouldBe(value.Data));
        }

        [Test]
        public void ShouldReadAndWriteOperationResponse()
        {
            var serializer = new BinarySerializer();
            var value = new TestResponse {Data = "asdf"};

            value.AddModalError("Data", "asdfasdf");
            value.AddModalError("OtherData", "asdfasdfasdfasdf");

            TestSerializer(
                bw => serializer.WriteObject(bw, typeof (TestResponse), value),
                br =>
                {
                    var readValue = ((TestResponse) serializer.ReadObject(br, typeof (TestResponse)));
                    readValue.Data.ShouldBe(value.Data);

                    readValue.ModalErrors.ShouldBe(value.ModalErrors);
                    readValue.IsValid.ShouldBeFalse();
                });
        }

        private static void TestSerializer(Action<BinaryWriter> write, Action<BinaryReader> read)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                write(bw);
                data = ms.ToArray();
            }

            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
                read(br);
        }
    }
}