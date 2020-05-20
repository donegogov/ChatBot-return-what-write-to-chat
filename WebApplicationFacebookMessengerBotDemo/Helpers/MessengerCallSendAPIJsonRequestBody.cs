using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationFacebookMessengerBotDemo.Helpers
{
    public class MessengerCallSendAPIJsonRequestBody
    {
        public Recipient recipient { get; set; }
        public Message message { get; set; }
    }

    public class Recipient
    {
        public String id { get; set; }
    }

    public class Message
    {
        public String text { get; set; }
    }
}
