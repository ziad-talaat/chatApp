using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public sealed class ResultResponse<T>
    {
        public bool Success { get; set; }
        public T  DataSet { get; set; }
    }
}
