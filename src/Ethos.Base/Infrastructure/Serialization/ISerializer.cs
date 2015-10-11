using System;

namespace Ethos.Base.Infrastructure.Serialization
{
    public interface ISerializer
    {
        byte[] SerializeObject(object @object);
        object DeserializeObject(Type type, byte[] data);
    }
}