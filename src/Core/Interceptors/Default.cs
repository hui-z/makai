using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai.Interceptors
{
    public class Default : IInterceptor
    {
        public IObservable<Unit> Process(object obj)
        {
            Console.WriteLine(obj);
            return Observable.Empty<Unit>();
        }
    }
}
