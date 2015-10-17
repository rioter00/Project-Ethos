using System;

namespace Ethos.Base.Infrastructure.Systems
{
    public interface ISystem : IDisposable
    {
        Type ServerComponentType { get; }
        Type ClientComponentType { get; }

        void Awake();
    }
}