using System.Data;
using System.Linq;
using Dapper;
using Giftango.Component.Utility;
using Giftango.Domain.Models;
namespace Giftango.Domain.Writer
{
   public class ExtraWriter
   {
      public int Write(Extra toWrite)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<int>("[dbo].[InsertExtra]", new { toWrite.name,toWrite.size,toWrite.price }, commandType: CommandType.StoredProcedure).FirstOrDefault();
         }
      }

      public int WriteById(int id, Extra toWrite)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<int>("[dbo].[UpdateExtra]", new { id, toWrite.name,toWrite.size,toWrite.price}, commandType: CommandType.StoredProcedure).FirstOrDefault();
         }
      }
   }
}
