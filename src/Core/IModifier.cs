using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace HuiZ.Makai
{
    public interface IModifier
    {
        bool CanModify(Context ctx);
        dynamic Process(Context ctx, dynamic json);
    }
}
