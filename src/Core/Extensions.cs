using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai
{
    public static class Extensions
    {
        public static void SubscribeWithLog(this IObservable<string> source, string name, string option = "")
        {
            source.Subscribe(r => Console.WriteLine($"[{name}]: {r} {option}"), 
                ex => Console.WriteLine($"[{name}] exception: {ex.Message}"));
        }
        public static void Protect(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
