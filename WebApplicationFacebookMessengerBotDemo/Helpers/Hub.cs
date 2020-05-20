using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationFacebookMessengerBotDemo.Models
{
    public static class Hub
    {
        public static String mode { get; set; }
        public static String verify_token { get; set; }
        public static String challenge { get; set; }
    }
}
