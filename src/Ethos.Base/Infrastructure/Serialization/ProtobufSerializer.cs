using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ethos.Base.Infrastructure.Operations;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Ethos.Base.Infrastructure.Serialization
{
    public class ProtobufSerializer : ISerializer
    {
        private static IList<Type> _registeredTypes;

        public static IList<Type> RegisteredTypes => _registeredTypes ?? (_registeredTypes = new List<Type>());

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
            foreach (var type in GetValidTypes(types))
            {
                var metaType = RuntimeTypeModel.Default.Add(type, false);
                var nextAvailableId = 1;

                foreach (var subclassType in GetSubclasses(types, type))
                    metaType.AddSubType(nextAvailableId++, subclassType);

                foreach (var propertyType in GetProperties(type))
                    metaType.AddField(nextAvailableId++, propertyType.Name);

                foreach (var serializableField in GetSerializableFields(type))
                    metaType.AddField(nextAvailableId++, serializableField.Name);

                RegisteredTypes.Add(type);
            }
        }

        private IEnumerable<Type> GetValidTypes(IEnumerable<Type> types)
        {
            return types.Where(t => t.IsClass && !RegisteredTypes.Contains(t));
        }

        private IEnumerable<Type> GetSubclasses(IEnumerable<Type> types, Type type)
        {
            return types.Where(t => t.IsSubclassOf(type));
        }

        private IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(t => t.GetSetMethod() != null);
        }

        private IEnumerable<FieldInfo> GetSerializableFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic).Where(t => Attribute.IsDefined(t, typeof (SerializableFieldAttribute)));
        }
    }
}