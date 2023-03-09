using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.classes
{
    [Serializable]
    public class Message
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
