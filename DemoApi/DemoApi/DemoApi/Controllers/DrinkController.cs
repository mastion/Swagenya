using System.Web.Http;
using Giftango.Domain.Actions;
using Giftango.Domain.Models;

namespace DemoApi.Controllers
{
    public class DrinkController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get(int? id = null)
        {
            var bl = new DrinkGetAction();
            if (id == null)
                return Ok(bl.GetAll());
            return Ok(bl.Get(id.Value));
        }


        [HttpPost]
        public IHttpActionResult Post(Drink data)
        {
          var tmpId = new DrinkPostAction().Write(data);
            return Ok(new DrinkGetAction().Get(tmpId));
        }


    }
}

