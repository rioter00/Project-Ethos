using System;
using Autofac;

namespace Ethos.Base.Infrastructure.Operations
{
    public class OperationHandlerFactory
    {
        private readonly IComponentContext _context;

        public OperationHandlerFactory(IComponentContext context)
        {
            _context = context;
        }

        public IOperationHandler CreateHandler(Type type)
        {
            return (IOperationHandler) _context.Resolve(type);
        }
    }
}