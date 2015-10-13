using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Operations
{
    public class AuthenticationOperation : IOperation<AuthenticationResponse>
    {
        public string Token { get; set; }
    }

    public class AuthenticationResponse : OperationResponse
    {
    }
}