using System;
using System.Collections.Generic;
using System.Linq;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Serialization;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base
{
    [TestFixture]
    public class ProtobufSerializerTests
    {
        #region Mocking

        abstract class TestObjectBase
        {
            public string BaseData { get; set; }
        }

        class TestObjectSubclass : TestObjectBase
        {
            public string SubclassData { get; set; }
        }

        class TestObjectWithSerializableField
        {
            [SerializableField]
            private string _fieldData;

            public void SetFieldData(string fieldData)
            {
                _fieldData = fieldData;
            }

            public string GetFieldData()
            {
                return _fieldData;
            }
        }

        class TestObjectWithCollections
        {
            public IDictionary<string, string> Dictionary { get; set;}
            public IList<string> List { get; set; }

            public IDictionary<string, IList<string>> DictionaryOfLists { get; set; }
                
            [SerializableField]
            private readonly string[] _array;

            public TestObjectWithCollections()
            {
                Dictionary = new Dictionary<string, string>();
                List = new List<string>();
                DictionaryOfLists = new Dictionary<string, IList<string>>();

                _array = new string[10];
            }

            public void AddDataToArray(int index, string data)
            {
                _array[index] = data;
            }

            public IEnumerable<string> GetArray()
            {
                return _array;
            }
        }

        #endregion

        private ProtobufSerializer _serializer;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _serializer = new ProtobufSerializer();
            _serializer.RegisterTypes(new[]
            {
                typeof (TestObjectBase),
                typeof (TestObjectSubclass),
                typeof (TestObjectWithSerializableField),
                typeof (TestObjectWithCollections)
            });
        }

        [Test]
        public void ShouldSerializeObject()
        {
            var data = _serializer.SerializeObject("asdf");
            data.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldDeserializeObject()
        {
            var data = _serializer.SerializeObject("data");

            var value = (string) _serializer.DeserializeObject(typeof (string), data);
            value.ShouldBe("data");
        }

        [Test]
        public void ShouldSerializeRegisteredObject()
        {
            var data = _serializer.SerializeObject(new TestObjectSubclass {BaseData = "base_data", SubclassData = "subclass_data"});
            data.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldDeserializeRegisteredObject()
        {
            var data = _serializer.SerializeObject(new TestObjectSubclass {BaseData = "base_data", SubclassData = "subclass_data"});

            var value = (TestObjectSubclass) _serializer.DeserializeObject(typeof (TestObjectSubclass), data);

            value.BaseData.ShouldBe("base_data");
            value.SubclassData.ShouldBe("subclass_data");
        }

        [Test]
        public void ShouldSerializeFieldMarkedWithAttribute()
        {
            var @object = new TestObjectWithSerializableField();
            @object.SetFieldData("field_data");

            var data = _serializer.SerializeObject(@object);
            data.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldDeserializeFieldMarkedWithAttribute()
        {
            var @object = new TestObjectWithSerializableField();
            @object.SetFieldData("field_data");

            var data = _serializer.SerializeObject(@object);

            var value = (TestObjectWithSerializableField) _serializer.DeserializeObject(typeof (TestObjectWithSerializableField), data);
            value.GetFieldData().ShouldBe("field_data");
        }

        [Test]
        public void ShouldSerializeCollections()
        {
            var @object = new TestObjectWithCollections();

            @object.Dictionary.Add("key_data", "value_data");
            @object.List.Add("list_data");
            @object.DictionaryOfLists.Add("key_data", new List<string> {"list_data"});
            @object.AddDataToArray(0, "array_data");

            var data = _serializer.SerializeObject(@object);
            data.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldDeserializeCollections()
        {
            var @object = new TestObjectWithCollections();

            @object.Dictionary.Add("key_data", "value_data");
            @object.List.Add("list_data");
            @object.DictionaryOfLists.Add("key_data", new List<string> {"list_data"});
            @object.AddDataToArray(0, "array_data");

            var data = _serializer.SerializeObject(@object);

            var value = (TestObjectWithCollections) _serializer.DeserializeObject(typeof (TestObjectWithCollections), data);

            value.Dictionary.ShouldContainKeyAndValue("key_data", "value_data");
            value.List.ShouldContain("list_data");
            value.DictionaryOfLists.ShouldContainKey("key_data");
            value.DictionaryOfLists.Values.Any(t => t.Contains("list_data")).ShouldBeTrue();
            value.GetArray().ShouldContain("array_data");
        }
    }
}