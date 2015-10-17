using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ethos.Base.Infrastructure.Components
{
    public class ComponentMap
    {
        private readonly MappedComponent[] _mappedComponents;
        private byte _nextAvailableId;

        public IEnumerable<MappedComponent> MappedComponents => _mappedComponents.Where(t => t != null);

        public ComponentMap()
        {
            _mappedComponents = new MappedComponent[byte.MaxValue + 1];
        }

        public void AutomapAssembly(Assembly assembly, Func<Type, bool> predicate)
        {
            foreach (var type in assembly.GetTypes().Where(predicate))
                AutomapComponent(type);
        }

        public MappedComponent AutomapComponent(Type componentType)
        {
            var mappedComponent = MapComponent(_nextAvailableId++, componentType);
            mappedComponent.AutomapMethods();

            return mappedComponent;
        }

        public MappedComponent MapComponent(byte componentId, Type componentType)
        {
            if (_mappedComponents[componentId] != null)
                throw new ArgumentException($"Failed to map component '{componentType}', a component with id '{componentId}' has already been mapped", nameof (componentId));

            var component = new MappedComponent(componentId, componentType);
            _mappedComponents[componentId] = component;

            return component;
        }

        public MappedComponent GetComponent(byte componentId)
        {
            return _mappedComponents[componentId];
        }

        public MappedComponent GetComponent(Type componentType)
        {
            return _mappedComponents.Single(t => t != null && t.ComponentType == componentType);
        }
    }
}