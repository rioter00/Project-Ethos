using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Ethos.Base.Infrastructure.Serialization
{
    public class BsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public BsonSerializer()
        {
            _jsonSerializer = new JsonSerializer();
        }

        public byte[] SerializeObject(object value)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BsonWriter(ms))
            {
                _jsonSerializer.Serialize(bw, value);
                return ms.ToArray();
            }
        }

        public object DeserializeObject(Type type, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BsonReader(ms))
            {
                if (type.GetInterfaces().Any(t => t == typeof (IEnumerable)))
                    br.ReadRootValueAsArray = true;

                return _jsonSerializer.Deserialize(br, type);
            }
        }
    }
}