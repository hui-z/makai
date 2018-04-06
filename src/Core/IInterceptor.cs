using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace HuiZ.Makai
{
    public interface IInterceptor
    {
        IObservable<Unit> ProcessRequest(SessionEventArgs e);
        IObservable<Unit> ProcessResponse(SessionEventArgs e);
    }
}
