using System;
using System.IO;

namespace Ethos.Base.Infrastructure.Serialization
{
    public class SerializationService : ISerializationService
    {
        private readonly ISerializer _serializer;

        public SerializationService(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public byte[] WriteArguments(Type[] types, object[] arguments)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                WriteArguments(bw, types, arguments);
                return ms.ToArray();
            }
        }

        public object[] ReadArguments(Type[] types, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
                return ReadArguments(br, types);
        }

        public byte[] WriteObject(Type type, object value)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                _serializer.WriteObject(bw, type, value);
                return ms.ToArray();
            }
        }

        public object ReadObject(Type type, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
                return _serializer.ReadObject(br, type);
        }

        public void WriteArguments(BinaryWriter writer, Type[] types, object[] arguments)
        {
            for (var i = 0; i < arguments.Length; i++)
                _serializer.WriteObject(writer, types[i], arguments[i]);
        }

        public object[] ReadArguments(BinaryReader reader, Type[] types)
        {
            var arguments = new object[types.Length];
            for (var i = 0; i < types.Length; i++)
                arguments[i] = _serializer.ReadObject(reader, types[i]);

            return arguments;
        }
    }
}