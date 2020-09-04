using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RequestResponseFlow
{
    public class JsonRequest
    {
        public class ScoreLoc
        {
            [JsonProperty(Required = Required.AllowNull)]
            public string? Address1 { get; set; }

            [JsonProperty(Required = Required.AllowNull)]
            public string? Address2 { get; set; }

            [JsonProperty(Required = Required.AllowNull)]
            public string? City { get; set; }

            [JsonProperty(Required = Required.AllowNull)]
            public string? State { get; set; }

            [JsonProperty(Required = Required.AllowNull)]
            public string? Zip { get; set; }

            [JsonProperty(Required = Required.AllowNull)]
            public string? Client_OccupancyCode { get; set; }

            [JsonProperty(Required = Required.AllowNull)]
            public string? Client_TIV { get; set; }



        }
        [JsonProperty(Required = Required.AllowNull)]
        public string? Client { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string? UserID { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string? RequestToken { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string? treatyNo { get; set; }
        
        private List<ScoreLoc> list;
        [JsonProperty(Required = Required.AllowNull)]
        public List<ScoreLoc> scorelocs { get { return list; } set { list = value; } }


    }
    public class ResponseJson
    {
        public string? _comment { get; set; }
        public string? ResponseEnvelope { get; set; }
        public string? Client { get; set; }
        public string? UserID { get; set; }
        public string? RequestToken { get; set; }
        public string? JobID { get; set; }
        public Response Response { get; set; }
    }

    public class Response
    {
        public string? ResponseType { get; set; }
        public string? Message { get; set; }
    }

}
