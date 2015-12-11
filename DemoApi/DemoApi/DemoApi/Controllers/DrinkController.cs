using System.Web.Http;
using Giftango.Domain.Models;
using Giftango.Domain.Actions;

namespace Giftango.Web.Admin.Pages
{
    public class DrinkController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get(int? id)
        {
            var bl = new DrinkGetAction();
            return Ok(id == null? bl.GetAll(): bl.Get(id.Value);
        }


        [HttpPost]
        public IHttpActionResult Post(Drink data)
        {
          var tmpId = new DrinkPostAction().Write(data);
            return Ok(new DrinkGetAction().Get(tmpId));
        }



    }
}

