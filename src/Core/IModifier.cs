using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai
{
    public interface IModifier
    {
        bool CanModify(string path);
        dynamic Process(dynamic json);
    }
}
