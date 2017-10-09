using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates
{
    public enum MessageType
    {
        Info,
        Error,
        Warning, 
        Success
    }
    public class UpdatesEventArgs :EventArgs
    {
        public string Message { get; set; }
        public MessageType InfoType { get; set; }
    }
}
