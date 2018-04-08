using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyCache;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace HuiZ.Makai
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IInterceptor>().To<Interceptors.Default>().InSingletonScope();
            Kernel?.Bind(x => x
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IModifier>()
                .BindSingleInterface()
                .Configure(b => b.InSingletonScope()));
            Bind<Proxy.IServer>().To<Proxy.Server>()
                .InSingletonScope()
#if DEBUG
                .WithConstructorArgument("port", 9998);
#else
                .WithConstructorArgument("port", 9999);
                #endif

            Bind<IAppCache>().To<CachingService>().InSingletonScope();
            Bind<IRequester>().To<Requester>().InThreadScope();
            Bind<Database>().ToSelf();
        }
    }
}
