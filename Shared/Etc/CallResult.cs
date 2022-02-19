using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CallResult<T> : CallResult
    {
        /// <summary>
        /// The data returned by the call
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Raw data
        /// </summary>
        public string RawData { get; set; }
    }

    public class CallResult : ICallResult
    {
        /// <summary>
        /// Previous CallResult, if was added.
        /// </summary>
        public List<ICallResult> PreviousCallResult { get; set; } = new List<ICallResult>();

        /// <summary>
        /// An error if the call didn't succeed
        /// </summary>
        public Error Error { get; set; }

        /// <summary>
        /// Whether the call was successful
        /// </summary>
        public bool Success => Error == null;

        /// <summary>
        /// Success message.
        /// </summary>
        public string SuccessMessage { get; set; }

        /// <summary>
        /// Success message.
        /// </summary>
        public string Message { get => Error == null ? SuccessMessage : Error.Message; }

        /// <summary>
        /// Add call result.
        /// </summary>
        /// <param name="callResult"></param>
        public void AddCallResult(ICallResult callResult) => PreviousCallResult.Add(callResult);

        /// <summary>
        /// Success message.
        /// </summary>
        public string GetAllMessages()
        {
            string message = null;
            PreviousCallResult.ForEach(c => message += c.Message + "\n");
            return (message + Message).Trim();
        }
    }
}
