using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Operations;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Ethos.Base.Infrastructure.Serialization
{
    public class ProtobufSerializer : ISerializer
    {
        public byte[] SerializeObject(object value)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        public object DeserializeObject(Type type, byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return Serializer.NonGeneric.Deserialize(type, ms);
        }

        public void RegisterOperations(Assembly assembly)
        {
            RegisterTypes(assembly.GetTypes().Where(t => typeof (IOperation).IsAssignableFrom(t) || typeof (IOperationResponse).IsAssignableFrom(t)));
        }

        public void RegisterTypes(IEnumerable<Type> types)
        {
            foreach (var type in types.Where(t => t.IsClass))
            {
                var metaType = RuntimeTypeModel.Default.Add(type, false);
                var nextAvailableId = 1;

                foreach (var subclassType in types.Where(t => t.IsSubclassOf(type)))
                    metaType.AddSubType(nextAvailableId++, subclassType);

                foreach (var propertyType in type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(t => t.GetSetMethod() != null))
                    metaType.AddField(nextAvailableId++, propertyType.Name);

                foreach (var serializableField in type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic).Where(t => Attribute.IsDefined(t, typeof (SerializableFieldAttribute))))
                    metaType.AddField(nextAvailableId++, serializableField.Name);
            }
        }
    }
}