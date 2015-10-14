using System.Threading;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Operations;

namespace Ethos.Server.Master.OperationHandlers
{
    public class AuthenticationHandler : IOperationHandler<AuthenticationOperation, AuthenticationResponse>
    {
        public AuthenticationResponse Handle(AuthenticationOperation operation)
        {
            var response = new AuthenticationResponse();

            if (operation.Token != "2C1A506F-F21B-4978-B63E-1EEEC9D776A7")
                response.AddModalError(nameof(operation.Token), "Invalid token");

            Thread.Sleep(5000);
            return response;
        }
    }
}