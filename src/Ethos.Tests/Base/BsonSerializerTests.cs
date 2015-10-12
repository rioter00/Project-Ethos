using System.Collections.Generic;
using Ethos.Base.Infrastructure.Serialization;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base
{
    [TestFixture]
    public class BsonSerializerTests
    {
        #region Mocking

        class TestObjectA
        {
            public string TestData { get; set; }
        }

        class TestObjectB
        {
            public string TestData { get; }

            public TestObjectB(string testData)
            {
                TestData = testData;
            }
        }

        class TestObjectC
        {
            public IDictionary<string, TestObjectB> TestDictionary { get; }
            public IList<TestObjectB> TestList { get; }

            public TestObjectC(IDictionary<string, TestObjectB> testDictionary, IList<TestObjectB> testList)
            {
                TestDictionary = testDictionary;
                TestList = testList;
            }
        }

        #endregion

        [Test]
        public void CanSerializeObject()
        {
            var serializer = new BsonSerializer();
            var bytes = serializer.SerializeObject(new TestObjectA());

            bytes.ShouldNotBeEmpty();
        }

        [Test]
        public void CanDeserializeObject()
        {
            var serializer = new BsonSerializer();
            var bytes = serializer.SerializeObject(new TestObjectA {TestData = "Stuff and Things!"});

            var deserializedObject = (TestObjectA) serializer.DeserializeObject(typeof (TestObjectA), bytes);
            deserializedObject.TestData.ShouldBe("Stuff and Things!");
        }

        [Test]
        public void CanSerializeObjectWithoutDefaultConstructor()
        {
            var serializer = new BsonSerializer();
            var bytes = serializer.SerializeObject(new TestObjectB("Stuff and Things"));

            bytes.ShouldNotBeEmpty();
        }

        [Test]
        public void CanDeserializeObjectWithoutDefaultConstructor()
        {
            var serializer = new BsonSerializer();
            var bytes = serializer.SerializeObject(new TestObjectB("Stuff and Things!"));

            var deserializedObject = (TestObjectB) serializer.DeserializeObject(typeof (TestObjectB), bytes);
            deserializedObject.TestData.ShouldBe("Stuff and Things!");
        }

        [Test]
        public void CanSerializeStandardCollections()
        {
            var serializer = new BsonSerializer();
            var listBytes = serializer.SerializeObject(new List<TestObjectB>
            {
                new TestObjectB("Stuff and Things!"),
                new TestObjectB("More Stuff and Things!")
            });

            var dictionaryBytes = serializer.SerializeObject(new Dictionary<string, TestObjectB>
            {
                ["object1"] = new TestObjectB("Stuff and Things!"),
                ["object2"] = new TestObjectB("More Stuff and Things!")
            });

            listBytes.ShouldNotBeEmpty();
            dictionaryBytes.ShouldNotBeEmpty();
        }

        [Test]
        public void CanDeserializeStandardCollections()
        {
            var serializer = new BsonSerializer();

            var listBytes = serializer.SerializeObject(new List<TestObjectB>
            {
                new TestObjectB("Stuff and Things!"),
                new TestObjectB("More Stuff and Things!")
            });

            // TODO: Fix deserialization of dictionaries
            var deserializedList = (List<TestObjectB>) serializer.DeserializeObject(typeof (List<TestObjectB>), listBytes);
            deserializedList.Count.ShouldBe(2);

            deserializedList.ShouldContain(t => t.TestData == "Stuff and Things!");
            deserializedList.ShouldContain(t => t.TestData == "More Stuff and Things!");
        }

        [Test]
        public void CanSerializeObjectsWithStandardCollections()
        {
            var serializer = new BsonSerializer();
            var bytes = serializer.SerializeObject(
                new TestObjectC(
                    new Dictionary<string, TestObjectB>
                    {
                        ["object1"] = new TestObjectB("Stuff and Things!"),
                        ["object2"] = new TestObjectB("More Stuff and Things!")
                    },
                    new List<TestObjectB>
                    {
                        new TestObjectB("Stuff and Things!"),
                        new TestObjectB("More Stuff and Things!")
                    }));

            bytes.ShouldNotBeEmpty();
        }

        [Test]
        public void CanDeserializeObjectsWithStandardCollections()
        {
            var serializer = new BsonSerializer();
            var bytes = serializer.SerializeObject(
                new TestObjectC(
                    new Dictionary<string, TestObjectB>
                    {
                        ["object1"] = new TestObjectB("Stuff and Things!"),
                        ["object2"] = new TestObjectB("More Stuff and Things!")
                    },
                    new List<TestObjectB>
                    {
                        new TestObjectB("Stuff and Things!"),
                        new TestObjectB("More Stuff and Things!")
                    }));

            var deserializedObject = (TestObjectC) serializer.DeserializeObject(typeof (TestObjectC), bytes);

            deserializedObject.TestDictionary.Keys.ShouldContain("object1", "object2");
            deserializedObject.TestDictionary.Count.ShouldBe(2);

            deserializedObject.TestDictionary["object1"].TestData.ShouldBe("Stuff and Things!");
            deserializedObject.TestDictionary["object2"].TestData.ShouldBe("More Stuff and Things!");

            deserializedObject.TestList.Count.ShouldBe(2);

            deserializedObject.TestList.ShouldContain(t => t.TestData == "Stuff and Things!");
            deserializedObject.TestList.ShouldContain(t => t.TestData == "More Stuff and Things!");
        }
    }
}