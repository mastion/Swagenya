using System;
using System.Threading.Tasks;
using System.Web.Http;
using ApiGeneratorApi.Models;
using Raml.Parser;

namespace ApiGeneratorApi.Controllers
{
    public class GeneratorController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Generate(ApiSpecification apiSpecification)
        {
            var parser = new RamlParser();
            var test = await parser.LoadAsync(@"C:\code\hackathon\ApiGeneratorApi\ApiGeneratorApi\APISpecs\CoffeeShop.raml");

           // var specDoc = new SpecLoader().Load("coffeeshop");
         

            foreach (var endpoint in apiSpecification.Endpoints)
            {
                new EndPointGenerator(endpoint).Generate();
            }


            return Ok(apiSpecification);
        }

    }
}
