using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using ApiGeneratorApi.AMLMappers;
using ApiGeneratorApi.Generator;
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
            var pathToOutputDirectory = WebConfigurationManager.AppSettings["OutputFolder"];

            var apiSpec = await new SpecLoader().Load(id);
            var mapper = new RAMLMapper(apiSpec);

            var resourceSpecifications = mapper.GetResourceSpecs().ToList();
            new ModelGenerator(resourceSpecifications).Generate();
            new WebApiGenerator(resourceSpecifications).Generate2();
            new ActionGenerator(resourceSpecifications).Generate();
            //new TestGenerator(mapper.GetResourceSpecs()).Generate();
            //new DataAccessGenerator(mapper.GetResourceSpecs()).Generate();
            //new StoredProcGenerator(mapper.GetResourceSpecs()).Generate();
            //new AngularGenerator(mapper.GetResourceSpecs()).Generate();

            return Ok(mapper.GetResourceSpecs());
        }

    }
}