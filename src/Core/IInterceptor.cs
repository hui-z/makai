using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai
{
    public interface IInterceptor
    {
        IObservable<Unit> Process(object obj);
    }
}
