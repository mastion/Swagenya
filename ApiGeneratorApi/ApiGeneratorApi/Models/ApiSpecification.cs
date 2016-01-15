using System.Collections.Generic;
using System.Globalization;

namespace ApiGeneratorApi.Models
{
    public class ApiSpecification
    {
        public List<EndpointSpec> Endpoints { get; set; }
    }

    public class ResourceSpec
    {
        public string ResourceUri { get; set; }
        public string ResourceName { get; set; }
        public string ResourceObjectType { get; set; }
        public List<EndpointSpec> Endpoints { get; set; }
        public List<PayloadFieldSpec> GetModel { get; set; }
        public List<PayloadFieldSpec> PostModel { get; set; }
        public static string ToTitleCase(string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }
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
        public string Name { get; set; }
        public string Type { get; set; }
    }
}