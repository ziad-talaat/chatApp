using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public sealed  class ErrorResult(string message,int status,string?details)
    {
        public string Message { get; set; } = message;
        public int StatusCode { get; set; }=status;
        public string? Details { get; set; }=details;
    }
}
