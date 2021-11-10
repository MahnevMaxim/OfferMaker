using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Error
    {
        /// <summary>
        /// The error code
        /// </summary>
        public int? Code { get; set; }
        /// <summary>
        /// The message for the error that occured
        /// </summary>
        public string Message { get; set; }

        public Error() { }

        public Error(string message) => Message = message;

        public Error(int code) => Code = code;

        public Error(Exception ex) => Message = ex.StackTrace;

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public Error(string message, Exception ex)
        {
            Message = message;
            Message += "\n" + ex.StackTrace;
        }

        public override string ToString() => Code == null ? Message : $"{Code}: {Message}";
    }
}
