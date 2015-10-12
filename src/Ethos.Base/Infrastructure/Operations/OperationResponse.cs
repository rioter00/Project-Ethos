using System.Collections.Generic;
using System.Linq;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations
{
    public abstract class OperationResponse : IOperationResponse
    {
        [SerializableField]
        private readonly IDictionary<string, IList<string>> _modalErrors;

        public string ModalErrors => string.Join("; ", _modalErrors.Select(r => $"{r.Key}: {string.Join(", ", r.Value.ToArray())}").ToArray());
        public bool IsValid => !_modalErrors.Any();

        protected OperationResponse()
        {
            _modalErrors = new Dictionary<string, IList<string>>();
        }

        public void AddModalError(string property, string error)
        {
            IList<string> errors;
            if (!_modalErrors.TryGetValue(property, out errors))
            {
                _modalErrors.Add(property, new List<string> {error});
                return;
            }

            errors.Add(error);
        }
    }
}