using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace HuiZ.Makai
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IInterceptor>().To<Interceptors.Default>().InSingletonScope();
            Bind<Proxy.IServer>().To<Proxy.Server>()
                .InSingletonScope()
                .WithConstructorArgument("port", 9999);
        }
    }
}
