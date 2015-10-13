using System;
using System.Linq;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Infrastructure.Extensions
{
    public static class OperationReflectionExtensions
    {
        public static Type GetResponseType(this Type operationType)
        {
            return operationType.GetInterfaces().Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IOperation<>)).GetGenericArguments()[0];
        }

        public static bool IsOperationHandler(this Type type)
        {
            return type.IsClass && !type.IsAbstract && typeof (IOperationHandler).IsAssignableFrom(type);
        }

        public static Type GetHandlerInterfaceType(this Type concreteType)
        {
            return concreteType.GetInterfaces().Single(r => r.IsGenericType && (r.GetGenericTypeDefinition() == typeof (IOperationHandler<>) || r.GetGenericTypeDefinition() == typeof (IOperationHandler<,>)));
        }
    }
}