using System.Collections.Generic;
using Giftango.Domain.Models;
using System.Data;
using System.Linq;
using Dapper;
using Giftango.Component.Utility;
namespace Giftango.Domain.Reader
{
   public class DrinkReader
   {
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
