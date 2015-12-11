using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ApiGeneratorApi.Models;
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
            return _doc.Resources.Select(r => new ResourceSpec
            {
                ResourceName = r.RelativeUri,
                Endpoints = r.Methods.Select(m => new EndpointSpec
                {
                    Uri = _doc.BaseUri + r.RelativeUri,
                    HttpVerb = m.Verb, 
                    Request = new List<PayloadFieldSpec>(),
                    Responses = new List<ResponseSpec>()
                }).ToList()
            }).ToList();
        }
    }
}