using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ApiGeneratorApi.Models;
using Newtonsoft.Json;
using Raml.Parser.Expressions;

namespace ApiGeneratorApi.AMLMappers
{
    public class RAMLMapper
    {
        private RamlDocument _doc;

        public RAMLMapper(RamlDocument doc)
        {
            _doc = doc;
        }

        public IEnumerable<ResourceSpec> GetResourceSpecs()
        {
            //apiSpecification.First(x => x.Uri != null).Uri;
            //modelType = modelType.First().ToString().ToUpper() + modelType.Substring(1);
            //var modelGenerator = new ModelGenerator(_endpointSpec);
            //modelGenerator.Generate();
            //string modelType = modelGenerator.GetType();
            var specs = new List<ResourceSpec>();
            foreach (var resource in _doc.Resources)
            {
                var resourceName = resource.RelativeUri;

                var getResponseObjectJson =
                resource.Methods.Where(m => m.Verb.ToLower() == "get")
                    .FirstOrDefault()
                    .Responses.Where(resp => resp.Code == "200")
                    .FirstOrDefault()
                    .Body.Values.FirstOrDefault().Schema;

                var getModel = GetObjectFields(getResponseObjectJson);

                var postResponseObjectJson =
                    resource.Methods.Where(m => m.Verb.ToLower() == "post")
                        .FirstOrDefault()
                        .Responses.Where(resp => resp.Code == "201")
                        .FirstOrDefault()
                        .Body.Values.FirstOrDefault().Schema;

                var postModel = GetObjectFields(postResponseObjectJson);
               
                var spec = new ResourceSpec
                {
                    ResourceName = resource.RelativeUri,
                    ResourceObjectType = String.Format("{0}Model", resourceName),
                    GetModel = getModel,
                    PostModel = postModel,
                    Endpoints = resource.Methods.Select(m => new EndpointSpec
                    {
                        Uri = _doc.BaseUri + resource.RelativeUri,
                        HttpVerb = m.Verb,
                        Request = getModel,// m.Body != null && m.Body.Values.Count > 0 ?  GetObjectFields(m.Body.Values.FirstOrDefault().Schema) : new List<PayloadFieldSpec>(),
                        Responses = m.Responses.Select(resp => new ResponseSpec { StatusCode = Int32.Parse(resp.Code), Body = postModel }).ToList() //resp.Body != null ? GetObjectFields(resp.Body.Values.FirstOrDefault().Schema) : new List<PayloadFieldSpec>()}).ToList()
                    }).ToList()
                };
                specs.Add(spec);
            }
            return specs;
        }

        private static List<PayloadFieldSpec> GetObjectFields(string getResponseObjectJson)
        {
            if (!string.IsNullOrWhiteSpace(getResponseObjectJson))
            {
                var getSchemaObject = JsonConvert.DeserializeObject<RamlObjectSchema>(getResponseObjectJson);
                var getModel = getSchemaObject.Properties.Select(prop => new PayloadFieldSpec
                {
                    Name = prop.Name,
                    Type = prop.Type
                }).ToList();
                return getModel;
            }
            return new List<PayloadFieldSpec>();
        }
    }

    public class RamlObjectSchema
    {
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<SchemaField> Properties { get; set; }
    }

    public class SchemaField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
    }
}