using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Giftango.Component.Utility;
using Giftango.Domain.Models;

namespace Giftango.Domain.Reader
{
   public class DrinkReader
   {
       public interface IDrinkReader
       {
           
       }

      public List<Drink> GetAll()
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<Drink>("[dbo].[GetAllDrink]", commandType: CommandType.StoredProcedure).ToList();
         }
      }
      public Drink GetById(int id)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<Drink>("[dbo].[GetDrink]", new {id}, commandType: CommandType.StoredProcedure).FirstOrDefault();
         }
      }
   }
}
