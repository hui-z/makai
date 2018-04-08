using System;
using Titanium.Web.Proxy.EventArguments;

namespace HuiZ.Makai
{
    public class Context
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public IObservable<string> RequestBody { get; set; }
        public string OsType { get; set; }
        public string Path { get; set; }
    }
}
