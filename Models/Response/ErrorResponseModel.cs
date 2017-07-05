using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSUMAP.Models.Response
{
    public class ErrorResponseModel
    {
        public string Message { get; }

        public ErrorResponseModel(string message)
        {
            this.Message = message;
        }
    }
}