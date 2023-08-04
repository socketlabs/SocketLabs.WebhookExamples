using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocketLabs.WebhookExamples.WebForms
{
    public partial class SocketLabs : System.Web.UI.Page
    {
        // Using JsonPath syntax
        // https://www.newtonsoft.com/json/help/html/QueryJsonSelectToken.htm
        // Example Sent Event
        //
        //{
        //    "data": {
        //        "Type": "Delivered",
        //        "Response": "Sample Response",
        //        "LocalIP": ":01",
        //        "RemoteMta": "Sample RemoteMta",
        //        "DateTime": "2022-09-07T17:49:08.8139901Z",
        //        "MailingId": "SLNL-0-9999999-9999999",
        //        "MessageId": "SampleMessageId",
        //        "Address": "email@example.com",
        //        "ServerId": 123456,
        //        "SecretKey": "n7BJk25EtAr39Sfa4K8R",
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            using (var bodyStream = Request.GetBufferedInputStream())
            using (var sr = new StreamReader(bodyStream))
            using (var jr = new JsonTextReader(sr))
            {
                // Create a JsonSerializer to parse the InputStream
                var serializer = new JsonSerializer();

                // Deserialize the InputStream into a JObject
                var jObject = serializer.Deserialize(jr);

                // Verify the object is of type JOBject and not NULL
                if (jObject is JObject eventData)
                {
                    // Select the SecretKey from the parsed json and see if it matches what we expect it to be.
                    string secretKey = (string)eventData.SelectToken("data.SecretKey");
                    if (secretKey != "n7BJk25EtAr39Sfa4K8R")
                    {
                        // The key doesn't match.  Return forbidden.
                        Response.StatusCode = 403;
                        return;
                    }

                    // Parse out a couple properties from the JSON
                    string type = (string)eventData.SelectToken("data.Type");
                    string address = (string)eventData.SelectToken("data.Address");
                    int serverId = (int)eventData.SelectToken("data.ServerId");

                    // Process them
                    ProcessEvent(type, address, serverId);
                }
                else
                {
                    // We couldn't parse the body. Return Unauthorized.
                    Response.StatusCode = 401;
                    return;
                }
            }
        }

        private void ProcessEvent(string type, string address, int serverId)
        {
            // Store in database.
        }
    }
}
