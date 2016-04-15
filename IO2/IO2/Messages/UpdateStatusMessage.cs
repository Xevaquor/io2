using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO2.Messages
{
    public class UpdateStatusMessage
    {
        public string StatusText { get; private set; }

        public UpdateStatusMessage(string status)
        {
            StatusText = status;
        }
    }
}
