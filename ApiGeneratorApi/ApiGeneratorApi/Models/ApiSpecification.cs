using System;
using System.Collections.Generic;

namespace ApiGeneratorApi.Models
{
    public class ApiSpecification
    {
        public List<EndpointSpec> Endpoints { get; set; }
    }

    public class EndpointSpec
    {
        public string Uri { get; set; }
        public string HttpVerb { get; set; }
        public List<PayloadFieldSpec> Request { get; set; }
        public List<ResponseSpec> Responses { get; set; } 
    }

    public class ResponseSpec
    {
        public int StatusCode { get; set; }
        public List<PayloadFieldSpec> Body { get; set; } 
    }

    public class PayloadFieldSpec
    {
        public string Name{ get; set; }
        public string Type { get; set; }
    }
}