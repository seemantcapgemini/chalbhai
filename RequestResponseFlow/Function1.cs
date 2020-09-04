using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using System.Text;
using Microsoft.Azure.ServiceBus;
using System.Configuration;

namespace RequestResponseFlow
{
    public static class Function1
    {

        public static JSchema CreateSchema(string content)
        {           
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(JsonRequest));
            return schema;
        }

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Validate request json

            IList<string> messages = new List<string>();
            string jsonBackToClient;
            string jsonBackToClient1;
            try
            {
                // Get Json Request
                string content = string.Empty;
                using (var streamReader = new StreamReader(req.Body, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                }

                // Validate Json Request
                JObject jObj = JObject.Parse(content);
                JSchema schema = CreateSchema(content);                
                bool isValid = jObj.IsValid(schema, out messages);
                if (!isValid)
                {
                    return new OkObjectResult(messages);
                }

                string sstr = Convert.ToString(jObj);

                string myJson = @"
{
   'JobProcID':82,
   'Client':'NationWide',
   'UserID':'UniqueUserID',
   'RequestToken':'Guid',
   'route':'PRESCORE',
   'treatyNo':'4400194',
   'isRealTimeReq':'N',
   'Scorelocs':[
      {
         'JobDetailID':219,
         'Address1':'9515 Deereco Road',
         'Address2':'#610',
         'City':'Timonium',
         'StateCode':null,
         'StateName':null,
         'County':null,
         'Zip':'21093',
         'ZipExt':null,
         'Post_Code_Complete':null,
         'ISO_Country_Code':null,
         'Country_Name':null,
         'Latitude':null,
         'Longitude':null,
         'GEO_Status_Code':null,
         'Search_Business_Key':null,
         'Client_OccupancyCode':'R3',
         'Client_TIV':'',
         'HSB_industry_Code':null,
         'LRScore':'16',
         'LRScoreMessage':'my lrs message',
         'ReasonMessage':'my reason message'
      }
   ],
   'StandardizedScorelocs':[
      {
         'JobDetailID':219,
         'Address1':'9515 DEERECO RD # 610',
         'Address2':'',
         'City':'TIMONIUM',
         'StateCode':'MD',
         'StateName':'MARYLAND',
         'County':null,
         'Zip':'21093',
         'ZipExt':'2116',
         'Post_Code_Complete':'21093-2116',
         'ISO_Country_Code':'US',
         'Country_Name':'UNITED STATES',
         'Latitude':'39.452421',
         'Longitude':'-76.638622',
         'GEO_Status_Code':'EGC9',
         'Search_Business_Key':'LAT39.452421LON-76.638622PCExtd21093-2116Str#9515UnitNoNULLFloorNoNULLSubBldgNo610',
         'Client_OccupancyCode':'R3',
         'Client_TIV':'',
         'HSB_industry_Code':null,
         'LRScore':16,
         'LRScoreMessage':'my lrs message',
         'ReasonMessage':'my rson message'
      }
   ],
   'modelVersionTarget':null,
   'requestEnrichments':null
}                                
";
                dynamic dynJson = JsonConvert.DeserializeObject(myJson);
              //  JArray obj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(myJson);
                foreach (var scoreloc in dynJson.Scorelocs)
                {
                    string strq1 = scoreloc.JobDetailID;

                    if (scoreloc.LRScore<=0)
                    {
                        scoreloc.LRSMessage = "No Score";
                    }
                    else if (scoreloc.LRScore > 0 && scoreloc.LRScore <= 10)
                    {
                        scoreloc.LRSMessage = "low score";
                    }
                    else if (scoreloc.LRScore > 10 && scoreloc.LRScore <= 15)
                    {
                        scoreloc.LRSMessage = "mid Score";
                    }
                    else if (scoreloc.LRScore > 15)
                    {
                        scoreloc.LRSMessage = "high Score";
                    }
                }
                myJson = JsonConvert.SerializeObject(dynJson);
                var reqAnonymousType = new
                {
                    JobProcID="",
                    Client = "",
                    UserID = "",
                    RequestToken = "",
                    route="",
                    treatyNo = "",
                    isRealTimeReq ="",
                    scorelocs = new[] { new { Address1 = "", Address2 = "", City = "", State = "", Zip = "", Client_OccupancyCode = "", Client_TIV = "" , LRScore ="", LRSMessage ="", ReasonMessage =""} }
                };
                var anonymousTypeFilled = JsonConvert.DeserializeAnonymousType(myJson, reqAnonymousType);
                // jsonBackToClient1 = JsonConvert.SerializeObject(anonymousTypeFilled);

                jsonBackToClient1 = JsonConvert.SerializeObject(anonymousTypeFilled);

                // Map Json to class
                JsonRequest lstJson = JsonConvert.DeserializeObject<JsonRequest>(content);
                //var objJson = JsonConvert.SerializeObject(lstJson);
                string str = "sdfdf";
                string str1113221 = string.Empty;
                if (lstJson.scorelocs.Count > 1)
                {

                    // Call Address Standardization


                    // passing Adress Std Json to service queue
                    string SBConnection = Environment.GetEnvironmentVariable("ServiceBusConnectionString",EnvironmentVariableTarget.Process);                   
                    const string QueueName = "outqueue";
                    IQueueClient queueClient = new QueueClient(SBConnection, QueueName);
                    string messageBody = "thu ka test 1";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));                    
                    await queueClient.SendAsync(message);

                }
                else
                {
                    str = "This is single request";
                }

                string JOBID = "12345";
                ResponseJson jsonResponse = new ResponseJson();
                jsonResponse.Client = lstJson.Client;
                jsonResponse.UserID = lstJson.UserID;
                jsonResponse.RequestToken = lstJson.RequestToken;
                jsonResponse.JobID = JOBID; //******************
                jsonResponse.Response = new Response();
                jsonResponse.Response.ResponseType = "Response Receipt";
                jsonResponse.Response.Message = "blank for now, but prep for future explanation";

                jsonBackToClient = JsonConvert.SerializeObject(jsonResponse);
            }
            catch(Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }

            return new OkObjectResult(jsonBackToClient1);
        }
    }
}
