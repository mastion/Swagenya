using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using ApiGeneratorApi.AMLMappers;
using ApiGeneratorApi.Models;

namespace ApiGeneratorApi.Controllers
{
    public class ApisController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Index(string id)
        {
            var specFilePath = SpecLoader.GetSpecFilePath(id);
            // This text is added only once to the file.
            if (!File.Exists(specFilePath))
            {
                return NotFound();
            }
            var contents = File.ReadAllText(specFilePath);
            return Ok(contents);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Generate(string id)
        {
            var pathToOutputDirectory = "path to output directory";

            var apiSpec = await new SpecLoader().Load(id);
            var mapper = new RAMLMapper(apiSpec);

            new WebApiGenerator(mapper.GetResourceSpecs()).Generate();

            new AngularGenerator(_endpointSpec, modelType).Generate();
            new BusinessLogicGenerator(_endpointSpec, modelType).Generate();
            new DataAccessGenerator(_endpointSpec, modelType).Generate();
            new TestGenerator(_endpointSpec, modelType).Generate();

            return Ok(pathToOutputDirectory);
        }

    }
}