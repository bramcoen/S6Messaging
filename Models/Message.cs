using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging 
{
    public class Message
    {
        public Message()
        {

        }
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}