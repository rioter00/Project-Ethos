using System;
using System.IO;

namespace Ethos.Base.Infrastructure.Serialization
{
    public interface ISerializationService
    {
        byte[] WriteArguments(Type[] types, object[] arguments);
        object[] ReadArguments(Type[] types, byte[] data);

        void WriteArguments(BinaryWriter writer, Type[] types, object[] arguments);
        object[] ReadArguments(BinaryReader reader, Type[] types);

        byte[] WriteObject(Type type, object value);
        object ReadObject(Type type, byte[] data);
    }
}