using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai.Game
{
    public class Response<T>
    {
        public T data { get; set; }
        public long error_cd { get; set; }
        public long status_cd { get; set; }
        public long timestamp { get; set; }
    }
}
