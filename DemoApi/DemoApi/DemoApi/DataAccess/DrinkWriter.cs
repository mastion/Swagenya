using System.Data;
using System.Linq;
using Dapper;
using Giftango.Component.Utility;
namespace Giftango.Domain.Writer
{
   public class DrinkWriter
   {
      public int Write(Drink toWrite)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<int>("[dbo].[InsertDrink]"new { toWrite }, commandType: CommandType.StoredProcedure);
         }
      }

      public int WriteById(int id, Drink toWrite)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<int>("[dbo].[UpdateDrink]"new {id, toWrite}, commandType: CommandType.StoredProcedure);
         }
      }
   }
}
