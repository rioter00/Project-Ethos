using System.IO;

namespace Ethos.Base.Infrastructure.Serialization
{
    public interface ICustomSerializer
    {
        void Save(BinaryWriter writer, ISerializer serializer);
        void Load(BinaryReader reader, ISerializer serializer);
    }
}