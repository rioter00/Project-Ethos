using System;
using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations.System.Networking;

namespace Ethos.Tests.Infrastructure.Base
{
    public class InMemoryOperationTransport : IOperationTransport
    {
        private readonly IList<Tuple<OperationCode, IDictionary<byte, object>>> _sentOperations;

        public IEnumerable<Tuple<OperationCode, IDictionary<byte, object>>> SentOperations => _sentOperations;

        public InMemoryOperationTransport()
        {
            _sentOperations = new List<Tuple<OperationCode, IDictionary<byte, object>>>();
        }

        public void SendOperation(OperationCode code, Dictionary<byte, object> parameters)
        {
            _sentOperations.Add(new Tuple<OperationCode, IDictionary<byte, object>>(code, parameters));
        }
    }
}