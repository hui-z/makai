﻿using System;
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
    }
}
