using System.Web.Http;
using Giftango.Domain.Actions;
using Giftango.Domain.Models;

namespace DemoApi.Controllers
{
    public class ExtraController : ApiController
    {

        [HttpPost]
        public IHttpActionResult Post(Extra data)
        {
            new ExtraPostAction().Write(data);
            return Ok(data);
        }


    }
}

