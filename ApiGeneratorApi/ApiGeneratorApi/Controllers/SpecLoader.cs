using System;
using System.Threading.Tasks;
using Raml.Parser;
using Raml.Parser.Expressions;

namespace ApiGeneratorApi.Controllers
{
    public class SpecLoader
    {
        public async Task<RamlDocument> Load(string api)
        {
            // load a RAML file
            var parser = new RamlParser();
            var test = await parser.LoadAsync(@"C:\code\hackathon\ApiGeneratorApi\ApiGeneratorApi\APISpecs\CoffeeShop.raml");
            return test;
        }
    }
}