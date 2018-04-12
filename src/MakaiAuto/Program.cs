using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Force.DeepCloner;
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
            var luxury = opt.DeepClone();
            luxury.Port = 9999;
            luxury.ItemRecovery = false;
            var thrift = opt.DeepClone();
            thrift.Port = 6666;
            thrift.ItemRecovery = true;

            RunByOption(luxury);
            RunByOption(thrift);

            Console.ReadLine();
        }
        static void RunByOption(Options opt)
        {
#if DEBUG
            opt.Port--;
#endif
            var kernel = new StandardKernel(new Module());
            kernel.Bind<Options>().ToMethod(_ => opt).InSingletonScope();
            var server = kernel.Get<Proxy.IServer>();
            server.Subscribe();
        }
        static void HandleErrors(IEnumerable<Error> errs)
        {
            errs.ForEach(Console.WriteLine);
        }
    }
}
