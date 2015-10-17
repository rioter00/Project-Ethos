using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ethos.Base.Infrastructure.Components
{
    public class MappedComponent
    {
        private readonly MappedMethod[] _mappedMethods;
        private byte _nextAvailableId;

        public byte Id { get; }
        public Type ComponentType { get; }

        public IEnumerable<MappedMethod> MappedMethods => _mappedMethods.Where(t => t != null);

        public MappedComponent(byte id, Type componentType)
        {
            Id = id;
            ComponentType = componentType;

            _mappedMethods = new MappedMethod[byte.MaxValue + 1];
        }

        public void AutomapMethods()
        {
            foreach (var methodInfo in ComponentType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                MapMethod(_nextAvailableId++, methodInfo);
        }

        public MappedMethod MapMethod(byte methodId, MethodInfo methodInfo)
        {
            if (_mappedMethods[methodId] != null)
                throw new ArgumentException($"Failed to map method '{methodInfo.Name}', a method with id '{methodId}' has already been mapped", nameof (methodId));

            var mappedMethod = new MappedMethod(this, methodId, methodInfo);
            _mappedMethods[methodId] = mappedMethod;

            return mappedMethod;
        }

        public MappedMethod GetMethod(byte methodId)
        {
            return _mappedMethods[methodId];
        }

        public MappedMethod GetMethod(MethodInfo methodInfo)
        {
            return _mappedMethods.Single(t => t != null && t.MethodInfo == methodInfo);
        }
    }
}