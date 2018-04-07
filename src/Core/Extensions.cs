using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace HuiZ.Makai
{
    public static class Extensions
    {
        public static void SubscribeWithLog(this IObservable<string> source, ILogger logger, string name, string option = "")
        {
            source.Subscribe(r => logger.Info($"[{name}]: {r} {option}"), 
                ex => logger.Error(ex, $"[{name}]"));
        }
        public static void Protect(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                var logger = LogManager.GetLogger("Protect");
                logger.Error(ex);
            }
        }
    }
}
