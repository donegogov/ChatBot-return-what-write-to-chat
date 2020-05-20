using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationFacebookMessengerBotDemo.Helpers
{
    public static class ToJSONHelper
    {
        public static string ToJSON(this object obj)
        {
            string json = JsonConvert.SerializeObject(obj);

            return json;
        }
    }
}
