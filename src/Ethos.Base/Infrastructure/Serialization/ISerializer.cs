using System;
using System.IO;

namespace Ethos.Base.Infrastructure.Serialization
{
    public interface ISerializer
    {
        void WriteObject(BinaryWriter writer, Type type, object value);
        object ReadObject(BinaryReader reader, Type type);
    }
}