using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace HuiZ.Makai
{
    public class Options
    {
#if DEBUG
        [Option('f', "fiddler", Default = true, HelpText = "upstream proxy to fiddler")]
#else
        [Option('f', "fiddler", Default = false, HelpText = "upstream proxy to fiddler")]
#endif
        public bool Fiddler { get; set; }

        [Option('a', "auth", Default = false, HelpText = "set proxy authentication")]
        public bool Authentication { get; set; }

        public int Port { get; set; }
        //[Option('i', "item-recovery", Default = false, HelpText = "use item to recovery ap")]
        public bool ItemRecovery { get; set; }

        public int MonitorPort { get; set; }

    }
}
