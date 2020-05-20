using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationFacebookMessengerBotDemo.Models
{
    public class WebhookJsonMessage
    { 
        public String @object { get; set; }
        public List<WebhookJsonEntryProperty> entry { get; set; }
    }

    public class WebhookJsonEntryProperty
    {
        public String id { get; set; }

        public int times { get; set; }
        public List<WebhookJsonMessagingProperty> messaging { get; set; }
    }

    public class WebhookJsonMessagingProperty
    {
        public WebhookJsonMessageText message { get; set; }
        public WebhookJsonSender sender { get; set; }
        public WebhookJsonRecipient recipient { get; set; }
        public int timestamp { get; set; }
    }

    public class WebhookJsonMessageText
    {
        public String mid { get; set; }
        public String text { get; set; }
    }

    public class WebhookJsonSender
    { 
        public String id { get; set; }
    }

    public class WebhookJsonRecipient
    {
        public String id { get; set; }
    }
}
