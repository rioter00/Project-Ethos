using System;
using System.Linq;
using Ethos.Base.Infrastructure.Components;
using NUnit.Framework;
using Shouldly;

namespace Ethos.Tests.Base.Components
{
    [TestFixture]
    public class MappedComponentTests
    {
        class TestComponent
        {
            public static void StaticMethod()
            {
            }

            public void PublicMethod()
            {
            }

            private void PrivateMethod()
            {
            }
        }

        [Test]
        public void ShouldMapMethod()
        {
            var mappedComponent = new MappedComponent(0, typeof (TestComponent));
            var mappedMethod = mappedComponent.MapMethod(0, typeof (TestComponent).GetMethod("PublicMethod"));

            mappedComponent.MappedMethods.ShouldContain(mappedMethod);

            mappedMethod.Id.ShouldBe((byte) 0);
            mappedMethod.MethodInfo.ShouldBe(typeof (TestComponent).GetMethod("PublicMethod"));
            mappedMethod.MappedComponent.ShouldBe(mappedComponent);
        }

        [Test]
        public void ShouldThrowWhenMappingDuplicateIds()
        {
            var mappedComponent = new MappedComponent(0, typeof (TestComponent));
            var mappedMethod = mappedComponent.MapMethod(0, typeof (TestComponent).GetMethod("PublicMethod"));

            Action action = () => mappedComponent.MapMethod(mappedMethod.Id, typeof (TestComponent).GetMethod("PublicMethod"));
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void ShouldAutoMapPublicAndInstanceMethods()
        {
            var mappedComponent = new MappedComponent(0, typeof (TestComponent));
            mappedComponent.AutomapMethods();

            mappedComponent.MappedMethods.Any(t => t.MethodInfo == typeof (TestComponent).GetMethod("StaticMethod")).ShouldBeFalse();
            mappedComponent.MappedMethods.Any(t => t.MethodInfo == typeof (TestComponent).GetMethod("PublicMethod")).ShouldBeTrue();
            mappedComponent.MappedMethods.Any(t => t.MethodInfo == typeof (TestComponent).GetMethod("PrivateMethod")).ShouldBeFalse();
        }

        [Test]
        public void ShouldGetMappedMethodById()
        {
            var mappedComponent = new MappedComponent(0, typeof (TestComponent));
            var mappedMethod = mappedComponent.MapMethod(0, typeof (TestComponent).GetMethod("PublicMethod"));

            var foundMethod = mappedComponent.GetMethod(mappedMethod.Id);
            foundMethod.ShouldBe(mappedMethod);
        }

        [Test]
        public void ShouldGetMappedMethodByMethodInfo()
        {
            var mappedComponent = new MappedComponent(0, typeof (TestComponent));
            var mappedMethod = mappedComponent.MapMethod(0, typeof (TestComponent).GetMethod("PublicMethod"));

            var foundMethod = mappedComponent.GetMethod(mappedMethod.MethodInfo);
            foundMethod.ShouldBe(mappedMethod);
        }
    }
}