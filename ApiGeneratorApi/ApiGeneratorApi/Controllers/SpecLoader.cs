using System;
using System.Threading.Tasks;
using Raml.Parser;
using Raml.Parser.Expressions;

namespace ApiGeneratorApi.Controllers
{
    public class SpecLoader
    {
        private static string SpecFilePath = @"C:\code\hackathon\ApiGeneratorApi\ApiGeneratorApi\APISpecs\{0}.raml";

        public static string GetSpecFilePath(string apiName)
        {
            return string.Format(SpecFilePath, apiName);
        }

        public async Task<RamlDocument> Load(string api)
        {
            // load a RAML file
            var parser = new RamlParser();
            var test = await parser.LoadAsync(GetSpecFilePath("CoffeeShop"));
            return test;
        }
    }
}