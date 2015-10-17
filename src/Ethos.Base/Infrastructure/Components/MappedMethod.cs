using System;
using System.Linq;
using System.Reflection;

namespace Ethos.Base.Infrastructure.Components
{
    public class MappedMethod
    {
        public MappedComponent MappedComponent { get; }

        public byte Id { get; }
        public MethodInfo MethodInfo { get; }

        public Type[] ParametersTypes { get; }

        public MappedMethod(MappedComponent mappedComponent, byte id, MethodInfo methodInfo)
        {
            MappedComponent = mappedComponent;

            Id = id;
            MethodInfo = methodInfo;

            ParametersTypes = MethodInfo.GetParameters().Select(t => t.ParameterType).ToArray();
        }
    }
}