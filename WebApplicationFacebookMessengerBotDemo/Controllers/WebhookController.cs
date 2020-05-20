using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApplicationFacebookMessengerBotDemo.Helpers;
using WebApplicationFacebookMessengerBotDemo.Models;

namespace WebApplicationFacebookMessengerBotDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IOptions<PageAccessTokenVerifyToken> _pageAccessTokenVerifyTokenConfig;
        private readonly ILogger _logger;

        public WebhookController(IOptions<PageAccessTokenVerifyToken> pageAccessTokenVerifyTokenConfig, ILogger<WebhookController> logger)
        {
            // Facebook Page Access Token is private to you
            // in this project I store it in appsettings.json
            _pageAccessTokenVerifyTokenConfig = pageAccessTokenVerifyTokenConfig;
            _logger = logger;
            _logger.LogInformation("Webhook kontroler konstruktor");
        }

        // GET: Webhook
        [HttpGet]
        public ActionResult Get([FromQuery(Name = "hub.mode")] String mode, [FromQuery(Name = "hub.verify_token")] String token
            , [FromQuery(Name = "hub.challenge")] String challenge)
        {
            _logger.LogInformation($"Get method mode={mode} token={token} challenge={challenge} ");
            // Checks if a token and mode is in the query string of the request
            if (mode != null && token != null)
            {
                // Checks the mode and token sent is correct
                if (mode.Equals("subscribe") && token.Equals(_pageAccessTokenVerifyTokenConfig.Value.VERIFY_TOKEN))
                {
                    // Responds with the challenge token from the request
                    Console.WriteLine("WEBHOOK_VERIFIED");
                    return Ok(challenge);
                }
                else
                {
                    // Responds with '403 Forbidden' if verify tokens do not match
                    return Forbid();
                }
            }
            return Forbid();
        }

        // POST: Webhook
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                _logger.LogInformation("post method");
                HttpContext.Request.EnableBuffering();

                //dynamic requestJson;
                var json = (dynamic)null;
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = sr.ReadToEnd();
                }
                dynamic data = JsonConvert.DeserializeObject(json);

                // Checks this is an event from a page subscription
                if (data.@object == "page")
                {
                    String sender_psid = " ";
                    // Iterates over each entry - there may be multiple if batched
                    foreach (var entry in data.entry)
                    {
                        foreach (var messaging in entry.messaging)
                        {
                            // Get the sender PSID
                            sender_psid = messaging.sender.id;
                            Console.WriteLine("Sender PSID: " + sender_psid);

                            // Check if the event is a message or postback and
                            // pass the event to the appropriate handler function
                            if (!String.IsNullOrEmpty((string)messaging.message.text))
                            {
                                await handleMessage((string)messaging.sender.id, (string)messaging.message.text);
                            }
                        }
                    }

                    // Returns a '200 OK' response to all requests
                    return Ok("EVENT_RECEIVED");
                }
                else
                {
                    // Returns a '404 Not Found' if event is not from a page subscription
                    return NotFound();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + " " + e);
            }
            
            return NotFound();
        }

        // Handles messages events
        public async Task<bool> handleMessage(String sender_psid, String received_message)
        {
            
            String responseText = "";
            
            // call Regex.Match.
            Match match = Regex.Match(received_message, @"\w+",
                RegexOptions.IgnoreCase);

            //var returnResponse = "";

            //  check the Match for Success.
            if (match.Success)
            {
                // Create the payload for a basic text message
                responseText = $"You sent the message: {received_message}.";

                //var responseTemp = JsonConvert.DeserializeObject(response);
                //returnResponse = JsonConvert.SerializeObject(responseTemp);
            }


            
            // Sends the response message
            return await callSendAPI(sender_psid, responseText);
        }

        // Handles messaging_postbacks events
        public bool handlePostback(String sender_psid, String received_postback)
        {
            return false;
        }

        // Sends response messages via the Send API
        public async Task<bool> callSendAPI(String sender_psid, String response)
        {
            // Query string parameters
            var queryString = new Dictionary<string, string>()
            {
                { "access_token", _pageAccessTokenVerifyTokenConfig.Value.PAGE_ACCESS_TOKEN }
            };

            // Create json for body
            String jsonStringResponse = "{\"recipient\":{\"id\":\" " + sender_psid + "\"},\"message\":{\"text\":\" " + response + "\" } }";

            var content = JsonConvert.DeserializeObject(jsonStringResponse);
            var requestContent = JsonConvert.SerializeObject(content);

            // Create HttpClient
            var client = new HttpClient();
            client.BaseAddress = new Uri(_pageAccessTokenVerifyTokenConfig.Value.FACEBOOK_GRAPH_URL);

            // Query string for the request
            var requestQueryString = QueryHelpers.AddQueryString("", queryString);

            var request = new HttpRequestMessage(HttpMethod.Post, requestQueryString);
            // Setup header(s)
            request.Headers.Add("Accept", "application/json");
            // Add body content
            request.Content = new StringContent(
                requestContent.ToString(),
                Encoding.UTF8,
                "application/json"
            );

            // Send the request
            await client.SendAsync(request);

            return true;
        }
    }
}
