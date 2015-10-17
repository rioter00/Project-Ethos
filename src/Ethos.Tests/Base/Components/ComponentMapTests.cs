using System;
using System.Linq;
using Ethos.Base.Infrastructure.Components;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Components
{
    [TestFixture]
    public class ComponentMapTests
    {
        class TestComponent
        {
            public void PublicMethod()
            {
            }

            private void PrivateMethod()
            {
            }
        }

        [Test]
        public void ShouldMapComponent()
        {
            var componentMap = new ComponentMap();
            var mappedComponent = componentMap.MapComponent(0, typeof (TestComponent));

            componentMap.MappedComponents.ShouldContain(mappedComponent);

            mappedComponent.Id.ShouldBe((byte) 0);
            mappedComponent.ComponentType.ShouldBe(typeof (TestComponent));
        }

        [Test]
        public void ShouldThrowWhenMappingDuplicateIds()
        {
            var componentMap = new ComponentMap();
            componentMap.MapComponent(0, typeof (TestComponent));

            Action action = () => componentMap.MapComponent(0, typeof (TestComponent));
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void ShouldAutomapComponent()
        {
            var componentMap = new ComponentMap();
            var mappedComponent = componentMap.AutomapComponent(typeof (TestComponent));

            componentMap.MappedComponents.ShouldContain(mappedComponent);
            mappedComponent.MappedMethods.Any(t => t.MethodInfo == typeof (TestComponent).GetMethod("PublicMethod")).ShouldBeTrue();
        }

        [Test]
        public void ShouldAutomapAssembly()
        {
            var componentMap = new ComponentMap();
            componentMap.AutomapAssembly(typeof (ComponentMapTests).Assembly, t => t == typeof (TestComponent));

            componentMap.MappedComponents.Any(t => t.ComponentType == typeof (TestComponent)).ShouldBeTrue();
        }

        [Test]
        public void ShouldGetMethodById()
        {
            var componentMap = new ComponentMap();
            var mappedComponent = componentMap.MapComponent(0, typeof (TestComponent));

            var foundComponent = componentMap.GetComponent(mappedComponent.Id);
            foundComponent.ShouldBe(mappedComponent);
        }

        [Test]
        public void ShouldGetMethodByType()
        {
            var componentMap = new ComponentMap();
            var mappedComponent = componentMap.MapComponent(0, typeof (TestComponent));

            var foundComponent = componentMap.GetComponent(typeof (TestComponent));
            foundComponent.ShouldBe(mappedComponent);
        }
    }
}