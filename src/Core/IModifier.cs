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
        int Priority { get; }
        bool CanModify(Context ctx);
        Reply Process(Context ctx, Reply reply);
    }

    public class Reply
    {
        public int Code { get; set; }
        public dynamic Body { get; set; }
    }
}
