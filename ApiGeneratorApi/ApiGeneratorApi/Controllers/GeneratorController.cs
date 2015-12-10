using System.Web.Http;
using ApiGeneratorApi.Models;

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
        public IHttpActionResult Generate(ApiSpecification apiSpecification)
        {
            foreach (var endpoint in apiSpecification.Endpoints)
            {
                new EndPointGenerator(endpoint).Generate();
            }


            return Ok(apiSpecification);
        }

    }
}
