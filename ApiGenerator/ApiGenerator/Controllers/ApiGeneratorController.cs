using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiGenerator.Controllers
{
    public class ApiGeneratorController : ApiController
    {
        [HttpPost]
        public IHttpActionResult PostApi(ApiSpecification apiSpecification)
        {
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Index()
        {
            return Ok();
        }
    }

    public class ApiSpecification
    {

    }
}
