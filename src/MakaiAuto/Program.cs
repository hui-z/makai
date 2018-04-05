using Ninject;
using System;

namespace HuiZ.Makai
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new Module());
            var server = kernel.Get<Proxy.IServer>();
            server.Subscribe();

            Console.ReadLine();
        }
    }
}
