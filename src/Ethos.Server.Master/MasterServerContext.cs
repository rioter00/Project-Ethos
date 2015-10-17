using Autofac;
using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Server.Infrastructure;

namespace Ethos.Server.Master
{
    public class MasterServerContext : ServerContextBase
    {
        public MasterServerContext(ISerializer serializer) : base(serializer)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof (MasterServerContext).Assembly)
                .Where(t => t.IsOperationHandler())
                .As(t => t.GetHandlerInterfaceType());
        }
    }
}