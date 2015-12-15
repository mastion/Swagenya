using System.Data;
using System.Linq;
using Dapper;
using Giftango.Component.Utility;
using Giftango.Domain.Models;
namespace Giftango.Domain.Writer
{
   public class DrinkWriter
   {
      public int Write(Drink toWrite)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<int>("[dbo].[InsertDrink]", new { toWrite.name,toWrite.size,toWrite.price }, commandType: CommandType.StoredProcedure).FirstOrDefault();
         }
      }

      public int WriteById(int id, Drink toWrite)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<int>("[dbo].[UpdateDrink]", new { id, toWrite.name,toWrite.size,toWrite.price}, commandType: CommandType.StoredProcedure).FirstOrDefault();
         }
      }
   }
}
