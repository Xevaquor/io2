using Autofac;
using IO2.Model;

namespace IO2
{
    public static class IoCKernel
    {
        public static Autofac.IContainer Container { get; set; }

        static IoCKernel()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<JsonNoteRepository>().AsImplementedInterfaces().SingleInstance();
            Container = builder.Build();
        }
    }
}