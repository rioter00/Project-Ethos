using System;

namespace Ethos.Base.Infrastructure.Serialization
{
    public interface ISerializer
    {
        byte[] SerializeObject(object value);
        object DeserializeObject(Type type, byte[] data);
    }
}