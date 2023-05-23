using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class ChatVM
    {
        public string FullName { get; set; }
        public string Message { get; set; }
        public string UserConnectionId { get; set; }
        public string ConnectionId { get; set; }
    }
}
