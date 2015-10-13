using System.Collections.Generic;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations.Mapping
{
    public class OperationMapBinaryFormatter
    {
        private readonly ISerializer _serializer;

        public OperationMapBinaryFormatter(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public void LoadOperationMap(OperationMap map, byte[] data)
        {
            map.SyncMappedOperations((List<MappedOperation>) _serializer.DeserializeObject(typeof (List<MappedOperation>), data));
        }

        public byte[] SaveOperationMap(OperationMap map)
        {
            return _serializer.SerializeObject(new List<MappedOperation>(map.MappedOperations));
        }
    }
}