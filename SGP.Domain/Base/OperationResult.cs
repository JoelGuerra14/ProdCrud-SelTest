using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGP.Domain.Base
{
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static OperationResult Success(string message, object data = null)
        {
            return new OperationResult { IsSuccess = true, Message = message, Data = data };
        }

        public static OperationResult Failure(string message)
        {
            return new OperationResult { IsSuccess = false, Message = message };
        }
    }
}
