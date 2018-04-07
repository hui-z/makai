using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Ninject.Parameters;
using NLog;

namespace HuiZ.Makai
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleErrors);
        }
        static void Run(Options opt)
        {
            var kernel = new StandardKernel(new Module());
            var server = kernel.Get<Proxy.IServer>(new ConstructorArgument("fiddler", opt.Fiddler));
            server.Subscribe();

            //var factory = kernel.Get<IRequesterFactory>();
            //var requester = factory.Get("eea6ac3c3051612c-635ccf3f-46743e55-b741df28-7fc0d97832b8b9bd09a18d43");
            //requester.SellEquips(8023654).Subscribe(Console.WriteLine);

            Console.ReadLine();
        }
        static void HandleErrors(IEnumerable<Error> errs)
        {
            errs.ForEach(Console.WriteLine);
        }
    }
}
