using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Logging
{
    public class CompareResult
    {
        public string Event { get; set; }
        public string Message { get; set; }

        public CompareResult(string _event, string _message)
        {
            Event = _event;
            Message = _message;
        }
    }
}
